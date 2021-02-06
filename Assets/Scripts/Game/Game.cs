using Photon.Pun;
using System;

public class Game
{
    public static Game Instance;
    public bool IsActive = false;
    public int TotalPlayers = 0;
    public int AlivePlayers = 0;
    public long StartEpoch = 0;

    /// <summary> Last physical in-game room player entered  </summary>
    public string CurrentRoom = "Unknown";

    public string Skin = "Unknown";
    public int Kills;
    public bool IsTournament = false;

    /// <summary>
    /// Resets game core to default values
    /// </summary>
    public static void ResetGame()
    {
        Instance = new Game();
    }

    public void StartGame()
    {
        IsActive = true;
        StartEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();
        TotalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        AlivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }
}