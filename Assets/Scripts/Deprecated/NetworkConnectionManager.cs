using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks
{
    public string[] mainscenes = { "main", "main_lts" };
    public Button ConnectToServer;
    public Button JoinGame;

    public InputField Nickname;
    public InputField Roomname;

    public Text Status;
    public bool TriesToConnect = false;
    // Start is called before the first frame update
    void Start()
    {
        if (Array.IndexOf(mainscenes, SceneManager.GetActiveScene().name) >= 0) Nickname.text = PlayerPrefs.GetString("username", "");
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Array.IndexOf(mainscenes, SceneManager.GetActiveScene().name) >= 0)
        {
            ConnectToServer.gameObject.SetActive(!PhotonNetwork.IsConnected);
            JoinGame.gameObject.SetActive(PhotonNetwork.IsConnected && !TriesToConnect);
            Nickname.gameObject.SetActive(!PhotonNetwork.IsConnected);
            Roomname.gameObject.SetActive(PhotonNetwork.IsConnected && !TriesToConnect);
            ConnectToServer.interactable = Nickname.text != "";
            JoinGame.interactable = Roomname.text != "";
        }
       
    }
    public void TryConnectToGame()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.NickName = Nickname.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PlayerPrefs.SetString("username", Nickname.text);

        TriesToConnect = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        TriesToConnect = false;
        Debug.LogError(cause);
        Status.text = cause.ToString();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        TriesToConnect = false;
        Debug.Log("Connected to server.");
        Status.text = "Connected to game server!";
    }
    public void TryConnectToRoom()
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.JoinRoom(Roomname.text);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"Master: {PhotonNetwork.MasterClient}, playing with {PhotonNetwork.CurrentRoom.PlayerCount} players.");
        SceneManager.LoadScene(2);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Status.text = $"\"{Roomname.name}\" is not a valid room :(";
    }
}
