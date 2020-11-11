using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemDatabase : MonoBehaviour
{
    Dictionary<string, Item> ItemList = new Dictionary<string, Item>();
    [SerializeField]
    public List<Item> Items;

    private void Start()
    {
        Item test = new _Weapon()
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
