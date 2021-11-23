using UnityEngine;

public class InvisibleTeleport : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}