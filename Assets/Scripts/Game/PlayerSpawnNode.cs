using UnityEngine;

namespace SajberRoyale.Map
{
    public class PlayerSpawnNode : MonoBehaviour
    {
        private void Start()
        {
            Destroy(GetComponent<MeshRenderer>());
        }
    }
}