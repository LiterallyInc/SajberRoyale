using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using SajberRoyale.Map;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SajberRoyale.Player
{
    /// <summary>
    /// Controls player interactions like picking up items, toggling UI etc
    /// </summary>
    public class PlayerManager : MonoBehaviourPun
    {
        private Weapon QueuedShot;
        private bool isHealing = false;
        private bool isReloading = false;
        private UI UI;

        //gets set when user starts eg. reloading, and matched when timer is up
        private int itemHash = 0;

        public AudioClip flashlight;
        public AudioClip Health;
        public AudioClip[] Impact;
        public AudioClip ReloadSfx;

        public Emote[] Emotes;

        private DamageController DMG;
        private int emoteid;

        private void Start()
        {
            DMG = GetComponent<DamageController>();
            UI = GetComponent<UI>();
            Physics.queriesHitTriggers = true;
        }

        private void Update()
        {
            if (!Game.Game.Instance.IsActive) return;
            // if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hits)) Debug.Log(hits.transform.gameObject.name);

            //pickup item
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.CompareTag("ItemNode") && Input.GetKeyDown(KeyCode.E) && Game.Game.Instance.IsAlive && hit.distance < 2.5f)
            {
                Game.Game.Instance.Stats.ItemsPickup++;
                ItemNode node = hit.transform.gameObject.GetComponent<ItemNode>();
                if (hit.transform.gameObject.GetComponent<Locker>()) //locker
                {
                    Locker locker = hit.transform.gameObject.GetComponent<Locker>();
                    if (!locker.isOpen)
                    {
                        Game.Game.Instance.Stats.LockersOpened++;
                        Core.Instance.photonView.RPC(nameof(Core.OpenLocker), RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
                    }
                    else
                        TakeItem(node, hit);
                }
                else
                {
                    TakeItem(node, hit);
                }
            }

            //toggle UI
            if (Input.GetKeyDown(KeyCode.F1) && !Core.Instance.H532.isCursed) Core.Instance.UI.ShowData(!Core.Instance.UI.Data.activeSelf);

            //take screenshot
            if (Input.GetKeyDown(KeyCode.F2) && !Application.isEditor)
            {
                var src = DateTime.Now;
                if (!Directory.Exists(Path.Combine(Application.dataPath, "Screenshots"))) Directory.CreateDirectory(Path.Combine(Application.dataPath, "Screenshots"));
                string filename = $"Screenshots/SajberRoyale {src.Year}.{src.Month}.{src.Day} - {src.Hour}.{src.Minute}.{src.Second}.png";
                ScreenCapture.CaptureScreenshot(filename);
            }
            //use item
            if (Input.GetMouseButtonDown(0) && Core.Instance.Inventory.CurrentWeapon != null && vp_Utility.LockCursor && !Game.Game.Instance.GracePeriod && Game.Game.Instance.IsAlive)
            {
                Item item = Core.Instance.Inventory.CurrentWeapon;
                if (item.GetType() == typeof(Weapon))
                {
                    UseWeapon((Weapon)item);
                }
                else if (item.GetType() == typeof(Healing))
                {
                    UseHealing((Healing)item);
                }
            }

            //use auto weapon
            if (Input.GetMouseButton(0) && Core.Instance.Inventory.CurrentWeapon != null && vp_Utility.LockCursor && !Game.Game.Instance.GracePeriod && Game.Game.Instance.IsAlive && Game.Game.Instance.IsActive)
            {
                if (Core.Instance.Inventory.CurrentWeapon.type == Item.Type.Weapon || Core.Instance.Inventory.CurrentWeapon.type == Item.Type.Melee)
                {
                    Weapon item = (Weapon)Core.Instance.Inventory.CurrentWeapon;
                    if (item.isAuto)
                    {
                        UseWeapon(item);
                    }
                }
            }

            //emote
            if (!Core.Instance.Sync.isDancing)
            {
                if (Input.GetKeyDown(KeyCode.B)) UI.EmoteWheel.SetActive(!UI.EmoteWheel.activeSelf);
                if (Input.GetKeyDown(KeyCode.Alpha0)) StartCoroutine(Emote(0));
                if (Input.GetKeyDown(KeyCode.Alpha9)) StartCoroutine(Emote(1));
                if (Input.GetKeyDown(KeyCode.Alpha8)) StartCoroutine(Emote(2));
                if (Input.GetKeyDown(KeyCode.Alpha7)) StartCoroutine(Emote(3));
                if (Input.GetKeyDown(KeyCode.Alpha6)) StartCoroutine(Emote(4));
                if (Input.GetKeyDown(KeyCode.Alpha5)) StartCoroutine(Emote(5));
            }

            //reload
            if (Input.GetKeyDown(KeyCode.R) && Core.Instance.Inventory.CurrentWeapon != null && !Core.Instance.Sync.isDancing && Game.Game.Instance.IsActive && !isReloading)
            {
                Item item = Core.Instance.Inventory.CurrentWeapon;
                if (item.GetType() == typeof(Weapon))
                {
                    StartCoroutine(Reload((Weapon)item));
                }
            }

            //toggle flashlight
            if (Input.GetKeyDown(KeyCode.F) && Game.Game.Instance.IsActive)
            {
                Core.Instance.Sync.LocalLight.enabled = !Core.Instance.Sync.LocalLight.enabled;
                Core.Instance.PlayerController.GetComponent<AudioSource>().clip = flashlight;
                Core.Instance.PlayerController.GetComponent<AudioSource>().maxDistance = 3;
                Core.Instance.PlayerController.GetComponent<AudioSource>().Play();
                if (Game.Game.Instance.IsAlive) photonView.RPC(nameof(ToggleFlashlight), RpcTarget.Others, Core.Instance.Sync.LocalLight.enabled);
            }

            //cancel dancing, reloading & healing
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.C))
            {
                StopEmote();
                if (!Input.GetKey(KeyCode.C) && isHealing)
                {
                    itemHash = Random.Range(0, 100000);
                    isReloading = false;
                    isHealing = false;
                    Core.Instance.UI.FillArea.StopPlayback();
                    Core.Instance.UI.FillArea.GetComponent<Image>().fillAmount = 0;
                }
            }
        }

        private void TakeItem(ItemNode node, RaycastHit hit)
        {
            Item i = node.item;
            Core.Instance.photonView.RPC(nameof(Core.DestroyNode), RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
            GetComponent<Inventory>().TakeItem(i);
        }

        private void UseWeapon(Weapon weapon)

        {
            AmmoHolder ammo = AmmoHolder.Get(weapon.ID);

            //return if user is dancing or if out of bullets
            if (Core.Instance.Sync.isDancing || !ammo.CanShoot) return;

            //return if user is on global cooldown
            if (!Game.Game.Instance.canShoot)
            {
                if (weapon.shootingDelay < 0.15f) QueuedShot = weapon;
                return;
            }

            StartCoroutine(Cooldown(weapon.shootingDelay));
            Physics.queriesHitTriggers = false;
            Game.Game.Instance.Stats.ShotsFired++;
            ammo.Shoot();
            Core.Instance.UI.Ammo.text = $"{ammo.Bullets}/{ammo.MaxBullets}";
            //hit target in range
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit target) //shoot raycast
                && target.transform.CompareTag("Player") //hit player
                && Vector3.Distance(Core.Instance.Player.position, target.transform.position) < weapon.range) //player is in range
            {
                DMG.PlayAudioAtPlayer(PhotonNetwork.LocalPlayer.ActorNumber, 5, Impact[Random.Range(0, Impact.Length)]);
                Core.Instance.UI.Crosshair.Play("CrosshairJump", 0, 0);
                int owner = target.transform.gameObject.GetComponent<PhotonView>().ControllerActorNr;
                int damage = Mathf.RoundToInt(Random.Range(weapon.minDamage, weapon.maxDamage));
                photonView.RPC(nameof(DamageController.Hit), RpcTarget.All, owner, damage, weapon.ID, Game.Game.Instance.Skin);
                Physics.queriesHitTriggers = true;
                Game.Game.Instance.Stats.DamageDone += damage;
                Game.Game.Instance.Stats.ShotsHit++;
                return;
            }
            Physics.queriesHitTriggers = true;
            // no target was in range
            photonView.RPC(nameof(DamageController.Fire), RpcTarget.All, weapon.ID);
        }

        private void UseHealing(Healing healing)
        {
            if (!isHealing && !Core.Instance.Sync.isDancing)
                StartCoroutine(Heal(healing));
        }

        private IEnumerator Cooldown(float time)
        {
            Game.Game.Instance.canShoot = false;
            yield return new WaitForSeconds(time);
            Game.Game.Instance.canShoot = true;
            if (QueuedShot != null && Input.GetMouseButton(0))
            {
                UseWeapon(QueuedShot);
                QueuedShot = null;
            }
        }

        private IEnumerator Heal(Healing healing)
        {
            Item h = Core.Instance.Inventory.CurrentWeapon;
            isHealing = true;
            itemHash = Random.Range(0, 100000);
            int hash = itemHash;
            photonView.RPC(nameof(Heal), RpcTarget.Others, healing.ID, true);
            Core.Instance.DamageController.PlayAudioAtPlayer(PhotonNetwork.LocalPlayer.ActorNumber, 10, healing.useSfx, "heal");
            Core.Instance.UI.FillArea.speed = 1 / healing.useTime;
            Core.Instance.UI.FillArea.Play("CrosshairFill", 0, 0);

            yield return new WaitForSeconds(healing.useTime);
            if(hash == itemHash && isHealing && Core.Instance.Inventory.CurrentWeapon == h)
            {
                Game.Game.Instance.HP += healing.health;
                Game.Game.Instance.Stats.HPRegen += healing.health;
                if (Game.Game.Instance.HP > Game.Game.Instance.MaxHP) Game.Game.Instance.HP = Game.Game.Instance.MaxHP;
                isHealing = false;
                Core.Instance.Inventory.RemoveItem();
                Core.Instance.DamageController.PlayAudioAtPlayer(PhotonNetwork.LocalPlayer.ActorNumber, 10, Health);
                photonView.RPC(nameof(Heal), RpcTarget.Others, healing.ID, false);
            }
        }

        private IEnumerator Reload(Weapon w)
        {
            AmmoHolder ammo = AmmoHolder.Get(w.ID);

            isReloading = true;
            itemHash = Random.Range(0, 100000);
            int hash = itemHash;
            Core.Instance.UI.FillArea.speed = 1 / w.reloadTime;
            Core.Instance.UI.FillArea.Play("CrosshairFill", 0,0);

            yield return new WaitForSeconds(w.reloadTime);
            if (hash == itemHash && isReloading && Core.Instance.Inventory.CurrentWeapon == w)
            {
                ammo.Reload();
                isReloading = false;

                Core.Instance.UI.Ammo.text = $"{ammo.Bullets}/{ammo.MaxBullets}";
                Core.Instance.DamageController.PlayAudioAtPlayer(PhotonNetwork.LocalPlayer.ActorNumber, 10, ReloadSfx);
            }
        }

        private IEnumerator Emote(int emoteIndex = -1)
        {
            UI.EmoteWheel.SetActive(false);
            if (emoteIndex == -1) emoteIndex = Random.Range(0, Emotes.Length);
            Game.Game.Instance.Stats.Emotes++;
            if (Game.Game.Instance.IsAlive) photonView.RPC(nameof(PlayEmote), RpcTarget.All, emoteIndex, Game.Game.Instance.CurrentRoom.allowMusic || !Game.Game.Instance.IsActive);
            vp_LocalPlayer.GoThirdPerson();
            Core.Instance.Sync.isDancing = true;
            Core.Instance.Sync.LocalHolder.SetActive(false);
            emoteid = Random.Range(0, 10000);
            int hash = emoteid;

            //cancel if emote is non-looping
            if (Emotes[emoteIndex].length != 0)
            {
                yield return new WaitForSeconds(Emotes[emoteIndex].length);
                if (hash == emoteid) StopEmote();
            }
        }

        private void StopEmote()
        {
            if (Core.Instance.Sync.isDancing)
            {
                vp_LocalPlayer.GoFirstPerson();
                Core.Instance.Sync.isDancing = false;
                Core.Instance.Sync.LocalHolder.SetActive(true);
                Core.Instance.Player.GetComponent<Animator>().Play("Idle", 1, 0);
                Core.Instance.Player.GetComponent<Animator>().Play("Idle", 2, 0);
                if (Game.Game.Instance.IsAlive) photonView.RPC(nameof(StopEmote), RpcTarget.All);
            }
        }

        [PunRPC]
        private void PlayEmote(int emoteIndex, bool playMusic, PhotonMessageInfo info)
        {
            Animator anim = Core.Instance.GetPlayer(info.Sender.ActorNumber).GetComponent<PlayerSync>().Player.GetComponent<Animator>();
            anim.Play(Emotes[emoteIndex].id, 1, 0);
            anim.Play(Emotes[emoteIndex].id, 2, 0);
            if (playMusic) DMG.PlayAudioAtPlayer(info.Sender.ActorNumber, 7, Emotes[emoteIndex].audio, "emote", emoteIndex != 0, 0.5f);
        }

        [PunRPC]
        private void StopEmote(PhotonMessageInfo info)
        {
            Animator anim = Core.Instance.GetPlayer(info.Sender.ActorNumber).GetComponent<PlayerSync>().Player.GetComponent<Animator>();
            anim.Play("Idle", 1, 0);
            anim.Play("Idle", 2, 0);
            if (GameObject.Find($"Player{info.Sender.ActorNumber}/emote")) Destroy(GameObject.Find($"Player{info.Sender.ActorNumber}/emote"));
        }

        [PunRPC]
        private void Heal(string itemID, bool start, PhotonMessageInfo info)
        {
            Healing h = (Healing)Core.Instance.ItemDatabase.GetItem(itemID);
            DMG.PlayAudioAtPlayer(info.Sender.ActorNumber, 7, start ? h.useSfx : Health);
        }

        [PunRPC]
        public void ToggleFlashlight(bool enable, PhotonMessageInfo info)
        {
            Transform player = Core.Instance.GetPlayer(info.Sender.ActorNumber);
            player.GetComponent<PlayerSync>().PublicLight.SetActive(enable);
            player.GetComponent<AudioSource>().clip = flashlight;
            player.GetComponent<AudioSource>().maxDistance = 3;
            player.GetComponent<AudioSource>().Play();
        }
    }
}