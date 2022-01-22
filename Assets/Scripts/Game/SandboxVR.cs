using SajberRoyale.Map;
using UnityEngine;

public class SandboxVR : MonoBehaviour
{
    private void Start()
    {
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("ItemNode"))
        {
            if (!node.GetComponent<Locker>())
            {
                Destroy(node);
            }
            else
            {
                Destroy(node.GetComponent<Locker>().light);
                Destroy(node.GetComponent<Locker>().particles);
            }
        }

        GameObject[] SpawnNodes = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        Vector3 SpawnPos = SpawnNodes[Random.Range(0, SpawnNodes.Length - 1)].transform.position;

        FindObjectOfType<Valve.VR.InteractionSystem.Player>().transform.position = SpawnPos;
    }
}