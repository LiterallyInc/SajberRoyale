using Photon.Pun;
using UnityEngine;

public class RemoveAnimator : MonoBehaviourPun
{

    private void Start()
    {
        if (!photonView.IsMine)
            Destroy(GetComponent<vp_FPBodyAnimator>());
    }
}