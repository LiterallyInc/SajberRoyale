using UnityEngine;
using UnityEngine.UI;

public class RoomNode : MonoBehaviour
{
    public string roomName;
    public bool isActivated = true;

    private void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
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

    public static RoomNode Get(string name)
    {
        RoomNode[] nodes = FindObjectsOfType<RoomNode>();
        foreach (RoomNode node in nodes)
        {
            if (node.roomName.ToLower() == name.ToLower()) return node;
        }
        return null;
    }

    public void Deactivate()
    {
        isActivated = false;
        Collider[] colliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        foreach (Collider c in colliders)
        {
            Debug.Log(c.name);
            LampContainer lampContainer = c.GetComponent<LampContainer>();
            if (lampContainer != null) lampContainer.TurnOff(roomName);
        }
    }
}