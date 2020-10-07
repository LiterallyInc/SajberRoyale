using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Melee", menuName = "SajberRoyale/Melee", order = 1)]
[System.Serializable]
public class Melee : Item
{
    /// <summary>
    /// Damage output from weapon is randomized between these two values
    /// </summary>
    public float minDamage, maxDamage;

    /// <summary>
    /// Wether the weapon in an animation or not
    /// </summary>
    public bool onCooldown;

    /// <summary>
    /// Played every time the weapon hits
    /// </summary>
    public AudioClip hitSFX;

    /// <summary>
    /// Played every time the weapon swings
    /// </summary>
    public AudioClip swingSFW;


}
