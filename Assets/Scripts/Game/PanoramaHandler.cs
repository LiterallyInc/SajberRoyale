using Photon.Pun;
using UnityEngine;
namespace SajberRoyale.Map
{
    public class PanoramaHandler : MonoBehaviour
    {
        public GameObject[] disableInZone;
        public GameObject[] enableInZone;

        public GameObject[] disableOnExit;
        public GameObject[] enableOnExit;



        private void OnTriggerEnter(Collider c)
        {
            if (!c.GetComponent<PhotonView>()) return;
            if (!c.GetComponent<PhotonView>().IsMine) return;

            foreach (GameObject g in disableInZone) Toggle(g, false, true);
            foreach (GameObject g in enableInZone) Toggle(g, true, true);
        }

        private void OnTriggerExit(Collider c)
        {
            if (!c.GetComponent<PhotonView>()) return;
            if (!c.GetComponent<PhotonView>().IsMine) return;

            foreach (GameObject g in disableOnExit) Toggle(g, false, false);
            foreach (GameObject g in enableOnExit) Toggle(g, true, false);
        }

        private void Toggle(GameObject g, bool enable, bool enter)
        {
            g.SetActive(enable);
            Debug.Log($"{name} {(enable ? "enabled" : "disabled")} {g.name} on {(enter ? "enter" : "exit")}");
        }
    }
}