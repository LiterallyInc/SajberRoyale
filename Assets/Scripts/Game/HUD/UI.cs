using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class UI : MonoBehaviour
    {
        public GameObject UI_Pregame;
        public GameObject Data;
        public Text GameStats;
        public Text TechStats;
        public Transform HPOverlay;
        public Text HPText;
        public Text StatusText;
        public Image FillArea;
        public float FillPercentage;

        public Animator WinLogo;

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
            HPText.text = $"{(Game.Instance.HP >= 0 ? Game.Instance.HP : 0)}/100";
            HPOverlay.transform.localScale = new Vector3(Game.Instance.HP >= 0 ? (float)Game.Instance.HP / 100f : 0, 1, 1);

            if (FillPercentage != -1) FillArea.fillAmount = FillPercentage;
            else FillArea.fillAmount = 0;
        }

        private void UpdateStats()
        {
            GameStats.text = $@"{(Helper.IsDev ? "<color=#FFB4C0>[DEV]</color> " : "")}{(Game.Instance.IsAlive ? "" : "<color=#FD6F77>[DEAD]</color> ")}{PhotonNetwork.NickName}
<size=10><i>Playing as {Game.Instance.Skin}</i></size>

Room: {Game.Instance.CurrentRoom}
Elims: {Game.Instance.Kills}
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
        
    }
}