using UnityEngine;

namespace SajberRoyale.Items
{
    [CreateAssetMenu(fileName = "Melee", menuName = "SajberRoyale/Melee", order = 1)]
    [System.Serializable]
    [System.Obsolete]
    public class _Melee : Item
    {
        /// <summary>
        /// Damage output from weapon is randomized between these two values
        /// </summary>
        public float minDamage, maxDamage;

        /// <summary>
        /// The effective range for applaying damage to the target
        /// </summary>
        public float range;

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
}