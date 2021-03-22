// this is NOT how to do settings menus do not take inspiration from me this is bad practice and i'll rewrite this someday. yikes
using Photon.Pun;
using SajberRoyale.Game;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class Settings : MonoBehaviour
    {
        private bool isOpen = false;

        public Slider InputDiscord;
        public Slider InputTheme;
        public Slider InputVolume;
        public Slider InputFOV;
        public Slider InputSens;
        public Dropdown InputQuality;
        public Text TextVolume;
        public Text TextFOV;
        public Text TextSens;

        public Button LeaveGame;

        private string KeyDiscord;
        private string KeyTheme;
        private string KeyVolume;
        private string KeyFOV;
        private string KeyQuality;
        private string KeySens;

        private bool isGame;

        private void Start()
        {
            isGame = SceneManager.GetActiveScene().name == "game";
            LeaveGame.gameObject.SetActive(isGame);
            KeyDiscord = Helper.Settings.discordRpc.ToString();
            KeyTheme = Helper.Settings.musicTheme.ToString();
            KeyVolume = Helper.Settings.volumeMaster.ToString();
            KeyFOV = Helper.Settings.fov.ToString();
            KeyQuality = Helper.Settings.quality.ToString();
            KeySens = Helper.Settings.sens.ToString();

            InputDiscord.SetValueWithoutNotify(PlayerPrefs.GetInt(KeyDiscord, 1));
            InputTheme.SetValueWithoutNotify(PlayerPrefs.GetInt(KeyTheme, 1));
            InputVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyVolume, 0.5f));
            InputFOV.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyFOV, 80));
            InputSens.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeySens, 5));
            InputQuality.SetValueWithoutNotify(PlayerPrefs.GetInt(KeyQuality, 5));

            AudioListener.volume = InputVolume.value;
            SetVolume(InputVolume.value);
            SetFOV(InputFOV.value);
            SetSens(InputSens.value);
            Helper.sens = InputSens.value;
            SetQuality(InputQuality.value);
        }

        private void Update()
        {
            if (isGame)
            {
                if (isOpen)
                {
                    vp_Utility.LockCursor = false;
                    if (Input.GetKeyDown(KeyCode.Escape)) OpenMenu(false);
                }
                else if (Input.GetKeyDown(KeyCode.Escape)) OpenMenu(true);
            }
        }

        public void ToggleDiscord(float n)
        {
            PlayerPrefs.SetInt(KeyDiscord, Mathf.RoundToInt(n));
            if (n == 1) DiscordController.Instance.StartRPC();
        }

        public void SetQuality(int n)
        {
            PlayerPrefs.SetInt(KeyQuality, n);
            QualitySettings.SetQualityLevel(InputQuality.value);
        }

        public void ToggleTheme(float n)
        {
            PlayerPrefs.SetInt(KeyTheme, Mathf.RoundToInt(n));
        }

        public void SetVolume(float n)
        {
            AudioListener.volume = n;
            PlayerPrefs.SetFloat(KeyVolume, n);
            TextVolume.text = $"{Mathf.RoundToInt(InputVolume.value * 100)}%";
        }

        public void SetFOV(float n)
        {
            PlayerPrefs.SetFloat(KeyFOV, n);
            if (n >= 61) TextFOV.text = $"{Mathf.RoundToInt(InputFOV.value)}°";
            else TextFOV.text = "headache";
        }

        public void SetSens(float n)
        {
            PlayerPrefs.SetFloat(KeySens, n);
            TextSens.text = Math.Round(InputSens.value, 1).ToString();
            Helper.sens = n;
        }

        public void OpenMenu(bool open)
        {
            transform.localScale = open ? Vector3.one : Vector3.zero;
            isOpen = open;
        }

        public void Leave()
        {
            if (Game.Game.Instance.IsActive && Game.Game.Instance.IsAlive) Core.Instance.Inventory.photonView.RPC("Die", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, "suicide", Game.Game.Instance.Skin, Game.Game.Instance.Skin);
            MatchCreator.LeaveGame();
        }
    }
}