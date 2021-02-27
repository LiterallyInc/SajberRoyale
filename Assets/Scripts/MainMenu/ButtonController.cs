using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class ButtonController : MonoBehaviour
    {
        public GameObject ConnectionGUI;
        public AudioSource ClickSound;

        public void ToggleConnectionGUI(bool open)
        {
            ConnectionGUI.SetActive(open);
        }

        public void Click()
        {
            ClickSound.Play();
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}