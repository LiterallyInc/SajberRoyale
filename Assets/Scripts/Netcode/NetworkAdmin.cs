using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.UI
{
    public class NetworkAdmin : MonoBehaviour
    {
        public Canvas AdminCanvas;
        public bool AdminMenuOpen = false;
        public InputField IF_Password;
        public InputField IF_Roomcode;

        private const string passwordhash = "A169577AB6A460D28C9BBF3ACDD5C653E153BBA872106E79B49EBC251C615CD0C7C0D3213A84A0FD2EBA945FAC161A3743BDFE4DF9297EBC392504CB6C6A0EAE";

        private void Start()
        {
            Object.DontDestroyOnLoad(AdminCanvas.gameObject);
        }

        private void Update()
        {
            //toggle menu
            if (Input.GetKeyDown(KeyCode.F1))
            {
                transform.localScale = AdminMenuOpen ? Vector3.zero : Vector3.one;
                AdminMenuOpen = !AdminMenuOpen;
            }
            TryToLogin();
        }
        private void TryToLogin()
        {
            bool loggedin = Application.isEditor || IF_Password.text.SHA512() == passwordhash;
            IF_Password.interactable = !loggedin;
            IF_Roomcode.interactable = loggedin && PhotonNetwork.IsConnected;
        }

        #region Called by UI

        public void CreateRoom(string name)
        {
            Webhook.Send($"**{PhotonNetwork.NickName}** created a room called \"**{name}**\".");
            PhotonNetwork.CreateRoom(name);
        }

        #endregion Called by UI
    }
}