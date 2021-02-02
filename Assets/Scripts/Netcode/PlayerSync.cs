using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviourPun
{
    public GameObject PlayerCam;
    public Component[] LocalScripts;
    private GameObject Player;

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
            Player = PhotonNetwork.Instantiate($"CharMeshes/{Game.Skin}", Vector3.zero, Quaternion.identity);
            Core.Instance.Player = Player.transform;
            //PlayerMovement.CharacterAnimator = Player.GetComponent<Animator>();
            if (PhotonNetwork.OfflineMode) PhotonNetwork.NickName = Game.Skin;
        }
        //place the other avatars

        InvokeRepeating("SetSkin", 0f, 1f);
        Destroy(this, 10f);
    }

    private void SetSkin()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Avatar"))
        {
            if (g.GetComponent<PhotonView>().ControllerActorNr == photonView.ControllerActorNr)
            {
                g.transform.parent = gameObject.transform;
                g.transform.localPosition = Vector3.zero;
                g.tag = "Untagged";
                if (g.GetComponent<PhotonView>().ControllerActorNr != PhotonNetwork.LocalPlayer.ActorNumber && g.GetComponent<vp_FPBodyAnimator>()) Destroy(g.GetComponent<vp_FPBodyAnimator>());
            }
        }
    }
}