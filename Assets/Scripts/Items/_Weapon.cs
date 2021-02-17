using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "SajberRoyale/Weapon", order = 1)]
[System.Serializable]
public class _Weapon : Item
{
    [Header("Weapon stats")]
    /// <summary>
    /// Damage output from weapon is randomized between these two values
    /// </summary>
    public float minDamage;
    public float maxDamage;

    /// <summary>
    /// The effective range for applaying damage to the target
    /// </summary>
    public float range;

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
    public bool canShoot;

    /// <summary>
    /// Whether the weapon is reloading or not
    /// </summary>
    public bool isReloading;

    /// <summary>
    /// How long it needs to reload
    /// </summary>
    public float reloadTime;

    /// <summary>
    /// Size of the weapon clip
    /// </summary>
    public int clipSize;

    /// <summary>
    /// Played every time the weapon is shot
    /// </summary>
    public AudioClip shootSFX;

    /// <summary>
    /// Played every time the weapon is reloading
    /// </summary>
    public AudioClip reloadSFX;
}
