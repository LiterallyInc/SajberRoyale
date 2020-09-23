using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNet : MonoBehaviourPun
{
    private void Awake()
    {
        if (!photonView.IsMine && GetComponent<PlayerController>() != null)
            Destroy(GetComponent<PlayerController>());
    }
}
