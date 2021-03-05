using Photon.Pun;
using SajberRoyale.Items;
using SajberRoyale.Map;
using SajberRoyale.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace SajberRoyale.Game
{
    public class Core : MonoBehaviourPun
    {
        [Header("Objects")]
        public UI UI;
        public Button Button_Start;
        public GameObject Camera;
        public Inventory Inventory;
        public AnimeClub Club;
        public CursedH532 H532;
        public Transform Player; //reference to player gameobject
        public Transform PlayerController;
        public vp_FPInput PlayerInput;
        public PlayerSync Sync;
        public AudioSource VictoryTheme;
        public AudioSource VictoryMusic;
        public DamageController DamageController;
        public PostgameCore Postgame;
        

        [Header("Values")]
        public bool GameStarted = false;

        public static Core Instance;
        public float SpawnOdds = 0.4f;
        public int seed = 0;
        public int localSeed = 0;

        [Header("Data")]
        public ItemDatabase ItemDatabase;

        public string[] Meshes; //all character names
        [HideInInspector] public List<NodeInfo> nodeSpawns = new List<NodeInfo>();

        // Start is called before the first frame update
        private void Awake()
        {
            localSeed = Random.Range(0, 100000);
            Instance = this;
            if (PhotonNetwork.IsMasterClient)
            {
                seed = Random.Range(0, 1000);
                Hashtable ht = new Hashtable();
                ht.Add("seed", seed);
                ht.Add("isTournament", Game.Instance.IsTournament);
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
                MCreateLoot();
                Button_Start.interactable = true;
            }
            else if (PhotonNetwork.IsConnected)
            {
                Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
                seed = (int)hashtable["seed"];
                Game.Instance.IsTournament = (bool)hashtable["isTournament"];
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == -1)
            {
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.CreateRoom("offline");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) && !Game.Instance.IsActive)
            {
                if (PhotonNetwork.IsMasterClient) MStartGame();
                else if (!PhotonNetwork.IsConnected)
                {
                    MStartGame();
                }
            }

            if (PlayerInput != null) if (!PlayerInput.MouseLookAcceleration) PlayerInput.MouseLookSensitivity = new Vector2(Helper.sens, Helper.sens);
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
        public void OpenLocker(double pos)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("ItemNode"))
            {
                if (go.transform.position.x * go.transform.position.y * go.transform.position.z == pos) go.GetComponent<Locker>().Open();
            }
        }

        [PunRPC]
        public void PlaceItem(string itemid, string pos)
        {
            GameObject node = Instantiate(Resources.Load("Prefabs/Items/ItemNode") as GameObject);
            node.transform.position = new Vector3(float.Parse(pos.Split('|')[0]), float.Parse(pos.Split('|')[1]), float.Parse(pos.Split('|')[2]));
            ItemNode itemnode = node.GetComponent<ItemNode>();
            itemnode.SetItem(ItemDatabase.GetItem(itemid));
        }

        [PunRPC]
        public void Summon()
        {
            Camera.SetActive(false);
            GameObject[] SpawnNodes = GameObject.FindGameObjectsWithTag("PlayerSpawn");
            Random.InitState(localSeed);
            Vector3 SpawnPos = SpawnNodes[Random.Range(0, SpawnNodes.Length - 1)].transform.position;
            Random.InitState(seed);
            SpawnPos.y++;
            PlayerController = PhotonNetwork.Instantiate("UFPS_Player", SpawnPos, Quaternion.identity).transform;
            PlayerInput = PlayerController.GetComponent<vp_FPInput>();
        }

        [PunRPC]
        private void StartGame()
        {
            StartCoroutine(StartIn());
        }

        /// <summary>
        /// Plays win effect when a player wins.
        /// </summary>
        public void Win()
        {
            StartCoroutine(StartVictory());
        }

        private IEnumerator StartVictory()
        {
            Game.Instance.IsActive = false;
            VictoryTheme.Play();
            VictoryMusic.Play();
            Time.timeScale = 0.2f;
            UI.WinEffect();
            yield return new WaitForSecondsRealtime(3);
            Time.timeScale = 1f;

            yield return new WaitForSecondsRealtime(4f);
            UI.Overlay.Play("ShowOverlay");
            yield return new WaitForSecondsRealtime(2);

            PhotonNetwork.Destroy(PlayerController.gameObject);
            UI.Data.SetActive(false);
            UI.WinLogo.gameObject.SetActive(false);
            Postgame.Victory();
            UI.Overlay.Play("HideOverlay");
            UI.UI_Postgame.SetActive(true);
            UI.SetPostgame();
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
            this.photonView.RPC(nameof(PlaceLoot), RpcTarget.All, (object)items.ToArray(), (object)nodes.ToArray());

            PhotonNetwork.CurrentRoom.IsOpen = false;
            this.photonView.RPC(nameof(StartGame), RpcTarget.All);
        }

        private IEnumerator StartIn()
        {
            Camera.GetComponent<Animator>().Play("CameraStart");
            yield return new WaitForSeconds(3.3f);
            Club.StartAudio();
            Summon();
            UI.gameObject.SetActive(true);
            Button_Start.gameObject.SetActive(false);
            AudioSync.RestartAll();
            Game.Instance.StartGame();
            Instantiate(new GameObject()).AddComponent<GracePeriod>();
        }

        /// <summary>
        /// Creates a list with all loot and saves it locally.
        /// </summary>
        public void MCreateLoot()
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

        /// <summary>
        /// Teleports local player to a room
        /// </summary>
        /// <param name="id">Player to teleport</param>
        /// <param name="room">Room name to teleport to</param>
        /// <returns>Whether the teleportation was successful</returns>
        [PunRPC]
        public bool TeleportTo(int id, string room)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != id) return false;
            if (RoomNode.Get(room.ToLower()) != null)
            {
                PlayerController.position = RoomNode.Get(room.ToLower()).transform.position;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a player object by actor number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Transform GetPlayer(int actornr)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.GetComponent<PhotonView>())
                {
                    if (g.GetComponent<PhotonView>().ControllerActorNr == actornr) return g.transform;
                }
            }
            return null;
        }

        #endregion Ran by master client only
    }
}