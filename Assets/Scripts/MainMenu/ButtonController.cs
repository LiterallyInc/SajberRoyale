using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject ConnectionGUI;
    public InputField IF_Name;
    public InputField IF_Server;
    public Button B_Connect;

    private void Update()
    {
        B_Connect.interactable = IF_Name.text.Trim().Length >= 3 && IF_Server.text.Trim().Length > 0;
    }
    public void ToggleConnectionGUI(bool open)
    {
        ConnectionGUI.SetActive(open);
    }
    public void ConnectToServer()
    {
        GetComponent<NetConnector>().Connect(IF_Name.text, IF_Server.text);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
