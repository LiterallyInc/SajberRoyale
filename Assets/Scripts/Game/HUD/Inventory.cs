using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

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

    /// <summary>
    /// inserts item in players inventory
    /// </summary>
    public void TakeItem(Item item)
    {
        Debug.Log($"Picked up {item.name} (id: {item.ID})");
        int slotIndex = -1;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                slotIndex = i;
                break;
            }
        }
        if (slotIndex == -1) slotIndex = currentSelected;
        items[slotIndex] = item;
        icons[slotIndex].texture = item.icon.texture;
        GetComponent<AudioSource>().clip = Pickup;
        GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) DropItem();
        SetSlot();
    }

    #region Hotbar control

    private void SetSlot()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetSlot(4);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (currentSelected == 0) SetSlot(4);
            else SetSlot(currentSelected - 1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (currentSelected == 4) SetSlot(0);
            else SetSlot(currentSelected + 1);
        }
    }

    private void SetSlot(int slot)
    {
        currentSelected = slot;
        CurrentWeapon = items[slot];
        selectedOverlay.transform.localPosition = new Vector3(-91.52f + slot * 46.5f, -0.5f, 0);
    }

    private void DropItem()
    {
        if (items[currentSelected] == null) return;
        else
        {
            CurrentWeapon = null;
            Item item = items[currentSelected];
            Debug.Log($"Dropped {item.name} (id: {item.ID})");
            Core.Instance.photonView.RPC("PlaceItem", RpcTarget.All, item.ID, $"{Core.Instance.Player.position.x}|{Core.Instance.Player.position.y}|{Core.Instance.Player.position.z}");
            RemoveItem();
            GetComponent<AudioSource>().clip = Drop;
            GetComponent<AudioSource>().Play();
        }
    }

    private void RemoveItem()
    {
        items[currentSelected] = null;
        icons[currentSelected].texture = baseTexture.texture;
    }

    #endregion Hotbar control
}