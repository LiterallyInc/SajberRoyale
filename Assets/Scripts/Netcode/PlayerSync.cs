using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviourPun
{
    public GameObject CameraController;
    public EasySurvivalScripts.PlayerMovement PlayerMovement;

    public GameObject[] Meshes;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected) return;
        Random.InitState(Core.seed * photonView.OwnerActorNr);
        GameObject newMesh = Instantiate(Meshes[Random.Range(0, Meshes.Length)], gameObject.transform);
        if (!photonView.IsMine)
        {
            Destroy(CameraController);
            Destroy(PlayerMovement);
        }
        else
        {
            PlayerMovement.CharacterAnimator = newMesh.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.K))
        {
            GameObject newMesh = Instantiate(Meshes[Random.Range(0, Meshes.Length)], gameObject.transform);
            PlayerMovement.CharacterAnimator = newMesh.GetComponent<Animator>();
        }
    }
}