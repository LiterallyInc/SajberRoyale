//docs: https://github.com/LiterallyInc/SajberRoyale/issues/9

using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using UnityEngine;

namespace SajberRoyale.Player
{
    public class DamageController : MonoBehaviourPun
    {
        public GameObject AudioContainer;
        public GameObject KillfeedEntryTemplate;
        public GameObject KillfeedEntry;
        public AudioClip[] damageSounds;
        public Game.Game game;

        private void Start()
        {
            game = Game.Game.Instance;
        }

        /// <summary>
        /// Runs for everyone when someone gets hit
        /// </summary>
        /// <param name="actorID">Actor ID of player who got hit</param>
        /// <param name="damage">How much damage the hit did</param>
        /// <param name="weaponID">Weapon ID of weapon that hit player</param>
        /// <param name="info">Info about the player who hit</param>
        [PunRPC]
        public void Hit(int actorID, int damage, string weaponID, string skin, PhotonMessageInfo info)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorID) //i got hit
            {
                Core.Instance.UI.HPOverlay.parent.GetComponent<Animator>().Play("HealthImpact", 0, 0);
                game.HP -= damage;
                game.Stats.DamageTaken += damage;
                if (game.HP <= 0)
                {
                    game.Stats.Placement = game.AlivePlayers;
                    game.IsAlive = false;
                    PhotonNetwork.Destroy(Core.Instance.Player.gameObject); //destroy avatar
                    Core.Instance.Sync.LocalLight.color = new Color(1, 0.7688679f, 0.7753899f); //tint flashlight

                    Random.InitState(Core.Instance.localSeed);
                    photonView.RPC(nameof(Die), RpcTarget.All, info.Sender.ActorNumber, weaponID, skin, game.Skin, Random.Range(0, Core.Instance.Postgame.VictoryEmotes.Length));
                }
            }
            else //someone else got hit
            {
            }
            AnimateWeapon(weaponID, info.Sender.ActorNumber);
            Weapon weapon = (Weapon)ItemDatabase.Instance.GetItem(weaponID);
            PlayAudioAtPlayer(actorID, 5, damageSounds[Random.Range(0, damageSounds.Length)]);
            PlayAudioAtPlayer(info.Sender.ActorNumber, weapon.range * 2f, weapon.shootSFX[Random.Range(0, weapon.shootSFX.Length)]);
        }

        /// <summary>
        /// Runs for everyone when someone uses their weapon
        /// </summary>
        /// <param name="weaponID">Weapon ID of weapon that shot</param>
        /// <param name="info">Info about the player who shot</param>
        [PunRPC]
        public void Fire(string weaponID, PhotonMessageInfo info)
        {
            AnimateWeapon(weaponID, info.Sender.ActorNumber);
            Weapon weapon = (Weapon)ItemDatabase.Instance.GetItem(weaponID);
            PlayAudioAtPlayer(info.Sender.ActorNumber, weapon.range * 1.25f, weapon.shootSFX[Random.Range(0, weapon.shootSFX.Length)]);
        }

        /// <summary>
        /// Plays a death effect for everyone. F
        /// </summary>
        /// <param name="killer">Player actor ID who killed player</param>
        /// <param name="weaponID">Weapon ID that killed player</param>
        /// <param name="info">Player who dies</param>
        [PunRPC]
        public void Die(int killer, string weaponID, string killerSkin, string killedSkin, int emoteIndex, PhotonMessageInfo info)
        {
            //turn off their flashlight
            photonView.RPC(nameof(PlayerManager.ToggleFlashlight), RpcTarget.Others, false);
            Core.Instance.AliveSkins.Remove(killedSkin);
            Weapon w = (Weapon)ItemDatabase.Instance.GetItem(weaponID);

            AddKillfeedEntry(killer, info.Sender.ActorNumber, killerSkin, killedSkin, w);
            Debug.Log($"{info.Sender.NickName} got killed by {PhotonNetwork.CurrentRoom.GetPlayer(killer).NickName} using {w.name}");

            Transform dead = Core.Instance.GetPlayer(info.Sender.ActorNumber);
            dead.GetComponent<PlayerSync>().DeathParticles.Play();
            if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) Destroy(dead.GetComponent<CharacterController>());
            if (PhotonNetwork.LocalPlayer.ActorNumber == killer) game.Stats.Eliminations++;
            game.AlivePlayers--;

            if (game.AlivePlayers == 1)
            {
                Core.Instance.Postgame.winnerEmote = emoteIndex;
                Core.Instance.Postgame.winnerSkin = Core.Instance.AliveSkins[0];
                Core.Instance.Win();
            }
        }

        /// <summary>
        /// Plays an audio clip at the players position
        /// </summary>
        /// <param name="actorID">Actor ID of player to play audio at</param>
        public void PlayAudioAtPlayer(int actorID, float range, AudioClip audio, string objname = "Audio", bool loop = false)
        {
            GameObject player = GameObject.Find($"Player{actorID}");
            GameObject audioContainer = Instantiate(AudioContainer, player.transform);
            audioContainer.name = objname;
            audioContainer.GetComponent<AudioSource>().clip = audio;
            audioContainer.GetComponent<AudioSource>().loop = loop;
            audioContainer.GetComponent<AudioSource>().maxDistance = range;
        }

        private void AnimateWeapon(string weaponID, int actorID)
        {
            string animation = ItemDatabase.Instance.GetItem(weaponID).type == Item.Type.Weapon ? "Shoot" : "Swing";
            if (actorID == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Animator weaponAnim = Core.Instance.Sync.LocallyHeld.GetComponent<Animator>();
                weaponAnim.Play(animation, -1, 0);
            }
            else
            {
                PlayerSync sync = Core.Instance.GetPlayer(actorID).GetComponent<PlayerSync>();
                sync.PubliclyHeld.GetComponent<Animator>().Play(animation, -1, 0);
            }
        }

        public void AddKillfeedEntry(int killer, int killed, string killerSkin, string killedSkin, Weapon weapon)
        {
            if (KillfeedEntry != null) KillfeedEntry.GetComponent<KillfeedEntry>().Close();
            KillfeedEntry = Instantiate(KillfeedEntryTemplate, Core.Instance.UI.Data.transform);
            KillfeedEntry feed = KillfeedEntry.GetComponent<KillfeedEntry>();
            
            feed.IconKiller.sprite = Resources.Load<Sprite>($"CharPortraits/{killerSkin}");
            feed.IconKilled.sprite = Resources.Load<Sprite>($"CharPortraits/Dead{killedSkin}");
            feed.IconWeapon.sprite = weapon.icon;
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber == killed) feed.TextKilled.text = player.NickName;
                if (player.ActorNumber == killer) feed.TextKiller.text = player.NickName;
            }
            //clear killer if used developer item
            if (weapon.developerItem)
            {
                feed.TextKiller.text = "";
                feed.IconKiller.color = Color.clear;
            }
        }

        private void OnApplicationQuit()
        {
            if (game.IsActive && game.IsAlive) Core.Instance.Inventory.photonView.RPC("Die", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, "suicide", game.Skin, game.Skin);
        }
    }
}