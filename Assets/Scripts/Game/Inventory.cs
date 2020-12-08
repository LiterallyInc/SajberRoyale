using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
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
    public int currentSelected;
    /// <summary>
    /// Texture used when item is not present
    /// </summary>
    public Sprite baseTexture;
    public void TakeItem(Item item)
    {
        int slotIndex = -1;
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                slotIndex = i;
            }
        }
        if (slotIndex == -1) slotIndex = currentSelected;
        items[slotIndex] = item;
        icons[slotIndex].texture = item.icon.texture;

    }
}
