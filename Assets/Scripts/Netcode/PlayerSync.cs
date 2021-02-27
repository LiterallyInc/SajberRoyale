using Photon.Pun;
using SajberRoyale.Game;
using UnityEngine;

namespace SajberRoyale.Player
{
    public class PlayerSync : MonoBehaviourPun
    {
        public static PlayerSync Me;
        public GameObject PlayerCam;
        public Component[] LocalScripts;
        private GameObject Player;
        public GameObject PublicHolder;
        public GameObject PubliclyHeld;
        public GameObject LocalHolder;
        public GameObject LocallyHeld;

        public ParticleSystem DeathParticles;

        private void Start()
        {
            if (!PhotonNetwork.IsConnected) return;

            //delete local comps if it's another user
            if (!photonView.IsMine)
            {
                Destroy(PlayerCam);
                foreach (Component c in LocalScripts) Destroy(c);
            }
            else //else instansiate the avatar and set the animator
            {
                Me = this;
                Player = PhotonNetwork.Instantiate($"CharMeshes/{Game.Game.Instance.Skin}", Vector3.zero, Quaternion.identity);
                Core.Instance.Player = Player.transform;
                Core.Instance.Sync = this;
                //PlayerMovement.CharacterAnimator = Player.GetComponent<Animator>();

                LocalHolder = Instantiate(new GameObject());
                LocalHolder.transform.parent = Camera.main.transform;
                LocalHolder.transform.localPosition = new Vector3(0.24f, -0.35f, .16f);
                LocalHolder.transform.localRotation = Quaternion.Euler(3f, -90f, 8f);
            }
            //place the other avatars
            GetComponent<CharacterController>().radius = 0.27f;
            InvokeRepeating(nameof(SetSkin), 0f, 1f);
        }

        private void SetSkin()
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Avatar"))
            {
                if (g.GetComponent<PhotonView>().ControllerActorNr == photonView.ControllerActorNr)
                {
                    gameObject.name = $"Player{photonView.ControllerActorNr}";
                    g.transform.parent = gameObject.transform;
                    g.transform.localPosition = Vector3.zero;
                    g.transform.localRotation = Quaternion.identity;
                    g.tag = "Untagged";
                    if (g.GetComponent<PhotonView>().ControllerActorNr != PhotonNetwork.LocalPlayer.ActorNumber && g.GetComponent<vp_FPBodyAnimator>()) Destroy(g.GetComponent<vp_FPBodyAnimator>());

                    //set itemholder at hand
                    PublicHolder = Instantiate(new GameObject());
                    PublicHolder.name = "Holder";
                    PublicHolder.transform.parent = g.GetComponent<vp_FPBodyAnimator>().Hand.transform;
                    if(photonView.ControllerActorNr != PhotonNetwork.LocalPlayer.ActorNumber) Destroy(g.GetComponent<vp_FPBodyAnimator>());
                    PublicHolder.transform.localPosition = Vector3.zero;
                    PublicHolder.transform.localRotation = Quaternion.Euler(-181f, 4f, -14f);
                }
            }
        }
    }
}