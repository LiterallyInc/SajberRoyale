using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class LootList : MonoBehaviour
{
    public int nodeID;
    public string itemID;

    /// <summary>
    /// List with all loot positions, created by master
    /// </summary>
    public static List<LootList> loot = new List<LootList>();
}

