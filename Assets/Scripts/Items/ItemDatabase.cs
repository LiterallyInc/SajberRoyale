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
    public Item GetItem(string id)
    {
        foreach(Item item in Items)
        {
            if (item.ID == id) return item;
        }
        Debug.LogError($"Tried getting invalid weapon: {id}");
        return null;
    }
}
