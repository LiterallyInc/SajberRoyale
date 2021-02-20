using UnityEngine;

namespace SajberRoyale.Items
{
    [CreateAssetMenu(fileName = "Healing", menuName = "SajberRoyale/Healing", order = 1)]
    [System.Serializable]
    public class Healing : Item
    {
        /// <summary>
        /// Health restored from this item
        /// </summary>
        public float health;
    }
}