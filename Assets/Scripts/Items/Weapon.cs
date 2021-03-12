using System.Collections.Generic;
using UnityEngine;

namespace SajberRoyale.Items
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "SajberRoyale/Weapon", order = 1)]
    [System.Serializable]
    public class Weapon : Item
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
        public AudioClip[] shootSFX;

        /// <summary>
        /// Played every time the weapon is reloading
        /// </summary>
        public AudioClip reloadSFX;
    }

    public class AmmoHolder
    {
        /// <summary>
        /// List of all ammo holders in game. Cleared in ItemDB before start.
        /// </summary>
        public static List<AmmoHolder> Ammo = new List<AmmoHolder>();

        public string WeaponID;
        public int MaxBullets;
        public int Bullets;
        public bool CanShoot { get { return Bullets > 0; } }

        public AmmoHolder(string ID, int max)
        {
            WeaponID = ID;
            MaxBullets = max;
            Bullets = max;
            Ammo.Add(this);
        }

        public void Reload()
        {
            Bullets = MaxBullets;
        }

        public void Shoot()
        {
            Bullets--;
        }
        public static AmmoHolder Get(string id)
        {
            foreach(AmmoHolder a in Ammo)
            {
                if (a.WeaponID == id) return a;
            }
            return null;
        }
    }
}