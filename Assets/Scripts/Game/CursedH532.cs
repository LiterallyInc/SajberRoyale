using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SajberRoyale.Map
{
    public class CursedH532 : MonoBehaviourPun
    {
        private float shakeSpeed = 0;
        public bool isCursed = false;
        private bool roomCursed = false;
        private bool isSpace = false;
        private bool isMe = false;

        [Header("Objects")]
        public GameObject DoorClosed;

        public GameObject DoorOpen;
        public AudioSource Ambient;
        public Renderer Whiteboard;
        public Renderer Poster;
        public Renderer Sayori;
        public Renderer PoemPaper;
        public Renderer DoorOverlay;
        public AudioReverbZone Echo;
        public Clock Clock;
        public Animator RoofLamps;
        public TextMesh Tombstone;
        public Animator Tables;
        public ParticleSystem Smoke;

        [Header("Space Objects")]
        public Animator Credits;

        public AudioSource Credits_Music;

        [Header("Misc")]
        public Transform AudioNodeHolder;

        public static List<Transform> AudioNodes;

        public BoxCollider DoorHitbox;

        [Header("Resources")]
        public AudioClip a_giggle;

        public AudioClip a_deadloop;
        public AudioClip a_theme;
        public AudioClip a_canyouhearme;
        public AudioClip a_glitch;
        public AudioClip a_breathing;
        public AudioClip a_spectrum;
        public AudioClip a_flippage;

        public Texture[] whiteboardDrawings;
        public Texture t_posterCursed;
        public Texture t_poem;

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
            Smoke.Stop();
        }

        private void Update()
        {
            if (isSpace) RenderSettings.skybox.SetFloat("_Rotation", Time.time / 3);
            else RenderSettings.skybox.SetFloat("_Rotation", 0);

            if (Core.Instance) if (Core.Instance.Sync) if (Core.Instance.Sync.PlayerCam.GetComponent<vp_FPCamera>()) Core.Instance.Sync.PlayerCam.GetComponent<vp_FPCamera>().ShakeSpeed = shakeSpeed;
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!c.GetComponent<PhotonView>() && c.name != "HeadCollider") return;
            if (roomCursed) return;

            // start as multiplayer
            if (c.GetComponent<PhotonView>())
            {
                isMe = c.GetComponent<PhotonView>().IsMine;
                if (c.GetComponent<PlayerSync>().Player == null) return;
                else StartCoroutine(Curse(c.transform));
            }
            // start as singleplayer VR
            else
            {
                isMe = true;
                StartCoroutine(Curse(GameObject.FindGameObjectWithTag("VRPlayer").transform, true));
            }
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

        private void Play(AudioClip clip, bool loop)
        {
            GameObject instance = Instantiate(Resources.Load("Prefabs/H532AudioNode", typeof(GameObject)), gameObject.transform) as GameObject;
            H532AudioComponent ac = instance.GetComponent<H532AudioComponent>();
            ac.Start(clip, false, Vector3.zero, loop);
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
        private IEnumerator Curse(Transform c, bool vrMode = false)
        {
            //instants
            DoorHitbox.enabled = true;
            roomCursed = true;
            isCursed = true;
            StartCoroutine(LerpAmbient(0.98f, 0f, 2f));

            yield return new WaitForSeconds(0.3f);
            Play(a_theme);

            //Queued
            StartCoroutine(Queue(1, () => Play(a_giggle)));
            StartCoroutine(Queue(2, () => OpenDoor(false)));
            StartCoroutine(Queue(2.5f, () => Smoke.Play()));
            if (isMe && !vrMode) StartCoroutine(Queue(2, () => Core.Instance.Sync.LocalLight.color = new Color(1, 0.7688679f, 0.7753899f)));
            if (isMe) StartCoroutine(Queue(2f, () => shakeSpeed = 1.5f));
            StartCoroutine(Queue(5, () => Play(a_spectrum)));
            StartCoroutine(Queue(5, () => RoofLamps.Play("Lights")));
            StartCoroutine(Queue(7, () => Play(a_canyouhearme)));
            if (!vrMode) StartCoroutine(Queue(7, () => Tombstone.text = c.GetComponent<PhotonView>().Controller.NickName));
            StartCoroutine(Queue(7, () => Tombstone.transform.parent.gameObject.SetActive(true)));
            StartCoroutine(Queue(5, () => Clock.modifier = -1000));
            StartCoroutine(Queue(8f, () => Play(a_flippage)));
            StartCoroutine(Queue(8f, () => Poster.material.SetTexture("_MainTex", t_posterCursed)));
            StartCoroutine(Queue(20f, () => PoemPaper.material.SetTexture("_MainTex", t_poem)));
            StartCoroutine(Queue(21.35f, () => StartCoroutine(SetWhiteboard())));
            StartCoroutine(Queue(23f, () => Sayori.enabled = true));
            StartCoroutine(Queue(25.35f, () => Play(a_breathing, Whiteboard.transform.localPosition)));
            StartCoroutine(Queue(25.35f, () => DoorOverlay.enabled = true));
            StartCoroutine(Queue(33f, () => Tables.Play("TableFall")));
            StartCoroutine(Queue(40f, () => Play(a_deadloop, true)));
            yield return new WaitForSeconds(33);

            //go to school if dead or in VR
            if (vrMode || (isMe && !Game.Game.Instance.IsAlive))
            {
                StartCoroutine(Queue(1.5f, () => Core.Instance.UI.Overlay.Play("ShowOverlay")));
                StartCoroutine(Queue(1.5f, () => Core.Instance.Sync.PlayerCam.GetComponent<vp_FPCamera>().ShakeAmplitude = new Vector3(10, 10, 10)));
                StartCoroutine(Queue(6f, () => shakeSpeed = 0f));
                StartCoroutine(Queue(6.2f, () => GoToNode(c)));
                StartCoroutine(Queue(6.2f, () => Core.Instance.UI.Overlay.Play("HideOverlay")));
                yield break;
            }

            //go to space if still alive
            if (isMe) StartCoroutine(Queue(1.5f, () => Core.Instance.UI.Overlay.Play("ShowOverlay")));
            if (isMe) StartCoroutine(Queue(1.5f, () => Core.Instance.Sync.PlayerCam.GetComponent<vp_FPCamera>().ShakeAmplitude = new Vector3(10, 10, 10)));
            if (isMe) StartCoroutine(Queue(5.2f, () => Credits_Music.Play()));
            if (isMe) StartCoroutine(Queue(6f, () => shakeSpeed = 0));
            if (isMe) StartCoroutine(Queue(6.2f, () => SetSpace(true)));
            if (isMe) StartCoroutine(Queue(6.2f, () => Core.Instance.Sync.LocalHolder.SetActive(false)));
            if (isMe) StartCoroutine(Queue(6.2f, () => Core.Instance.UI.Overlay.Play("HideOverlay")));
            if (isMe) StartCoroutine(Queue(6.2f, () => Core.Instance.UI.ShowData(false)));
            if (isMe) StartCoroutine(Queue(6.4f, () => c.transform.position = new Vector3(0, -211, 1233)));
            if (isMe) StartCoroutine(Queue(6.5f, () => Credits.Play("CreditsAnim")));
            if (isMe) StartCoroutine(Queue(44.5f, () => shakeSpeed = 10f));
            if (isMe) StartCoroutine(Queue(44.5f, () => Core.Instance.UI.Overlay.Play("ShowOverlay")));
            if (isMe) StartCoroutine(Queue(44.5f, () => shakeSpeed = 0f));
            if (isMe) StartCoroutine(Queue(47f, () => SetSpace(false)));
            if (isMe) StartCoroutine(Queue(47f, () => GoToNode(c)));
            if (isMe) StartCoroutine(Queue(47f, () => Credits.gameObject.SetActive(false)));
            if (isMe) StartCoroutine(Queue(47f, () => Core.Instance.UI.ShowData(true)));
            if (isMe) StartCoroutine(Queue(47f, () => Core.Instance.Sync.LocalLight.color = new Color(1, 1, 1)));
            if (isMe) StartCoroutine(Queue(47f, () => Core.Instance.Sync.LocalHolder.SetActive(true)));
            if (isMe) StartCoroutine(Queue(47f, () => Core.Instance.UI.Overlay.Play("HideOverlay")));
            if (isMe) StartCoroutine(Queue(47f, () => isCursed = false));
        }

        private void GoToNode(Transform c)
        {
            GameObject[] SpawnNodes = GameObject.FindGameObjectsWithTag("PlayerSpawn");
            Vector3 SpawnPos = SpawnNodes[UnityEngine.Random.Range(0, SpawnNodes.Length - 1)].transform.position;
            SpawnPos.y++;
            c.position = SpawnPos;
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
                if (isMe && Core.Instance) Core.Instance.Inventory.photonView.RPC(nameof(DamageController.Hit), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, Random.Range(6, 11), "h532", Game.Game.Instance.Skin);
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
}