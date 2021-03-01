using UnityEngine;

namespace SajberRoyale.Game
{
    public class AudioContainer : MonoBehaviour
    {
        private AudioSource source;

        private void Start()
        {
            source = GetComponent<AudioSource>();
            source.Play();
        }

        private void Update()
        {
            if (!source.isPlaying) Destroy(gameObject);
        }
    }
}