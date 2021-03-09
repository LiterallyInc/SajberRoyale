using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SajberRoyale.MainMenu
{
    public class AccountManager : MonoBehaviour
    {
        public static AccountManager Manager = null;
        public bool isLoggedIn = false;
        public string ApiEndPoint;

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(VerifyLogin());
            if (Manager == null)
            {
                Manager = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public IEnumerator VerifyLogin(bool isAuto = true)
        {
            string id = PlayerPrefs.GetString(Helper.Account.id.ToString(), "");
            string auth = PlayerPrefs.GetString(Helper.Account.access.ToString(), "");

            if (!(id.Length > 0) && !(auth.Length > 0)) yield break;

            WWWForm form = new WWWForm();
            form.AddField("id", id);
            form.AddField("access_token", auth);

            using (UnityWebRequest www = UnityWebRequest.Post($"{ApiEndPoint}/auth/verify", form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
                    {
                        sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                    }

                    if (www.downloadHandler.text == "true") //logged in
                    {
                        Debug.Log($"Successfully logged in as {PlayerPrefs.GetString(Helper.Account.id.ToString())}");
                        ButtonController.Instance.LoginStatus.text = "Welcome!";
                        ButtonController.Instance.Login.interactable = false;
                        ButtonController.Instance.Login.GetComponent<Text>().text = "LOGGED IN";
                        ButtonController.Instance.ToggleLoginGUI(false);
                        isLoggedIn = true;
                    }
                    else //token invalid
                    {
                        if (!isAuto) ButtonController.Instance.LoginStatus.text = "Could not log you in.";
                    }
                }
            }
        }
    }
}