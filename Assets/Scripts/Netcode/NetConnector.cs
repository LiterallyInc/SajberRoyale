using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    [System.Obsolete]
    public class NetConnector : MonoBehaviourPunCallbacks
    {
        private string srv;
        private static bool isConnected = false;
        public Text Status;

        private void Start()
        {
            Game.Game.ResetGame();
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
    }
}