using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    [SerializeField]
    public List<Item> Items;
    [HideInInspector]
    public List<Item> weightedItems = new List<Item>();

    private void Awake()
    {
        Instance = this;
        //weights all items
        foreach(Item item in Items)
        {
            for (int i = 0; i < item.spawnWeight; i++)
            {
                weightedItems.Add(item);
            }
        }

    }
    /// <summary>
    /// Gets an item object by its ID
    /// </summary>
    public Item GetItem(string id)
    {
        foreach(Item item in Items)
        {
            if (item.ID == id) return item;
        }
        Debug.LogWarning($"ItemDB: Tried getting invalid weapon: {id}");
        return null;
    }
}
