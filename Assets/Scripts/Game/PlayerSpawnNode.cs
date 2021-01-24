using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnNode : MonoBehaviour
{
    private void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
    }
}
