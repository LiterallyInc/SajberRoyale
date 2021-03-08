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
            Application.OpenURL("https://discord.com/api/oauth2/authorize?client_id=793179363029549057&redirect_uri=http%3A%2F%2Flocalhost%3A3000%2Fauth%2Froyale&response_type=code&scope=identify");
            ToggleLoginGUI(true);
        }
        public void TryLogin()
        {
            PlayerPrefs.SetString(Helper.Account.access.ToString(), AuthCode.text);
            StartCoroutine(AccountManager.Manager.VerifyLogin(false));
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