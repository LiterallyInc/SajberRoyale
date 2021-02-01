using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeSong : MonoBehaviour
{
    void Update()
    {
        if (PlayerPrefs.GetInt(Helper.Settings.musicTheme.ToString(), 1) == 1 && !GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
        if (PlayerPrefs.GetInt(Helper.Settings.musicTheme.ToString(), 1) == 0 && GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();
    }
}
