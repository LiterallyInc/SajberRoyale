using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class MatchCreator : MonoBehaviourPunCallbacks
    {
        private string[] MatchNames = //easy-to-spell food
        {
            "apple",
            "avocado",
            "banana",
            "blueberry",
            "cherry",
            "coconut",
            "cranberry",
            "grapefruit",
            "grape",
            "kiwi",
            "lemon",
            "lime",
            "mandarin",
            "mango",
            "olive",
            "orange",
            "papaya",
            "peach",
            "pear",
            "pineapple",
            "plum",
            "raspberry",
            "strawberry",
            "watermelon",
            "bean",
            "carrot",
            "corn",
            "cucumber",
            "eggplant",
            "lettuce",
            "onion",
            "pepper",
            "potato",
            "pumpkin",
            "tomato"
        };

        private enum CurrentStatus
        {
            Random,
            JoinPrivate,
            Create
        }

        public GameObject Menu_Main;
        public GameObject Menu_Create;
        public GameObject Menu_Join;

        public InputField IF_Name;
        public InputField IF_Room;
        public Dropdown D_Type;

        public Button B_Play;
        public Button B_Sandbox;
        public Button B_Create;
        public Button B_Join;
        public Button B_Back;

        public Button JoinRoom;
        public Button CreateRoom;

        public Text Status;
        public Text Title;
        private CurrentStatus JoinStatus;

        private void Start()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = Application.version;

            Game.Game.ResetGame();
            IF_Name.text = PlayerPrefs.GetString("username", "");
            if (PhotonNetwork.NickName.Length > 0) IF_Name.text = PhotonNetwork.NickName;

            if (Helper.IsDev)
            {
                List<string> options = new List<string>();
                options.Add("Tournament");
                D_Type.AddOptions(options);
            }
        }

        private void Update()
        {
            bool entered = IF_Name.text.Trim().Length >= 3;

            B_Play.interactable = entered;
            B_Create.interactable = entered;
            B_Join.interactable = entered;
            IF_Name.interactable = !PhotonNetwork.IsConnected;
            JoinRoom.interactable = PhotonNetwork.IsConnected && IF_Room.text.Length > 2;
            CreateRoom.interactable = PhotonNetwork.IsConnected;
        }

        public void JoinRandom()
        {
            PhotonNetwork.NickName = IF_Name.text;
            JoinStatus = CurrentStatus.Random;
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void JoinSandbox()
        {
            string name = "Player";
            if (IF_Name.text.Trim().Length >= 3) name = IF_Name.text.Trim();
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.NickName = name;
            PhotonNetwork.CreateRoom($"offline{Random.Range(0, 10000)}");
        }

        public void CreateGame()
        {
            if (D_Type.value == 1) Game.Game.Instance.IsTournament = true;
            PhotonNetwork.NickName = IF_Name.text;
            PhotonNetwork.CreateRoom(MatchNames[Random.Range(0, MatchNames.Length)]);
        }

        public void JoinPrivate()
        {
            Debug.Log(IF_Room.text.SHA512());
            Debug.Log(Helper.devhash);
            if(IF_Room.text.SHA512() == Helper.devhash)
            {
                Helper.IsDev = true;
                PlayerPrefs.SetInt(Helper.Settings.isDev.ToString(), 1);
                Status.text = "Welcome to Literally Inc!";
                return;
            }
            PhotonNetwork.NickName = IF_Name.text;
            PhotonNetwork.JoinRoom(IF_Room.text.ToLower());
        }

        public void OpenCreateGUI()
        {
            JoinStatus = CurrentStatus.Create;
            if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
            Title.text = "Create private match";
            Menu_Create.SetActive(true);
            Menu_Main.SetActive(false);
            B_Back.gameObject.SetActive(true);
        }

        public void OpenJoinGUI()
        {
            JoinStatus = CurrentStatus.JoinPrivate;
            if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
            Title.text = "Join private match";
            Menu_Join.SetActive(true);
            Menu_Main.SetActive(false);
            B_Back.gameObject.SetActive(true);
        }

        public void OpenMainGUI()
        {
            Status.text = "";
            Title.text = "Connect to server";
            Menu_Main.SetActive(true);
            Menu_Create.SetActive(false);
            Menu_Join.SetActive(false);
            B_Back.gameObject.SetActive(false);
        }

        #region Photon callbacks

        public override void OnConnectedToMaster()
        {
            if (!PhotonNetwork.OfflineMode)
            {
                Debug.Log($"SRNet: Connected to server {PhotonNetwork.ServerAddress}");
            }
            switch (JoinStatus)
            {
                case CurrentStatus.Random: JoinRandom(); break;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"SRNet: Disconnected from server: {cause}");
            if(Status) Status.text = cause.ToString();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined room {PhotonNetwork.CurrentRoom}");
            base.OnJoinedRoom();
            if (SceneManager.GetActiveScene().name != "game") SceneManager.LoadScene("game");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("random fail");
            PhotonNetwork.CreateRoom(MatchNames[Random.Range(0, MatchNames.Length)]);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log(message);
            Status.text = returnCode switch
            {
                32758 => $"Error {returnCode}: This server does not exist.",
                32764 => $"Error {returnCode}: This server have already started.",
                _ => $"Error {returnCode}: {message}",
            };
            base.OnJoinRoomFailed(returnCode, message);
        }

        public static void LeaveGame()
        {
            Time.timeScale = 1;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("main");
        }

        #endregion Photon callbacks
    }
}