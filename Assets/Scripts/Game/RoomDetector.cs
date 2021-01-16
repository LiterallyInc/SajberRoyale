using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDetector : MonoBehaviour
{
    public string roomName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered room {roomName}");
        GameObject.Find("Canvas: Player UI/RoomInfo").GetComponent<Text>().text = $"Current room: {roomName}";
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Exited room {roomName}");
    }
}
