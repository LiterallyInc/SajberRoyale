using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class KillfeedEntry : MonoBehaviour
    {
        public Text TextKiller;
        public Text TextKilled;
        public Image IconKiller;
        public Image IconKilled;
        public Image IconWeapon;

        private bool isClosed = false;

        private void Start()
        {
            Invoke(nameof(Close), 4);
        }

        public void Close()
        {
            if (isClosed) return;
            isClosed = true;
            GetComponent<Animator>().Play("Killfeed Close");
            Destroy(gameObject, 0.5f);
        }
    }
}