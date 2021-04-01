using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class UI : MonoBehaviour
    {
        public GameObject UI_Pregame;
        public GameObject Data;
        public GameObject UI_Postgame;
        public Text GameStats;
        public Text TechStats;
        public Transform HPOverlay;
        public Text HPText;
        public Text StatusText;
        public Text Ammo;
        public Animator FillArea;
        public Text Tooltip;

        public Animator WinLogo;
        public Animator Crosshair;
        public Animator Overlay;

        public Text PostgamePlacement;
        public Text PostgameStats;

        private void Start()
        {
            InvokeRepeating("UpdateStats", 0, 0.1f);
            StatusText.text = "";
        }

        public void ShowData(bool show)
        {
            Data.SetActive(show);
        }

        private void Update()
        {
            HPText.text = $"{(Game.Instance.HP >= 0 ? Game.Instance.HP : 0)}/{Game.Instance.MaxHP}";
            HPOverlay.transform.localScale = new Vector3(Game.Instance.HP >= 0 ? (float)Game.Instance.HP / Game.Instance.MaxHP : 0, 1, 1);

        }

        private void UpdateStats()
        {
            GameStats.text = $@"{(Helper.IsDev ? "<color=#FFB4C0>[DEV]</color> " : "")}{(Game.Instance.IsAlive ? "" : "<color=#FD6F77>[DEAD]</color> ")}{PhotonNetwork.NickName}
<size=10><i>Playing as {Game.Instance.Skin}</i></size>

Room: {Game.Instance.CurrentRoom.roomName}
Elims: {Game.Instance.Stats.Eliminations}
Left: {Game.Instance.AlivePlayers}/{Game.Instance.TotalPlayers}
";
            string ping = $"{PhotonNetwork.GetPing()} ms";
            if (PhotonNetwork.OfflineMode) ping = "Offline";
            TechStats.text = $"{Mathf.Round(1.0f / Time.deltaTime)} fps\n{ping}";
        }

        public void WinEffect()
        {
            WinLogo.gameObject.SetActive(true);
            WinLogo.PlayInFixedTime("VictoryAnimation");
        }

        public void SetPostgame()
        {
            Stats stats = Game.Instance.Stats;
            DateTime diff = DateTimeOffset.FromUnixTimeSeconds(stats.StartEpoch).DateTime;
            PostgamePlacement.text = $"#{stats.Placement}";
            PostgameStats.text = $"{DateTime.Now.Subtract(diff):mm\\:ss}\n\n{stats.Eliminations}\n\n{stats.ShotsFired}\n\n{stats.ShotsHit} ({((stats.ShotsFired > 0) ? Math.Round(stats.ShotsHit / (double)stats.ShotsFired * 100, 2) : 0)}%)\n\n{stats.HPRegen}\n\n{stats.Emotes}";
        }
    }
}