using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject ConnectionGUI;
    private void Update()
    {

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
