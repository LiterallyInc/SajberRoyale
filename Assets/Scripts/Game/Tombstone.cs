using Photon.Pun;
using UnityEngine;

namespace SajberRoyale.Game
{
    public class Tombstone : MonoBehaviour
    {
        public TextMesh Text;

        private void Start()
        {
            Text.text = PhotonNetwork.NickName;
        }
    }
}