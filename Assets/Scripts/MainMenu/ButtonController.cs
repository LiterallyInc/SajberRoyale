using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class ButtonController : MonoBehaviour
    {
        public GameObject ConnectionGUI;
        public GameObject LoginGUI;
        public InputField AuthCode;
        public AudioSource ClickSound;
        public Text LoginStatus;
        public Button Login;
        public static ButtonController Instance;
        private void Start()
        {
            Destroy(Instance);
            Instance = this;
        }
        public void ToggleConnectionGUI(bool open)
        {
            ConnectionGUI.SetActive(open);
        }

        public void Click()
        {
            ClickSound.Play();
        }

        public void OpenLeaderboards()
        {
            Application.OpenURL(AccountManager.Manager.ApiEndPoint + "/sajberroyale");
        }
        public void OpenLogin()
        {
            Application.OpenURL(AccountManager.Manager.AuthLink);
            ToggleLoginGUI(true);
        }
        public void TryLogin()
        {
            StartCoroutine(AccountManager.Manager.ProcessToken(AuthCode.text));
        }
        public void ToggleLoginGUI(bool open)
        {
            LoginGUI.SetActive(open);
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}