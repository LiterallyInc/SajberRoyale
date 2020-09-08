using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class NetworkAdmin : MonoBehaviour
{
    public Canvas AdminCanvas;
    public bool AdminMenuOpen = false;
    public InputField IF_Password;
    public InputField IF_Roomcode;

    readonly string hash = "A169577AB6A460D28C9BBF3ACDD5C653E153BBA872106E79B49EBC251C615CD0C7C0D3213A84A0FD2EBA945FAC161A3743BDFE4DF9297EBC392504CB6C6A0EAE";
    private void Start()
    {
        Object.DontDestroyOnLoad(AdminCanvas.gameObject);
    }
    private void Update()
    {
        //toggle menu
        if (Input.GetKeyDown(KeyCode.F1))
        {
            transform.localScale = AdminMenuOpen ? Vector3.zero : Vector3.one;
            AdminMenuOpen = !AdminMenuOpen;
        }
        if (IF_Password.text == "" && !Application.isEditor) return;
        string hashedpass = IF_Password.text.SHA512();

        //allow all fields if logged in or in editor
        bool correct = (hashedpass == hash || Application.isEditor) && PhotonNetwork.IsConnected;
        IF_Roomcode.interactable = correct;

        //no need to edit pass once correct 
        if (Application.isEditor) 
            IF_Password.interactable = false;
        else
            IF_Password.interactable = hashedpass != hash;
    }
    //called by UI
    public void CreateRoom(string name)
    {
        PhotonNetwork.CreateRoom(name);
        Webhook.Send($"**{PhotonNetwork.NickName}** created a room called \"**{name}**\".");
    }
}
