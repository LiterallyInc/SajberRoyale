using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public float speed = 10;
    [Obsolete] public TextMesh Name;
    private Text Canvas;
    public string playername;
    public Camera cam;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    // Start is called before the first frame update
    void Start()
    {
        Name.text = playername = PhotonNetwork.NickName;
        GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV();
        Canvas = GameObject.Find("Canvas/Panel/UIInfo").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    { 
        //Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 newPosition = new Vector3(horizontalInput, 0.0f, verticalInput);
        transform.Translate(newPosition * speed * Time.deltaTime, Space.World);

        //If player falls
        if (transform.position.y < -1) transform.position = Vector3.one;

        if (Input.GetKeyDown(KeyCode.X)) GameManager.Instance.LeaveRoom();

        //UI
        Canvas.text = $"Players connected: {PhotonNetwork.CurrentRoom.PlayerCount}\n" +
            $"Your name: {PhotonNetwork.NickName}\n\n" +
            $"X: {Mathf.RoundToInt(transform.position.x)}   Z: {Mathf.RoundToInt(transform.position.z)}\n\n" +
            $"Press <b>X</b> to leave room";
    }
    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        else
        {
            Destroy(cam); //keep own camera only
            Destroy(GetComponent<CameraWork>());
        }
    }
    public static void RefreshInstance(ref PlayerController player, PlayerController Prefab)
    {
        var position = Vector3.zero;
        var rotation = Quaternion.identity;
        if(player != null)
        {
            position = player.transform.position;
            rotation = player.transform.rotation;
            PhotonNetwork.Destroy(player.gameObject);
        }
        player = PhotonNetwork.Instantiate(Prefab.gameObject.name, position, rotation).GetComponent<PlayerController>();
    }
    //Sync
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data
            stream.SendNext(playername);
        }
        else
        {
            // Receive data
            this.playername = (string)stream.ReceiveNext();
        }
    }
}
