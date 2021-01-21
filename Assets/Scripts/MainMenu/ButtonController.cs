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
        B_Connect.interactable = IF_Name.text.Length >= 3 && IF_Server.text.Length > 0;
    }
    public void ToggleConnectionGUI(bool open)
    {
        ConnectionGUI.SetActive(open);
    }
    public void ConnectToServer()
    {

    }

    public void JoinRoom()
    {
        SceneManager.LoadScene("main_lts");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
