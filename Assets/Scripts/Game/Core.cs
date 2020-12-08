using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        if(PhotonNetwork.IsMasterClient) MCreateLoot();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && PhotonNetwork.IsMasterClient) MStartGame();
    }
    [PunRPC]
    private void PlaceLoot(string[] itemIDs, int[] nodes)
    {
        Debug.Log($"Core/PlaceLoot: Recieved {nodes.Length} items from master.");
        Debug.Log($"CorePlaceLoot:\n{string.Join("|", nodes)}\n{string.Join("|", itemIDs)}");
        nodeSpawns.Clear();
        for (int i = 0; i < itemIDs.Length; i++) nodeSpawns.Add(new NodeInfo(itemIDs[i], nodes[i]));

        //get gameobject list sorted equal to all clients
        GameObject[] itemNodes = GameObject.FindGameObjectsWithTag("ItemNode");
        itemNodes = itemNodes.OrderBy(o => o.transform.position.x * o.transform.position.y * o.transform.position.z).ToArray();

        foreach (NodeInfo n in nodeSpawns)
        {
            itemNodes[n.nodeIndex].GetComponent<ItemNode>().SetItem(ItemDatabase.GetItem(n.itemID));
        }
        foreach (GameObject n in GameObject.FindGameObjectsWithTag("ItemNode"))
        {
            ItemNode node = n.GetComponent<ItemNode>();
            if (!node.hasItem) node.SetItem(null);
        }
    }
    [PunRPC]
    public void DestroyNode(double pos)
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("ItemNode"))
        {
            if (go.transform.position.x * go.transform.position.y * go.transform.position.z == pos) go.GetComponent<ItemNode>().SetItem(null);
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
        this.photonView.RPC("PlaceLoot", RpcTarget.All, (object)items.ToArray(), (object)nodes.ToArray());
    }
    /// <summary>
    /// Creates a list with all loot and saves it locally. 
    /// </summary>
    private void MCreateLoot()
    {
        nodeSpawns.Clear();
        Debug.Log("Core/CreateLoot: MASTER: Creating node spawn list...");

        //get gameobject list sorted equal to all clients
        GameObject[] itemNodes = GameObject.FindGameObjectsWithTag("ItemNode");
        itemNodes = itemNodes.OrderBy(o => o.transform.position.x * o.transform.position.y * o.transform.position.z).ToArray();

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
