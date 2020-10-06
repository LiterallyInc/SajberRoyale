using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    /// <summary>
    /// Damage output from weapon is randomized between these two values
    /// </summary>
    public float minDamage, maxDamage;

    /// <summary>
    /// Delay between shots
    /// </summary>
    public float shootingDelay;

    /// <summary>
    /// Wether the trigger can be held or not
    /// </summary>
    public bool isAuto;

    /// <summary>
    /// Wether the weapon is on cooldown or not
    /// </summary>
    public bool hasShot;

    /// <summary>
    /// Size of the weapon clip
    /// </summary>
    public int clipSize;

    /// <summary>
    /// Played every time the weapon is shot
    /// </summary>
    public AudioClip shootSFX;


}
