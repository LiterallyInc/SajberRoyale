using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace SajberRoyale.MainMenu
{
    public class Intro : MonoBehaviour
    {
        public float introLenght;

        private void Start()
        {
            if (PlayerPrefs.GetInt(Helper.Settings.isDev.ToString(), 0) == 1) Helper.IsDev = true;

            GetComponent<VideoPlayer>().SetDirectAudioVolume(0, PlayerPrefs.GetFloat(Helper.Settings.volumeMaster.ToString(), 0.5f));
            if (PlayerPrefs.GetInt(Helper.Settings.playIntro.ToString(), 1) == 0)
            {
                SceneManager.LoadScene("main");
            }
            else
            {
                Cursor.visible = false;
                StartCoroutine(Wait());
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Cursor.visible = true;
                SceneManager.LoadScene(1);
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(introLenght);
            Cursor.visible = true;
            SceneManager.LoadScene("main");
        }
    }
}