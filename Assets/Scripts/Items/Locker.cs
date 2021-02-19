using SajberRoyale.Items;
using UnityEngine;

namespace SajberRoyale.Map
{
    public class Locker : ItemNode
    {
        public GameObject Door;
        public bool isOpen = false;

        public void Open()
        {
            Door.transform.localRotation = Quaternion.Euler(0, -120, 0);
            if (hasItem) ShowItem();
            isOpen = true;
            GetComponent<AudioSource>().Play();
            GetComponent<Animator>().Play("OpenLocker");
        }
    }
}