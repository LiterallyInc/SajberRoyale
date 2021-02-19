using UnityEngine;

/// <summary>
/// makes music sources in-game respect the music toggle in settings
/// </summary>
namespace SajberRoyale.Map
{
    public class MusicLevel : MonoBehaviour
    {
        private bool isVA = false;
        private new AudioSource audio;
        private VA_AudioSource Vaudio;
        private float baseVolume = 1;

        private void Start()
        {
            if (GetComponent<VA_AudioSource>())
            {
                Vaudio = GetComponent<VA_AudioSource>();
                isVA = true;
            }
            if (GetComponent<AudioSource>())
            {
                audio = GetComponent<AudioSource>();
                baseVolume = isVA ? Vaudio.BaseVolume : audio.volume;
            }
            else Destroy(this);
        }

        private void Update()
        {
            float vol = PlayerPrefs.GetInt(Helper.Settings.musicTheme.ToString(), 1) == 1 ? baseVolume : 0;
            if (audio == null) return;
            if (isVA) Vaudio.BaseVolume = vol;
            else
                audio.volume = vol;
        }
    }
}