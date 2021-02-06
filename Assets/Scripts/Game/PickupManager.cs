using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

/// <summary>
/// Controls player interactions like picking up items, toggling UI etc
/// </summary>
public class PickupManager : MonoBehaviourPun
{
    private void Update()
    {
        //pickup item
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.CompareTag("ItemNode") && Input.GetKeyDown(KeyCode.F))
        {
            ItemNode node = hit.transform.gameObject.GetComponent<ItemNode>();
            Item i = node.item;
            Core.Instance.photonView.RPC("DestroyNode", RpcTarget.All, (double)hit.transform.position.x * (double)hit.transform.position.y * (double)hit.transform.position.z);
            GetComponent<Inventory>().TakeItem(i);
        }

        //toggle UI
        if (Input.GetKeyDown(KeyCode.F1) && !Core.Instance.H532.isCursed) Core.Instance.UI_Data.SetActive(!Core.Instance.UI_Data.activeSelf);

        //take screenshot
        if (Input.GetKeyDown(KeyCode.F2) && !Application.isEditor)
        {
            var src = DateTime.Now;
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Screenshots"))) Directory.CreateDirectory(Path.Combine(Application.dataPath, "Screenshots"));
            string filename = $"Screenshots/SajberRoyale {src.Year}.{src.Month}.{src.Day} - {src.Hour}.{src.Minute}.{src.Second}.png";
            ScreenCapture.CaptureScreenshot(filename);
        }
    }
}

