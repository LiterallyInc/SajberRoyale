using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedH532 : MonoBehaviourPun
{
    private bool roomCursed = false;
    private bool isSpace = false;

    [Header("Objects")]
    public GameObject DoorClosed;

    public GameObject DoorOpen;
    public Light RoofLamp;
    public AudioSource Ambient;
    public Renderer Whiteboard;
    public Renderer Poster;
    public Renderer Sayori;
    public Renderer DoorOverlay;
    public AudioReverbZone Echo;
    public Clock Clock;
    public Animator Overlay;

    [Header("Space Objects")]
    public Animator Credits;

    public AudioSource Credits_Music;

    [Header("Misc")]
    public Transform AudioNodeHolder;

    public static List<Transform> AudioNodes;

    public BoxCollider DoorHitbox;

    [Header("Resources")]
    public AudioClip a_giggle;

    public AudioClip a_theme;
    public AudioClip a_canyouhearme;
    public AudioClip a_glitch;
    public AudioClip a_breathing;
    public AudioClip a_spectrum;
    public AudioClip a_flippage;

    public Texture[] whiteboardDrawings;
    public Texture t_posterCursed;

    public Material Skybox_Default;
    public Material Skybox_Space;

    private void Start()
    {
        //Find audio nodes, hide them and add to list
        AudioNodes = new List<Transform>();
        foreach (Transform child in AudioNodeHolder)
        {
            Destroy(child.gameObject.GetComponent<MeshRenderer>());
            AudioNodes.Add(child);
        }
    }

    private void Update()
    {
        if (isSpace) RenderSettings.skybox.SetFloat("_Rotation", Time.time/3);
        else RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (!c.GetComponent<PhotonView>()) return;
        if (roomCursed) return;
        else StartCoroutine(Curse(c.GetComponent<PhotonView>().IsMine, c));
    }

    private void OpenDoor(bool open)
    {
        DoorClosed.SetActive(!open);
        DoorOpen.SetActive(open);

        if (open) DoorOpen.GetComponent<AudioSource>().Play();
        else DoorClosed.GetComponent<AudioSource>().Play();

        Echo.enabled = !open;
    }

    private void SetSpace(bool space)
    {
        isSpace = space;
        RenderSettings.skybox = isSpace ? Skybox_Space : Skybox_Default;
        if (!isSpace) RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    private void Play(AudioClip clip)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/H532AudioNode", typeof(GameObject)), gameObject.transform) as GameObject;
        H532AudioComponent ac = instance.GetComponent<H532AudioComponent>();
        ac.Start(clip, false, Vector3.zero);
    }

    private void Play(AudioClip clip, Vector3 pos)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/H532AudioNode", typeof(GameObject)), gameObject.transform) as GameObject;
        H532AudioComponent ac = instance.GetComponent<H532AudioComponent>();
        ac.Start(clip, true, pos);
    }

    /// <summary>
    /// Starts the H533 curse sequence
    /// </summary>
    /// <param name="isMe">Local variable whether you triggered it yourself or not</param>
    private IEnumerator Curse(bool isMe, Collider c)
    {
        //instants
        DoorHitbox.enabled = true;
        roomCursed = true;
        StartCoroutine(LerpAmbient(0.98f, 0f, 2f));

        yield return new WaitForSeconds(0.3f);
        Play(a_theme);

        //Queued
        StartCoroutine(Queue(1, () => Play(a_giggle)));
        StartCoroutine(Queue(2, () => OpenDoor(false)));
        if (isMe) StartCoroutine(Queue(2, () => StartCoroutine(LerpCamera(0, 10, 10))));
        StartCoroutine(Queue(2, () => RoofLamp.color = new Color(1, 0.7688679f, 0.7753899f)));
        StartCoroutine(Queue(5, () => Play(a_spectrum)));
        StartCoroutine(Queue(7, () => Play(a_canyouhearme)));
        StartCoroutine(Queue(5, () => Clock.modifier = -1000));
        StartCoroutine(Queue(8f, () => Play(a_flippage)));
        StartCoroutine(Queue(8f, () => Poster.material.SetTexture("_MainTex", t_posterCursed)));
        if (isMe) StartCoroutine(Queue(13.9f, () => Play(a_glitch)));
        if (isMe) StartCoroutine(Queue(13.9f, () => StartCoroutine(LerpCamera(10, 350, 1.3f))));
        StartCoroutine(Queue(21.35f, () => StartCoroutine(SetWhiteboard())));
        StartCoroutine(Queue(23f, () => Sayori.enabled = true));
        StartCoroutine(Queue(25.35f, () => Play(a_breathing, Whiteboard.transform.localPosition)));
        StartCoroutine(Queue(25.35f, () => DoorOverlay.enabled = true));

        //Go to space
        if (isMe) StartCoroutine(Queue(34.5f, () => Overlay.Play("ShowOverlay")));
        if (isMe) StartCoroutine(Queue(34.5f, () => StartCoroutine(LerpCamera(350, 0, 4.5f))));
        if (isMe) StartCoroutine(Queue(39.2f, () => SetSpace(true)));
        if (isMe) StartCoroutine(Queue(39.2f, () => Overlay.Play("HideOverlay")));
        if (isMe) StartCoroutine(Queue(39.4f, () => c.transform.position = new Vector3(0, -211, 1233)));
        if (isMe) StartCoroutine(Queue(40f, () => Credits_Music.Play()));
        if (isMe) StartCoroutine(Queue(39.5f, () => Credits.Play("CreditsAnim")));
        if (isMe) StartCoroutine(Queue(77.5f, () => Overlay.Play("ShowOverlay")));
        if (isMe) StartCoroutine(Queue(80f, () => SetSpace(false)));
        if (isMe) StartCoroutine(Queue(80f, () => GoToNode(c)));
        if (isMe) StartCoroutine(Queue(80f, () => Overlay.Play("HideOverlay")));
    }
    private void GoToNode(Collider c)
    {
        GameObject[] SpawnNodes = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        Vector3 SpawnPos = SpawnNodes[UnityEngine.Random.Range(0, SpawnNodes.Length - 1)].transform.position;
        SpawnPos.y++;
        c.transform.position = SpawnPos;
    }

    private IEnumerator LerpCamera(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, 0, Mathf.Lerp(v_start, v_end, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, 0, v_end);
    }

    private IEnumerator LerpAmbient(float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            Ambient.pitch = Mathf.Lerp(v_start, v_end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Ambient.pitch = v_end;
    }

    private IEnumerator SetWhiteboard()
    {
        foreach (Texture t in whiteboardDrawings)
        {
            yield return new WaitForSeconds(0.5f);
            Whiteboard.material.SetTexture("_MainTex", t);
        }
    }

    public static IEnumerator Queue(float delay, Action method)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    private void OnApplicationQuit()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }
}