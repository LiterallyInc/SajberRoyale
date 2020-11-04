using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void ConnectToServer()
    {

    }

    public void JoinRoom()
    {
        SceneManager.LoadScene("game");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
