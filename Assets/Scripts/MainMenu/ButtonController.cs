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
    public AudioSource ClickSound;
    
    private void Start()
    {
        IF_Name.text = PlayerPrefs.GetString("username", "");
        if (Application.isEditor) IF_Server.text = "@dev";
    }
    private void Update()
    {
        B_Connect.interactable = IF_Name.text.Trim().Length >= 3 && IF_Server.text.Trim().Length > 0;
    }
    public void ToggleConnectionGUI(bool open)
    {
        ConnectionGUI.SetActive(open);
    }
    public void Sandbox()
    {
        GetComponent<NetConnector>().PlayOffline();
    }
    public void ConnectToServer()
    {
        PlayerPrefs.SetString("username", IF_Name.text);
        GetComponent<NetConnector>().Connect(IF_Name.text, IF_Server.text);
    }
    public void Click()
    {
        ClickSound.Play();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
