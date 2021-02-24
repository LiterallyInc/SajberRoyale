// this is NOT how to do settings menus do not take inspiration from me this is bad practice and i'll rewrite this someday. yikes
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class Settings : MonoBehaviour
    {
        private bool isOpen = false;

        public Toggle InputIntro;
        public Toggle InputDiscord;
        public Toggle InputTheme;
        public Toggle InputServer;
        public Slider InputVolume;
        public Slider InputFOV;
        public Slider InputSens;
        public Text TextVolume;
        public Text TextFOV;
        public Text TextSens;
        public Text TextServer;

        private string KeyIntro;
        private string KeyDiscord;
        private string KeyTheme;
        private string KeyVolume;
        private string KeyFOV;
        private string KeyServer;
        private string KeySens;

        private void Start()
        {
            KeyIntro = Helper.Settings.playIntro.ToString();
            KeyDiscord = Helper.Settings.discordRpc.ToString();
            KeyTheme = Helper.Settings.musicTheme.ToString();
            KeyVolume = Helper.Settings.volumeMaster.ToString();
            KeyFOV = Helper.Settings.fov.ToString();
            KeyServer = Helper.Settings.altServer.ToString();
            KeySens = Helper.Settings.sens.ToString();

            InputIntro.SetIsOnWithoutNotify(PlayerPrefs.GetInt(KeyIntro, 1) == 1);
            InputDiscord.SetIsOnWithoutNotify(PlayerPrefs.GetInt(KeyDiscord, 1) == 1);
            InputTheme.SetIsOnWithoutNotify(PlayerPrefs.GetInt(KeyTheme, 1) == 1);
            InputVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyVolume, 0.5f));
            InputFOV.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyFOV, 80));
            InputSens.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeySens, 5));

            AudioListener.volume = InputVolume.value;
            SetVolume(InputVolume.value);
            SetFOV(InputFOV.value);
            SetSens(InputSens.value);
            Helper.sens = InputSens.value;
            ToggleServer(false);
        }

        private void Update()
        {
            if (isOpen && Input.GetKeyDown(KeyCode.Escape)) OpenMenu(false);
        }

        public void ToggleIntro(bool n)
        {
            PlayerPrefs.SetInt(KeyIntro, n ? 1 : 0);
        }

        public void ToggleDiscord(bool n)
        {
            PlayerPrefs.SetInt(KeyDiscord, n ? 1 : 0);
            if (n) DiscordController.Instance.StartRPC();
        }

        public void ToggleServer(bool n)
        {
            PlayerPrefs.SetInt(KeyServer, n ? 1 : 0);
            TextServer.gameObject.SetActive(n);
        }

        public void ToggleTheme(bool n)
        {
            PlayerPrefs.SetInt(KeyTheme, n ? 1 : 0);
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