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
        private void Update()
        {
            //pickup item
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.CompareTag("ItemNode") && Input.GetKeyDown(KeyCode.F))
            {
                ItemNode node = hit.transform.gameObject.GetComponent<ItemNode>();

                if (hit.transform.gameObject.GetComponent<Locker>()) //locker
                {
                    Locker locker = hit.transform.gameObject.GetComponent<Locker>();
                    if (!locker.isOpen)
                        Core.Instance.photonView.RPC("OpenLocker", RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
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
            if (Input.GetMouseButtonDown(0) && Core.Instance.Inventory.CurrentWeapon != null && vp_Utility.LockCursor && !Game.Game.Instance.GracePeriod)
            {
                Item item = Core.Instance.Inventory.CurrentWeapon;
                if (item.type == Item.Type.Weapon)
                {
                    UseWeapon((Weapon)item);
                }
                else if (item.type == Item.Type.Healing)
                {
                    UseHealing((Healing)item);
                }
            }
        }

        private void TakeItem(ItemNode node, RaycastHit hit)
        {
            Item i = node.item;
            Core.Instance.photonView.RPC("DestroyNode", RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
            GetComponent<Inventory>().TakeItem(i);
        }

        private void UseWeapon(Weapon weapon)
        {
            //return if user is on global cooldown
            if (!Game.Game.Instance.canShoot) return;
            StartCoroutine(Cooldown(weapon.shootingDelay));
            Physics.queriesHitTriggers = false;

            //hit target in range
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit target) //shoot raycast
                && target.transform.CompareTag("Player") //hit player 
                && Vector3.Distance(Core.Instance.Player.position, target.transform.position) < weapon.range) //player is in range
            {
                int owner = target.transform.gameObject.GetComponent<PhotonView>().ControllerActorNr;
                int damage = Mathf.RoundToInt(Random.Range(weapon.minDamage, weapon.maxDamage));
                photonView.RPC("Hit", RpcTarget.All, owner, damage, weapon.ID);
                return;
            }
            Physics.queriesHitTriggers = true;
            // no target was in range
            photonView.RPC("Fire", RpcTarget.All, weapon.ID);
        }

        private void UseHealing(Healing healing)
        {
        }

        private IEnumerator Cooldown(float time)
        {
            Game.Game.Instance.canShoot = false;
            yield return new WaitForSeconds(time);
            Game.Game.Instance.canShoot = true;
        }
    }
}