﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class Core : MonoBehaviourPun
{
    public string[] Meshes;
    public bool GameStarted;
    public static Core Instance;
    public const float SpawnOdds = 0.4f;
    public GameObject Canvas_Pregame;
    public GameObject Canvas_Gameplay;
    public Button Button_Start;

    [HideInInspector]
    public List<NodeInfo> nodeSpawns = new List<NodeInfo>();

    public ItemDatabase ItemDatabase;
    public GameObject Camera;
    public static int seed = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        if (PhotonNetwork.IsMasterClient)
        {
            seed = Random.Range(0, 1000);
            Hashtable rseed = new Hashtable();
            rseed.Add("seed", seed);
            PhotonNetwork.CurrentRoom.SetCustomProperties(rseed);
            MCreateLoot();
            Button_Start.interactable = true;
        }
        else if(PhotonNetwork.IsConnected)
        {
            Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
            seed = (int)hashtable["seed"];
        }
        else if(PhotonNetwork.LocalPlayer.ActorNumber == -1)
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("offline");
            
        }
    
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !Game.IsActive)
        {
            if (PhotonNetwork.IsMasterClient) MStartGame();
            else if (!PhotonNetwork.IsConnected)
            {

                MStartGame();
            }
        }
    }

    [PunRPC]
    private void PlaceLoot(string[] itemIDs, int[] nodes)
    {
        Debug.Log($"Core/PlaceLoot: Recieved {nodes.Length} items from master.");
        if (nodes.Length > 0) Debug.Log($"Core/PlaceLoot:\n{string.Join("|", nodes)}\n{string.Join("|", itemIDs)}");
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
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ItemNode"))
        {
            if (go.transform.position.x * go.transform.position.y * go.transform.position.z == pos) go.GetComponent<ItemNode>().SetItem(null);
        }
    }

    [PunRPC]
    public void PlaceItem(string itemid, string pos)
    {
        GameObject node = Instantiate(Resources.Load("Prefabs/Items/ItemNode") as GameObject);
        node.transform.position = new Vector3(float.Parse(pos.Split('|')[0]), float.Parse(pos.Split('|')[1]) - 0.8f, float.Parse(pos.Split('|')[2]));
        ItemNode itemnode = node.GetComponent<ItemNode>();
        itemnode.SetItem(ItemDatabase.GetItem(itemid));
    }

    [PunRPC]
    public void Summon()
    {
        Destroy(Camera);
        GameObject[] SpawnNodes = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        Vector3 SpawnPos = SpawnNodes[Random.Range(0, SpawnNodes.Length - 1)].transform.position;
        SpawnPos.y++;
        PhotonNetwork.Instantiate("UFPS_Player", SpawnPos, Quaternion.identity);
    }
    [PunRPC]
    private void StartGame()
    {
        StartCoroutine(StartIn());
    }
    #region Ran by master client only
    /// <summary>
    /// Starts the game for everyone.
    /// </summary>
    public void MStartGame()
    {
        Button_Start.interactable = false;
        //this is an extremely bad optimized method with poor standards, couldn't be bothered with seralizing shit
        List<int> nodes = new List<int>();
        List<string> items = new List<string>();
        for (int i = 0; i < nodeSpawns.Count; i++)
        {
            items.Add(nodeSpawns[i].itemID);
            nodes.Add(nodeSpawns[i].nodeIndex);
        }
        this.photonView.RPC("PlaceLoot", RpcTarget.All, (object)items.ToArray(), (object)nodes.ToArray());

        PhotonNetwork.CurrentRoom.IsOpen = false;
        this.photonView.RPC("StartGame", RpcTarget.All);
    }
    IEnumerator StartIn()
    {
        Camera.GetComponent<Animator>().Play("CameraStart");
        yield return new WaitForSeconds(3.3f);
        Summon();
        Canvas_Gameplay.SetActive(true);
        Button_Start.gameObject.SetActive(false);
        Game.StartGame();
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

    private IEnumerator WaitForLights()
    {
        yield return new WaitForSeconds(5);
        RoomNode.Get("H527").Deactivate();
    }

    #endregion Ran by master client only
}