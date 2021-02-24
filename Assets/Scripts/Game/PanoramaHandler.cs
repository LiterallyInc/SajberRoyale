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

            foreach (GameObject g in disableInZone) g.SetActive(false);
            foreach (GameObject g in enableInZone) g.SetActive(true);
        }

        private void OnTriggerExit(Collider c)
        {
            if (!c.GetComponent<PhotonView>()) return;
            if (!c.GetComponent<PhotonView>().IsMine) return;

            foreach (GameObject g in disableOnExit) g.SetActive(false);
            foreach (GameObject g in enableOnExit) g.SetActive(true);
        }
    }
}