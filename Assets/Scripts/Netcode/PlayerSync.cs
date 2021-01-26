using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviourPun
{
    public GameObject CameraController;
    public EasySurvivalScripts.PlayerMovement PlayerMovement;
    private GameObject Player;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected) return;
        

        //delete local comps if it's another user
        if (!photonView.IsMine)
        {
            Destroy(CameraController);
            Destroy(PlayerMovement);
        }
        else //else instansiate the avatar and set the animator
        {
            Player = PhotonNetwork.Instantiate($"CharMeshes/{Game.Skin}", Vector3.zero, Quaternion.identity);
            PlayerMovement.CharacterAnimator = Player.GetComponent<Animator>();
            if (PhotonNetwork.OfflineMode) PhotonNetwork.NickName = Game.Skin;
        }
        //place the other avatars
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Avatar"))
        {
            if(g.GetComponent<PhotonView>().ControllerActorNr == photonView.ControllerActorNr)
            {
                g.transform.parent = gameObject.transform;
                g.transform.localPosition = new Vector3(-0.01f, -1.16f, -0.1f);
            }
        }
        
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.K))
        {
            Player.GetComponent<PhotonView>().ObservedComponents.Add(Player.GetComponent<PhotonAnimatorView>());
            //GameObject newMesh = Instantiate(Meshes[Random.Range(0, Meshes.Length)], gameObject.transform);
            //PlayerMovement.CharacterAnimator = newMesh.GetComponent<Animator>();
        }
    }
}