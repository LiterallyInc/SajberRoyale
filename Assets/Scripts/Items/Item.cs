using UnityEngine;

public class Item : ScriptableObject
{
    #region Local variables

    [Header("Basics")]
    /// <summary>
    /// Localized & stylized name of the item showing up in-game.
    /// Example: Diamond Sword
    /// </summary>
    public new string name;

    /// <summary>
    /// Description of the item showing up when hovered in-game.
    /// </summary>
    public string description;

    /// <summary>
    /// Key for accessing item through dictionary.
    /// Example: diamondsword
    /// </summary>
    public string ID;

    [Header("In-game")]
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

    [Header("Backend")]
    /// <summary>
    /// Odds of item spawning in its category.
    /// </summary>
    public int spawnWeight;

    /// <summary>
    /// Category of item.
    /// </summary>
    public Type type;

    #endregion Local variables

    public enum Type
    {
        Weapon = 1,
        Ammo = 2,
        Healing = 3
    }
}