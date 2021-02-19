using System.Collections.Generic;
using UnityEngine;

namespace SajberRoyale.Items
{
    public class LootList : MonoBehaviour
    {
        public int nodeID;
        public string itemID;

        /// <summary>
        /// List with all loot positions, created by master
        /// </summary>
        public static List<LootList> loot = new List<LootList>();
    }
}