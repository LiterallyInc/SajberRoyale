using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public PlayerController PlayerPrefab;
    [HideInInspector] public PlayerController LocalPlayer;
    public static GameManager Instance;
    void Start()
    {
        Instance = this;
        PlayerController.RefreshInstance(ref LocalPlayer, PlayerPrefab);
    }
    public void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
            return;
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        PlayerController.RefreshInstance(ref LocalPlayer, PlayerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
