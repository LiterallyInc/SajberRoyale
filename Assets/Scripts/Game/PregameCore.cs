using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class PregameCore : MonoBehaviourPun
    {
        public Vector3[] PortraitPositions;
        public Text Room;

        // Start is called before the first frame update
        private void Start()
        {
            Room.text = $"Room: <b>{PhotonNetwork.CurrentRoom.Name}</b>";
            if (PhotonNetwork.OfflineMode) Room.text = "OFFLINE";


            if (PhotonNetwork.LocalPlayer.ActorNumber == -1) return;
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                SetPort(p.ActorNumber, p.NickName);
            }

            photonView.RPC("SetPort", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName);
        }

        [PunRPC]
        private void SetPort(int id, string name)
        {
            GameObject port = Instantiate(Resources.Load("Prefabs/UI/Portrait") as GameObject, gameObject.transform, false);
            port.transform.localPosition = PortraitPositions[id - 1];
            port.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Random.InitState(Core.Instance.seed * id);
            string skin = Core.Instance.Meshes[Random.Range(0, Core.Instance.Meshes.Length)];
            Core.Instance.PlayerSkins.Add(skin);
            if (Game.Instance.IsTournament) skin = "GigaChad";
            if (id == PhotonNetwork.LocalPlayer.ActorNumber) Game.Instance.Skin = skin;
            port.GetComponent<PortraitContainer>().Username.text = name;
            port.GetComponent<PortraitContainer>().Portrait.sprite = Resources.Load<Sprite>($"CharPortraits/{skin}");
            if (id % 2 == 0) port.GetComponent<PortraitContainer>().Frame.sprite = port.GetComponent<PortraitContainer>().FrameAlt;
            if (id % 2 == 0) port.GetComponent<PortraitContainer>().Username.color = new Color(0.1529412f, 0.8666667f, 0.5968716f);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Game.Instance.IsTournament) Room.text = "[TOURNAMENT]";
        }
    }
}