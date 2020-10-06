using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    #region Local variables

    /// <summary>
    /// Localized & stylized name of the item showing up in-game.
    /// Example: Diamond Sword
    /// </summary>
    public string name;

    /// <summary>
    /// Description of the item showing up when hovered in-game.
    /// </summary>
    public string description;

    /// <summary>
    /// Key for accessing item through dictionary.
    /// Example: diamondsword
    /// </summary>
    public string ID;

    /// <summary>
    /// Icon for item loaded into the hotbar.
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// Prefab for item spawned in-game.
    /// </summary>
    public GameObject item;

    /// <summary>
    /// Max amount of this item that can be held at the same time.
    /// </summary>
    public int maxStack;

    /// <summary>
    /// Category of item.
    /// </summary>
    public Type type;

    #endregion

    public enum Type
    {
        Weapon,
        Ammo,
        Healing
    }
}
