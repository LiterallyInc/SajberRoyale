using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Player
{
    public class Inventory : MonoBehaviourPun
    {
        /// <summary>
        /// The 5 slots each player have in their inventory.
        /// </summary>
        public Item[] items = new Item[5];

        /// <summary>
        /// Images for icons in inventory
        /// </summary>
        public RawImage[] icons = new RawImage[5];

        /// <summary>
        /// Zero based index of currently selected item
        /// </summary>
        public int currentSelected = 0;

        /// <summary>
        /// Texture used when item is not present
        /// </summary>
        public Sprite baseTexture;

        public GameObject selectedOverlay;

        public AudioClip Drop;
        public AudioClip Pickup;

        public Item CurrentWeapon;

        public GameObject UIInfo;

        /// <summary>
        /// inserts item in players inventory
        /// </summary>
        public void TakeItem(Item item)
        {
            Debug.Log($"Picked up {item.name} (id: {item.ID})");
            int itemPos = ItemPosition(item);

            //check if item is weapon & if user got it
            if (item.GetType() == typeof(Weapon) && itemPos != -1)
            {
                //limit user to 1 per weapon
                SetSlot(itemPos);
                DropItem(itemPos);
            }

            //try to place in first available slot
            int slotIndex = -1;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    slotIndex = i;
                    break;
                }
            }
            //no slot free, place in current
            if (slotIndex == -1)
            {
                slotIndex = currentSelected;
                DropItem();
            }

            items[slotIndex] = item;
            icons[slotIndex].texture = item.icon.texture;
            GetComponent<AudioSource>().clip = Pickup;
            GetComponent<AudioSource>().Play();

            if (slotIndex == currentSelected)
            {
                SetSlot(currentSelected);
            }
        }

        private int ItemPosition(Item item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == item) return i;
            }
            return -1;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) DropItem();
            SetSlot();
        }

        #region Hotbar control

        /// <summary>
        /// Checks if selected slot in inventory should be changed
        /// </summary>
        private void SetSlot()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetSlot(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetSlot(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetSlot(2);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (currentSelected == 0) SetSlot(2);
                else SetSlot(currentSelected - 1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (currentSelected == 2) SetSlot(0);
                else SetSlot(currentSelected + 1);
            }
        }

        /// <summary>
        /// Sets selected slot in inventory
        /// </summary>
        /// <param name="slot">Zero-indexed slot number</param>
        private void SetSlot(int slot)
        {
            Item prevSelected = CurrentWeapon;
            currentSelected = slot;
            CurrentWeapon = items[slot];
            selectedOverlay.transform.localPosition = new Vector3(-48f + slot * 48, 0, 0);

            if (CurrentWeapon != prevSelected)
                SummonItem();

            if (CurrentWeapon != null) //got item
            {
                if (CurrentWeapon.type == Item.Type.Weapon) SetDesc((Weapon)CurrentWeapon);
                if (CurrentWeapon.type == Item.Type.Healing) SetDesc((Healing)CurrentWeapon);
            }
            else
            {
                UIInfo.GetComponent<Text>().text = "";
            }
        }

        /// <summary>
        /// Tries to drop item
        /// </summary>
        /// <param name="pos">Item index. Default is selected</param>
        private void DropItem(int pos = -1)
        {
            if (pos == -1) pos = currentSelected;
            if (items[pos] == null) return;
            else
            {
                CurrentWeapon = null;
                Item item = items[pos];
                Debug.Log($"Dropped {item.name} (id: {item.ID})");
                Core.Instance.photonView.RPC(nameof(Core.PlaceItem), RpcTarget.All, item.ID, $"{Core.Instance.Player.position.x}|{Core.Instance.Player.position.y}|{Core.Instance.Player.position.z}");
                RemoveItem(pos);
                GetComponent<AudioSource>().clip = Drop;
                GetComponent<AudioSource>().Play();
            }
        }

        /// <summary>
        /// Removes item from hotbar
        /// </summary>
        /// <param name="pos">Index of item to remove. Default is selected</param>
        public void RemoveItem(int pos = -1)
        {
            if (pos == -1) pos = currentSelected;
            items[pos] = null;
            CurrentWeapon = null;
            icons[pos].texture = baseTexture.texture;
            SummonItem();
        }

        /// <summary>
        /// Summons a weapon prefab at hand position
        /// </summary>
        private void SummonItem()
        {
            Destroy(PlayerSync.Me.LocallyHeld);
            if (CurrentWeapon == null)
            {
                photonView.RPC("SummonItemOther", RpcTarget.Others, "NONE");
            }
            else
            {
                PlayerSync.Me.LocallyHeld = Instantiate(CurrentWeapon.item);
                PlayerSync.Me.LocallyHeld.transform.parent = PlayerSync.Me.LocalHolder.transform;
                PlayerSync.Me.LocallyHeld.transform.localPosition = Vector3.zero;
                PlayerSync.Me.LocallyHeld.transform.localRotation = Quaternion.identity;
                photonView.RPC("SummonItemOther", RpcTarget.Others, CurrentWeapon.ID);
            }
        }

        /// <summary>
        /// Sets item description in inventory
        /// </summary>
        private void SetDesc(Weapon w)
        {
            string range = "Short";
            if (w.range > 7.5f) range = "Medium";
            else if (w.range > 15) range = "Long";

            UIInfo.GetComponent<Text>().text = $@"<size=20>{w.name}</size>
<size=10><i>{w.description}</i>
➤ {range} ❤ {w.maxDamage} ✱ {Mathf.RoundToInt(60 / w.shootingDelay)}rpm</size>";

            UIInfo.GetComponent<Animator>().Play("InventoryItemInfo", 0, 0);
        }

        private void SetDesc(Healing h)
        {
            UIInfo.GetComponent<Text>().text = $@"<size=20>{h.name}</size>
<size=10><i>{h.description}</i>
❤ {h.health} ✱ {h.useTime}s</size>";

            UIInfo.GetComponent<Animator>().Play("InventoryItemInfo", 0, 0);
        }

        [PunRPC]
        private void SummonItemOther(string weaponID, PhotonMessageInfo info)
        {
            GameObject player = GameObject.Find($"Player{info.Sender.ActorNumber}");
            Destroy(player.GetComponent<PlayerSync>().PubliclyHeld);
            if (weaponID != "NONE")
            {
                player.GetComponent<PlayerSync>().PubliclyHeld = Instantiate(ItemDatabase.Instance.GetItem(weaponID).item);
                player.GetComponent<PlayerSync>().PubliclyHeld.transform.parent = player.GetComponent<PlayerSync>().PublicHolder.transform;
                player.GetComponent<PlayerSync>().PubliclyHeld.transform.localPosition = Vector3.zero;
                player.GetComponent<PlayerSync>().PubliclyHeld.transform.localRotation = Quaternion.identity;
            }
        }

        #endregion Hotbar control
    }
}