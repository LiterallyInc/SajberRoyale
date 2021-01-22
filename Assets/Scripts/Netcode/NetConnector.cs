using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetConnector : MonoBehaviourPunCallbacks
{
    private string srv;
    private static bool isConnected = false;
    public Text Status; 

    public void Connect(string name, string server)
    {
        srv = server;
        PhotonNetwork.NickName = name;
        PhotonNetwork.GameVersion = Application.version;
        if (!isConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log($"SRNet: Connecting to server..");
        }
        else SetRoom();
    }

    public override void OnConnectedToMaster()
    {
        isConnected = true;
        Debug.Log($"SRNet: Connected to server. Region: {PhotonNetwork.ServerAddress}");
        SetRoom();
    }

    public void SetRoom()
    {
        if (srv[0] == '@') PhotonNetwork.CreateRoom(srv.Substring(1).Trim());
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
        SceneManager.LoadScene("game");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        Status.text = $"Room {PhotonNetwork.CloudRegion}, Error {returnCode}: {message}";
        base.OnJoinRoomFailed(returnCode, message);
    }
}