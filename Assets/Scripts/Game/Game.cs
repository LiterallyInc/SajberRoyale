using Photon.Pun;
using System;

public class Game : MonoBehaviourPun
{
    public static bool IsActive = false;
    public static int TotalPlayers = 0;
    public static int AlivePlayers = 0;
    public static long StartEpoch = 0;
    public static string CurrentRoom = "Unknown";
    public static string Skin = "Unknown";
    public static int Kills;

    public static void StartGame()
    {
        IsActive = true;
        StartEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();
        TotalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        AlivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }
}