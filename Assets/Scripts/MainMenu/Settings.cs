// this is NOT how to do settings menus do not take inspiration from me this is bad practice and i'll rewrite this someday. yikes
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class Settings : MonoBehaviour
    {
        private bool isOpen = false;

        public Slider InputDiscord;
        public Slider InputTheme;
        public Slider InputServer;
        public Slider InputVolume;
        public Slider InputFOV;
        public Slider InputSens;
        public Text TextVolume;
        public Text TextFOV;
        public Text TextSens;
        public Text TextServer;

        private string KeyDiscord;
        private string KeyTheme;
        private string KeyVolume;
        private string KeyFOV;
        private string KeyServer;
        private string KeySens;

        private void Start()
        {
            KeyDiscord = Helper.Settings.discordRpc.ToString();
            KeyTheme = Helper.Settings.musicTheme.ToString();
            KeyVolume = Helper.Settings.volumeMaster.ToString();
            KeyFOV = Helper.Settings.fov.ToString();
            KeyServer = Helper.Settings.altServer.ToString();
            KeySens = Helper.Settings.sens.ToString();

            InputDiscord.SetValueWithoutNotify(PlayerPrefs.GetInt(KeyDiscord, 1));
            InputTheme.SetValueWithoutNotify(PlayerPrefs.GetInt(KeyTheme, 1));
            InputVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyVolume, 0.5f));
            InputFOV.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyFOV, 80));
            InputSens.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeySens, 5));
            InputSens.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeySens, 5));

            AudioListener.volume = InputVolume.value;
            SetVolume(InputVolume.value);
            SetFOV(InputFOV.value);
            SetSens(InputSens.value);
            Helper.sens = InputSens.value;
            ToggleServer(0);
        }

        private void Update()
        {
            if (isOpen && Input.GetKeyDown(KeyCode.Escape)) OpenMenu(false);
        }

        public void ToggleDiscord(float n)
        {
            PlayerPrefs.SetInt(KeyDiscord, Mathf.RoundToInt(n));
            if (n == 1) DiscordController.Instance.StartRPC();
        }

        public void ToggleServer(float n)
        {
            PlayerPrefs.SetInt(KeyServer, Mathf.RoundToInt(n));
            TextServer.gameObject.SetActive(n == 1);
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
    }
}