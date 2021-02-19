using UnityEngine;

namespace SajberRoyale.MainMenu
{
    public class ThemeSong : MonoBehaviour
    {
        private void Update()
        {
            if (PlayerPrefs.GetInt(Helper.Settings.musicTheme.ToString(), 1) == 1 && !GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
            if (PlayerPrefs.GetInt(Helper.Settings.musicTheme.ToString(), 1) == 0 && GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();
        }
    }
}