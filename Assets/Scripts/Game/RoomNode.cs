using Photon.Pun;
using SajberRoyale.Game;
using UnityEngine;

namespace SajberRoyale.Map
{
    public class RoomNode : MonoBehaviour
    {
        public string roomName;
        public bool isActivated = true;
        public bool isElevator = false;
        public Light[] Lights;
        public bool allowMusic = true;
        [HideInInspector] public string id; //gameobject name //update: yes i know that it's a built in variable

        public string toolTip = ""; //tooltip that will show up when you enter this node

        private void Start()
        {
            id = name.ToLower();
            Destroy(GetComponent<MeshRenderer>());
            foreach (BoxCollider c in GetComponents<BoxCollider>()) c.isTrigger = true;
        }

        public void OnTriggerEnter(Collider c)
        {
            if (!c.GetComponent<PhotonView>()) return;
            else if (c.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log($"Entered room {roomName}");
                Game.Game.Instance.CurrentRoom = this;
                Core.Instance.UI.Tooltip.text = toolTip;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (!c.GetComponent<PhotonView>()) return;
            else if (c.GetComponent<PhotonView>().IsMine)
            {
                if (Core.Instance.UI.Tooltip.text == toolTip) Core.Instance.UI.Tooltip.text = "";
            }
        }

        public static RoomNode Get(string id)
        {
            RoomNode[] nodes = FindObjectsOfType<RoomNode>();
            foreach (RoomNode node in nodes)
            {
                if (node.id == id) return node;
            }
            return null;
        }

        public void Deactivate()
        {
            isActivated = false;
            foreach (Light l in Lights)
            {
                l.color = Color.red;
                l.intensity = 0.1f;
            }
        }
    }
}