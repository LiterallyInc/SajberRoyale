using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI : MonoBehaviour
{
    public Text GameStats;
    public Text TechStats;
    private void Start()
    {
        InvokeRepeating("UpdateStats", 0, 0.1f);
    }
    private void UpdateStats()
    {
        GameStats.text = $@"{PhotonNetwork.NickName}
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