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

    /// <summary>
    /// inserts item in players inventory
    /// </summary>
    public void TakeItem(Item item)
    {
        Debug.Log($"Picked up {item.name} (id: {item.ID}");
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Drop();
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
        selectedOverlay.transform.localPosition = new Vector3(-91.52f + slot * 46.5f, -0.5f, 0);
    }

    private void Drop()
    {
        if (items[currentSelected] == null) return;
        else
        {
            Core.Instance.photonView.RPC("PlaceItem", RpcTarget.All, items[currentSelected].ID, $"{this.transform.position.x}|{this.transform.position.y}|{this.transform.position.z}");
            RemoveItem();
        }
    }

    private void RemoveItem()
    {
        items[currentSelected] = null;
        icons[currentSelected].texture = baseTexture.texture;
    }

    #endregion Hotbar control
}