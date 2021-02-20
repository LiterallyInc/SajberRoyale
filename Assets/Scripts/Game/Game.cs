using Photon.Pun;
using System;

namespace SajberRoyale.Game
{
    public class Game
    {
        public static Game Instance;
        public bool IsActive = false;
        public int TotalPlayers = 0;
        public int AlivePlayers = 0;
        public long StartEpoch = 0;

        //Player data
        public int Kills = 0;
        public int HP = 100;
        public bool canShoot = true;
        /// <summary> Last physical in-game room player entered  </summary>
        public string CurrentRoom = "Unknown";

        public string Skin = "Unknown";
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
}