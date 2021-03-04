using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using SajberRoyale.Map;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
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

        public AudioClip flashlight;
        public AudioClip Health;

        public string[] EmoteNames;
        public AudioClip[] EmoteSfx;

        private int emoteid;

        private void Update()
        {
            //pickup item
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.CompareTag("ItemNode") && Input.GetKeyDown(KeyCode.E) && Game.Game.Instance.IsAlive && hit.distance < 2.5f)
            {
                ItemNode node = hit.transform.gameObject.GetComponent<ItemNode>();
                if (hit.transform.gameObject.GetComponent<Locker>()) //locker
                {
                    Locker locker = hit.transform.gameObject.GetComponent<Locker>();
                    if (!locker.isOpen)
                        Core.Instance.photonView.RPC(nameof(Core.OpenLocker), RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
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
                if (item.type == Item.Type.Weapon || item.type == Item.Type.Melee)
                {
                    UseWeapon((Weapon)item);
                }
                else if (item.type == Item.Type.Healing)
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
            if (Input.GetKeyDown(KeyCode.B) && !Core.Instance.Sync.isDancing && Game.Game.Instance.IsActive)
            {
                if (Input.GetKey(KeyCode.Alpha0)) StartCoroutine(Emote(0));
                else StartCoroutine(Emote());
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

            //cancel dancing & healing
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.C))
            {
                StopEmote();
                if (!Input.GetKey(KeyCode.C)) Core.Instance.UI.FillPercentage = -1;
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
            if (Core.Instance.Sync.isDancing) return;
            //return if user is on global cooldown
            if (!Game.Game.Instance.canShoot)
            {
                if (weapon.shootingDelay < 0.3f) QueuedShot = weapon;
                return;
            }
            StartCoroutine(Cooldown(weapon.shootingDelay));
            Physics.queriesHitTriggers = false;

            //hit target in range
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit target) //shoot raycast
                && target.transform.CompareTag("Player") //hit player
                && Vector3.Distance(Core.Instance.Player.position, target.transform.position) < weapon.range) //player is in range
            {
                int owner = target.transform.gameObject.GetComponent<PhotonView>().ControllerActorNr;
                int damage = Mathf.RoundToInt(Random.Range(weapon.minDamage, weapon.maxDamage));
                photonView.RPC(nameof(DamageController.Hit), RpcTarget.All, owner, damage, weapon.ID, Game.Game.Instance.Skin);
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
            if (QueuedShot != null)
            {
                UseWeapon(QueuedShot);
                QueuedShot = null;
            }
        }

        private IEnumerator Heal(Healing healing)
        {
            AudioSource source = Core.Instance.PlayerController.GetComponent<AudioSource>();
            Item h = Core.Instance.Inventory.CurrentWeapon;
            isHealing = true;
            Core.Instance.UI.FillPercentage = 0.01f;
            source.clip = healing.useSfx;
            source.Play();
            int i = 1;
            while (i < 100)
            {
                if (Core.Instance.UI.FillPercentage == -1 || Core.Instance.Inventory.CurrentWeapon != h)
                {
                    Core.Instance.UI.FillPercentage = -1;
                    isHealing = false;
                    source.Stop();
                    yield break;
                }
                else
                {
                    Core.Instance.UI.FillPercentage = i * 0.01f;
                }
                yield return new WaitForSeconds(healing.useTime / 100);
                i++;
            }
            Game.Game.Instance.HP += healing.health;
            if (Game.Game.Instance.HP > 100) Game.Game.Instance.HP = 100;
            isHealing = false;
            Core.Instance.UI.FillPercentage = 0;
            Core.Instance.Inventory.RemoveItem();
            source.clip = Health;
            source.Play();
        }

        private IEnumerator Emote(int emoteIndex = -1)
        {
            if (emoteIndex == -1) emoteIndex = Random.Range(0, EmoteNames.Length);

            if (Game.Game.Instance.IsAlive) photonView.RPC(nameof(ToggleEmote), RpcTarget.All, true, emoteIndex);
            Core.Instance.Sync.isDancing = true;
            Core.Instance.Sync.LocalHolder.SetActive(false);
            emoteid = Random.Range(0, 10000);
            int hash = emoteid;
            Core.Instance.Player.GetComponent<Animator>().Play(EmoteNames[emoteIndex], 1, 0);
            Core.Instance.Player.GetComponent<Animator>().Play(EmoteNames[emoteIndex], 2, 0);

            //cancel if emote is non-looping
            if (EmoteNames[emoteIndex] == "Dance Moves")
            {
                yield return new WaitForSeconds(6.9f);
                if (hash == emoteid) StopEmote();
            }
        }

        private void StopEmote()
        {
            if (Core.Instance.Sync.isDancing)
            {
                Core.Instance.Sync.isDancing = false;
                Core.Instance.Sync.LocalHolder.SetActive(true);
                Core.Instance.Player.GetComponent<Animator>().Play("Idle", 1, 0);
                Core.Instance.Player.GetComponent<Animator>().Play("Idle", 2, 0);
                if (Game.Game.Instance.IsAlive) photonView.RPC(nameof(ToggleEmote), RpcTarget.All, false, 0);
            }
        }

        [PunRPC]
        private void ToggleEmote(bool enable, int emoteIndex, PhotonMessageInfo info)
        {
            Animator anim = Core.Instance.GetPlayer(info.Sender.ActorNumber).GetComponent<PlayerSync>().Player.GetComponent<Animator>();
            if (enable)
            {
                anim.Play(EmoteNames[emoteIndex], 1, 0);
                anim.Play(EmoteNames[emoteIndex], 2, 0);
                Core.Instance.DamageController.PlayAudioAtPlayer(info.Sender.ActorNumber, 7, EmoteSfx[emoteIndex], "emote", emoteIndex != 0);
            }
            else
            {
                anim.Play("Idle", 1, 0);
                anim.Play("Idle", 2, 0);
                Destroy(GameObject.Find($"Player{info.Sender.ActorNumber}/emote"));
            }
        }

        [PunRPC]
        public void ToggleFlashlight(bool enable, PhotonMessageInfo info)
        {
            Transform player = Core.Instance.GetPlayer(info.Sender.ActorNumber);
            player.GetComponent<PlayerSync>().PublicLight.enabled = enable;
            player.GetComponent<AudioSource>().clip = flashlight;
            player.GetComponent<AudioSource>().maxDistance = 3;
            player.GetComponent<AudioSource>().Play();
        }
    }
}