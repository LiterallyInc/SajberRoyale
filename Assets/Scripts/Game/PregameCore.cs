using Photon.Pun;
using SajberRoyale.Map;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class PregameCore : MonoBehaviourPun
    {
        public Vector3[] PortraitPositions;
        public Text Room;
        public Text Tip;

        private void Start()
        {
            SetTip();
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
            if (Input.GetKeyDown(KeyCode.N)) SetTip();
        }
        private void SetTip()
        {
            //List is created in realtime instead of compile to contain realtime data
            string[] Tips =
               {
                "Press B to emote in-game.",
                "You can do specific dances with 8, 9 and 0.",
                "Turning off your flashlight with F makes you harder to spot.",
                "Nothing happened in H532. Everything you have heard is a lie.",
                "Make sure to pick up some of Björns cola if you need to heal up!",
                "The D.va blaster isn't as bad as you might think.\n<size=12>Actually, it probably is.</size>",
                "Pay a visit to The Weeb Empire on floor 5 if you get the chance!",
                "Apparently there is a lot of abandoned stuff backstage.",
                "There are lots of great hiding spots as long as you turn off your flashlight (with F). Be creative!",
                "Make sure to be logged in to save your stats!",
                "You can turn off the in-game music in the settings. Not sure why anyone would do that though.",
                "Fabina is real.",
                "V-bucks is coming later in a later update. Sorry about the inconvenience.",
                "Welcome to SajberRoyale, created by LiterallyInc.!",
                "Thank you Selou for the art <3",
                "You hopefully already figured it out, but press E to pick up items.",
                "Reloading is free, don't forget to keep your weapons reloaded!",
                $"There are currently {GameObject.FindGameObjectsWithTag("Room").Length} rooms to explore.",
                $"There are currently {FindObjectsOfType<Locker>().Length} lockers. Maybe someone forgot an item?",
            };

            System.Random rnd = new System.Random();
            Tip.text = Tips[rnd.Next(Tips.Length)];
        }
    }
}