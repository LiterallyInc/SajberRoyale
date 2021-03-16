using Photon.Pun;
using SajberRoyale.Game;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Map
{
    public class RoomNode : MonoBehaviour
    {
        public string roomName;
        public bool isActivated = true;
        public Light[] Lights;
        public bool allowMusic = true;
        private string id; //gameobject name //update: yes i know that it's a built in variable

        private void Start()
        {
            id = name.ToLower();
            Destroy(GetComponent<MeshRenderer>());
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!c.GetComponent<PhotonView>()) return;
            else if (c.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log($"Entered room {roomName}");
                Game.Game.Instance.CurrentRoom = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //Debug.Log($"Exited room {roomName}");
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