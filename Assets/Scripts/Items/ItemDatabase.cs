using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemDatabase : MonoBehaviour
{
    Dictionary<string, Item> ItemList = new Dictionary<string, Item>();
    [SerializeField]
    public Item[] Items;

    private void Start()
    {
        Item test = new Weapon()
        {
            description = "hey",
            type = Item.Type.Ammo
        };

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
