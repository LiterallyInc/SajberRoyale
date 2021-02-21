//docs: https://github.com/LiterallyInc/SajberRoyale/issues/9

using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using System.Collections;
using UnityEngine;

namespace SajberRoyale.Player
{
    public class DamageController : MonoBehaviourPun
    {
        public AudioClip[] damageSounds;

        /// <summary>
        /// Runs for everyone when someone gets hit
        /// </summary>
        /// <param name="actorID">Actor ID of player who got hit</param>
        /// <param name="damage">How much damage the hit did</param>
        /// <param name="weaponID">Weapon ID of weapon that hit player</param>
        /// <param name="info">Info about the player who hit</param>
        [PunRPC]
        private void Hit(int actorID, int damage, string weaponID, PhotonMessageInfo info)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorID) //i got hit
            {
                Game.Game.Instance.HP -= damage;
                if (Game.Game.Instance.HP <= 0)
                {
                    Game.Game.Instance.IsAlive = false;
                    PhotonNetwork.Destroy(Core.Instance.Player.gameObject); //destroy avatar
                    photonView.RPC("Die", RpcTarget.All, info.Sender.ActorNumber, weaponID);
                }
            }
            else //someone else got hit
            {

            }

            PlayAudioAtPlayer(actorID, 5, damageSounds[Random.Range(0, damageSounds.Length)]);
        }

        /// <summary>
        /// Runs for everyone when someone uses their weapon
        /// </summary>
        /// <param name="weaponID">Weapon ID of weapon that shot</param>
        /// <param name="info">Info about the player who shot</param>
        [PunRPC]
        private void Fire(string weaponID, PhotonMessageInfo info)
        {
            Weapon weapon = (Weapon)ItemDatabase.Instance.GetItem(weaponID);
            PlayAudioAtPlayer(info.Sender.ActorNumber, weapon.range * 1.25f, weapon.shootSFX);
        }

        /// <summary>
        /// Plays a death effect for everyone. F
        /// </summary>
        /// <param name="killer">Player actor ID who killed player</param>
        /// <param name="weaponID">Weapon ID that killed player</param>
        /// <param name="info">Player who dies</param>
        [PunRPC]
        private void Die(int killer, string weaponID, PhotonMessageInfo info)
        {
            Debug.Log($"{info.Sender.NickName} got killed by {PhotonNetwork.CurrentRoom.GetPlayer(killer).NickName} using {ItemDatabase.Instance.GetItem(weaponID).name}");
            Transform dead = Core.Instance.GetPlayer(info.Sender.ActorNumber);
            dead.GetComponent<PlayerSync>().DeathParticles.Play();
            if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) Destroy(dead.GetComponent<CharacterController>());
            if (PhotonNetwork.LocalPlayer.ActorNumber == killer) Game.Game.Instance.Kills++;
            Game.Game.Instance.AlivePlayers--;
        }

        /// <summary>
        /// Plays an audio clip at the players position
        /// </summary>
        /// <param name="actorID">Actor ID of player to play audio at</param>
        private void PlayAudioAtPlayer(int actorID, float range, AudioClip audio)
        {
            GameObject player = GameObject.Find($"Player{actorID}");
            player.GetComponent<AudioSource>().maxDistance = range;
            player.GetComponent<AudioSource>().clip = audio;
            player.GetComponent<AudioSource>().Play();
        }
    }
}