using Photon.Pun;
using SajberRoyale.Map;
using System;
using System.Collections;

namespace SajberRoyale.Game
{
    public class Game
    {
        public static Game Instance;
        public bool IsActive = false;
        public bool GracePeriod = true;
        public int TotalPlayers = 0;
        public int AlivePlayers = 0;

        //Player data
        public Stats Stats = new Stats();
        public int HP = 200;
        public int MaxHP;
        public bool canShoot = true;
        public bool IsAlive = true;
        /// <summary> Last physical in-game room player entered  </summary>
        public RoomNode CurrentRoom;

        public string Skin = "Unknown";
        public bool IsTournament = false;

        /// <summary>
        /// Resets game core to default values
        /// </summary>
        public static void ResetGame()
        {
            Instance = new Game();
            Instance.MaxHP = Instance.HP;
        }

        public void StartGame()
        {
            IsActive = true;
            Stats.StartEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();
            TotalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            AlivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }
}