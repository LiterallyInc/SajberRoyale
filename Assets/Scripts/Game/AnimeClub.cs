using SajberRoyale.Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SajberRoyale.Map
{
    public class AnimeClub : MonoBehaviour
    {
        public AudioSource Audio;
        public List<AudioClip> Songs;
        private int currentSong = 0;

        public void StartAudio()
        {
            Shuffle(Songs, Core.Instance.seed);
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "vr")
            {
                if (Game.Game.Instance == null) return;
                if (!Audio.isPlaying && Game.Game.Instance.IsActive) NextSong();
            }
            else
            {
                if (!Audio.isPlaying) NextSong();
            }
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
}