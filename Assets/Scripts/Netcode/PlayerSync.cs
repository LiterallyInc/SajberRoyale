using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSync : MonoBehaviourPun
{
    public GameObject CameraController;
    public EasySurvivalScripts.PlayerMovement PlayerMovement;
    private void Start()
    {
        if (!photonView.IsMine)
        {
                Destroy(CameraController);
                Destroy(PlayerMovement);
        }
    }
}
