using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeClub : MonoBehaviour
{
    public AudioSource Audio;
    public List<AudioClip> Songs;
    private int currentSong = 0;

    public void StartAudio()
    {
        Shuffle(Songs, Core.Instance.seed);
    }
    void Update()
    {
        if (!Audio.isPlaying && Game.Instance.IsActive) NextSong();
    }
    private void NextSong()
    {
        currentSong++;
        if (currentSong == Songs.Count) currentSong = 0;
        Audio.clip = Songs[currentSong];
        Audio.Play();
    }
    public static void Shuffle<T>(IList<T> list, int seed)
    {
        var rng = new System.Random(seed);
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
