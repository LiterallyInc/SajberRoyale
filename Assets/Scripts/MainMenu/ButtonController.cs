using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private void Update()
    {
        GameObject.Find("Canvas/Text").GetComponent<Text>().text = DiscordController.Instance.presence.joinSecret;
    }
    public void ConnectToServer()
    {

    }

    public void JoinRoom()
    {
        SceneManager.LoadScene("game");
    }
    public void SetSecret(string s)
    {
        DiscordController.joinSecret = s;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
