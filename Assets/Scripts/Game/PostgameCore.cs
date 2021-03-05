using Photon.Pun;
using SajberRoyale.Items;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace SajberRoyale.Game
{
    public class PostgameCore : MonoBehaviour
    {
        public string winnerSkin;
        public int winnerEmote;

        public Emote[] VictoryEmotes;
        private Animator CharacterAnim;

        public Light DirLight;
        public Light SpotLight;

        public void Victory()
        {
            //instansiate victory character
            GameObject character = Instantiate(Resources.Load($"CharMeshes/{winnerSkin}"), new Vector3(-9.8f, 3.102514f, -16.32f), Quaternion.Euler(0, 85, 0)) as GameObject;
            Destroy(character.GetComponent<PhotonAnimatorView>());
            Destroy(character.GetComponent<PhotonView>());
            Destroy(character.GetComponent<vp_FPBodyAnimator>());
            Destroy(character.GetComponent<vp_PlayerFootFXHandler>());
            CharacterAnim = character.GetComponent<Animator>();

            //start camera animation
            Core.Instance.Camera.SetActive(true);
            Core.Instance.Camera.GetComponent<Animator>().Play("CameraWin", 0, 0);
            StartCoroutine(AnimateCharacter());
            StartCoroutine(Leave());

            //set light
            DirLight.shadowStrength = 0;
            SpotLight.intensity = 0.9f;

        }

        private IEnumerator AnimateCharacter()
        {
            Emote e = VictoryEmotes[winnerEmote];
            CharacterAnim.Play(e.id, 1, 0);
            CharacterAnim.Play(e.id, 2, 0);
            Debug.Log(e.id);
            if (e.length != 0)
            {
                yield return new WaitForSeconds(e.length);
                CharacterAnim.Play("Idle", 1, 0);
                CharacterAnim.Play("Idle", 2, 0);
            }
            else if (e.freeze != 0)
            {
                yield return new WaitForSeconds(e.freeze);
                CharacterAnim.speed = 0;
            }
        }
        private IEnumerator Leave()
        {
            yield return new WaitForSeconds(25.5f);
            SplashScreen.Begin();
            yield return new WaitForSeconds(3.2f);
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("main");
        }
    }
}