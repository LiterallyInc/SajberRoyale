using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class NetConnector : MonoBehaviourPunCallbacks
    {
        private string srv;
        private static bool isConnected = false;
        public Text Status;

        private void Start()
        {
            Game.Game.ResetGame();
        }

        public void PlayOffline()
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.NickName = "Player";
        }

        public void Connect(string name, string server)
        {
            if (server.SHA512() == Helper.devhash) //make user developer
            {
                if (!Helper.IsDev)
                {
                    PlayerPrefs.SetInt(Helper.Settings.isDev.ToString(), 1);
                    Status.text = "Welcome, fellow LiterallyInc. member!";
                    Helper.IsDev = true;
                }
                else
                {
                    Status.text = "You are already a member of LiterallyInc.!";
                }
                return;
            }
            srv = server;
            PhotonNetwork.NickName = name;
            PhotonNetwork.GameVersion = Application.version;
            if (!isConnected)
            {
                if (PlayerPrefs.GetInt(Helper.Settings.altServer.ToString(), 0) == 1) PhotonNetwork.ConnectToRegion("ru");
                else PhotonNetwork.ConnectUsingSettings();
                Debug.Log($"SRNet: Connecting to server...");
            }
            else SetRoom();
        }

        public override void OnConnectedToMaster()
        {
            if (!PhotonNetwork.OfflineMode)
            {
                isConnected = true;
                Debug.Log($"SRNet: Connected to server. Region: {PhotonNetwork.ServerAddress}");
            }
            SetRoom();
        }

        public void SetRoom()
        {
            if (PhotonNetwork.OfflineMode) PhotonNetwork.CreateRoom($"offline{Random.Range(0, 10000)}");
            else if (srv[0] == '@') PhotonNetwork.CreateRoom(srv.Substring(1).Trim());
            else if (srv[0] == '%' && Helper.IsDev)
            {
                Game.Game.Instance.IsTournament = true;
                PhotonNetwork.CreateRoom(srv.Substring(1).Trim());
            }
            else PhotonNetwork.JoinRoom(srv.Trim());
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnected = false;
            Debug.Log($"SRNet: Disconned from server: {cause}");
            Status.text = cause.ToString();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined room {PhotonNetwork.CurrentRoom}");
            Debug.Log(PhotonNetwork.CurrentRoom.Players[1]);
            base.OnJoinedRoom();
            if (SceneManager.GetActiveScene().name != "game") SceneManager.LoadScene("game");
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
    }
}