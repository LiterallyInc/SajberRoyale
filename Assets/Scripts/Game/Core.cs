using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviourPun
{
    public static Core Instance;
    public const float SpawnOdds = 0.4f;
    public List<NodeInfo> nodeSpawns = new List<NodeInfo>();
    public ItemDatabase ItemDatabase;
    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        MCreateLoot();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) MStartGame();
    }
    [PunRPC]
    private void ConfigureLoot(string[] itemIDs, int[] nodes)
    {
        nodeSpawns.Clear();
        for (int i = 0; i < itemIDs.Length; i++) nodeSpawns.Add(new NodeInfo(itemIDs[i], nodes[i]));

        GameObject[] itemNodes = GameObject.FindGameObjectsWithTag("ItemNode");
        foreach (NodeInfo n in nodeSpawns)
        {
            itemNodes[n.nodeIndex].GetComponent<ItemNode>().SetItem(ItemDatabase.GetItem(n.itemID));
        }
    }
    
    #region Ran by master client only
    /// <summary>
    /// Starts the game for everyone.
    /// </summary>
    public void MStartGame()
    {
        //this is an extremely bad optimized method with poor standards, couldn't be bothered with seralizing shit
        List<int> nodes = new List<int>();
        List<string> items = new List<string>();
        for (int i = 0; i < nodeSpawns.Count; i++)
        {
            items.Add(nodeSpawns[i].itemID);
            nodes.Add(nodeSpawns[i].nodeIndex);
        }
        this.photonView.RPC("ConfigureLoot", RpcTarget.All, (object)items.ToArray(), (object)nodes.ToArray());
    }
    /// <summary>
    /// Creates a list with all loot and saves it locally. 
    /// </summary>
    private void MCreateLoot()
    {
        nodeSpawns.Clear();
        Debug.Log("Core/CreateLoot: MASTER: Creating node spawn list...");
        GameObject[] itemNodes = GameObject.FindGameObjectsWithTag("ItemNode");
        for (int i = 0; i < itemNodes.Length; i++)
        {
            ItemNode node = itemNodes[i].GetComponent<ItemNode>();
            Item item = node.MGetItem();
            if (item != null)
            {
                nodeSpawns.Add(new NodeInfo(item.ID, i));
            }
        }
        Debug.Log($"Core/CreateLoot: MASTER: Created a node spawn list. Added items to {nodeSpawns.Count}/{itemNodes.Length} nodes.");
    }
    #endregion
}
