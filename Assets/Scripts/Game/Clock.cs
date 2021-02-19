using System;
using UnityEngine;

namespace SajberRoyale.Map

{
    public class Clock : MonoBehaviour
    {
        public Transform Sec;
        public Transform Min;
        public Transform Hour;
        public int modifier;

        public AudioClip Tick;
        public AudioClip Tock;
        public bool hasTocked = false;

        private void Start()
        {
            Sec.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Second * 6, 0, 0));
            Min.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Minute * 6, 0, 0));
            Hour.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Hour * 30 - DateTime.Now.Minute * 0.5f, 0, 0));
            InvokeRepeating("UpdateClock", 1, 1);
            InvokeRepeating("TickTock", 0.7f, 1f);
        }

        private void UpdateClock()
        {
            Sec.Rotate(new Vector3(-6 * modifier, 0, 0));
            Min.Rotate(new Vector3(-6 * modifier / 60, 0, 0));
            Hour.Rotate(new Vector3(-6 * modifier / 60 / 90, 0, 0));
        }

        private void TickTock()
        {
            GetComponent<AudioSource>().clip = hasTocked ? Tick : Tock;
            GetComponent<AudioSource>().Play();
            hasTocked = !hasTocked;
        }
    }
}