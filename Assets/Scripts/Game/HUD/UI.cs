using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class UI : MonoBehaviour
    {
        public Text GameStats;
        public Text TechStats;
        public Transform HPOverlay;
        public Text HPText;

        private void Start()
        {
            InvokeRepeating("UpdateStats", 0, 0.1f);
        }

        private void Update()
        {
            HPText.text = $"{(Game.Instance.HP >= 0 ? Game.Instance.HP : 0)}/100";
            HPOverlay.transform.localScale = new Vector3(Game.Instance.HP >= 0 ? (float)Game.Instance.HP / 100f : 0, 1, 1);
        }

        private void UpdateStats()
        {
            GameStats.text = $@"{(Helper.IsDev ? "<color=#FFB4C0>[DEV]</color> " : "")}{PhotonNetwork.NickName}
<size=10><i>Playing as {Game.Instance.Skin}</i></size>

Room: {Game.Instance.CurrentRoom}
Kills: {Game.Instance.Kills}
Alive: {Game.Instance.AlivePlayers}/{Game.Instance.TotalPlayers}
";
            string ping = $"{PhotonNetwork.GetPing()} ms";
            if (PhotonNetwork.OfflineMode) ping = "Offline";
            TechStats.text = $"{Mathf.Round(1.0f / Time.deltaTime)} fps\n{ping}";
        }
    }
}