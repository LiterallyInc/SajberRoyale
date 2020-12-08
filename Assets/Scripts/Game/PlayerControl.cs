using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerControl : MonoBehaviourPun
{
    private void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.CompareTag("ItemNode") && Input.GetKeyDown(KeyCode.F))
        {
            ItemNode node = hit.transform.gameObject.GetComponent<ItemNode>();
            Item i = node.item;
            Core.Instance.photonView.RPC("DestroyNode", RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
            GetComponent<Inventory>().TakeItem(i);
        }
    }
}

