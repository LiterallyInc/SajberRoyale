using Photon.Pun;
using SajberRoyale.Items;
using SajberRoyale.MainMenu;
using System.Collections;
using System.Collections.Generic;
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
        public Emote ThumbsDown;
        public Emote Salute;
        private Animator CharacterAnim;

        public List<Transform> LoserPositions;

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
            Core.Instance.Camera.GetComponent<Light>().intensity = 0;
            Core.Instance.Camera.GetComponent<Animator>().Play("CameraWin", 0, 0);
            StartCoroutine(AnimateCharacter());
            StartCoroutine(Leave());

            //set light
            DirLight.shadowStrength = 0;
            SpotLight.intensity = 0.9f;

            PlaceLosers();
        }

        /// <summary>
        /// Places all loser characters and adds animations to them
        /// </summary>
        private void PlaceLosers()
        {
            //shuffle with synced random
            for (int i = 0; i < LoserPositions.Count; i++)
            {
                int rnd = Random.Range(0, LoserPositions.Count);
                Transform temp = LoserPositions[rnd];
                LoserPositions[rnd] = LoserPositions[i];
                LoserPositions[i] = temp;
            }
            Core.Instance.PlayerSkins.Remove(winnerSkin);
            for (int i = 0; i < Core.Instance.PlayerSkins.Count; i++)
            {
                //create loser
                GameObject character = Instantiate(Resources.Load($"CharMeshes/{Core.Instance.PlayerSkins[i]}"), LoserPositions[i].position, LoserPositions[i].rotation) as GameObject;
                Destroy(character.GetComponent<PhotonAnimatorView>());
                Destroy(character.GetComponent<PhotonView>());
                Destroy(character.GetComponent<vp_FPBodyAnimator>());
                Destroy(character.GetComponent<vp_PlayerFootFXHandler>());

                //animate loser
                int emote = Random.Range(0, 12);
                float startPos = Random.value * 3;
                float startTime = Random.value * 5;
                Animator anim = character.GetComponent<Animator>();
                if (emote < 8)
                {
                    anim.Play("SlowClap", 1, startPos);
                    anim.Play("SlowClap", 2, startPos);
                }
                else if (emote > 7 && emote <= 8) StartCoroutine(DelayAnimation(anim, Salute, startTime, startPos));
                else if (emote > 8 && emote <= 9) StartCoroutine(DelayAnimation(anim, ThumbsDown, startTime, startPos));
                else
                {
                    anim.Play("Confused", 1, startPos);
                    anim.Play("Confused", 2, startPos);
                }
            }
        }

        private IEnumerator DelayAnimation(Animator loser, Emote emote, float startTime, float startPos)
        {
            yield return new WaitForSeconds(startTime);
            loser.Play(emote.id, 1, startPos);
            loser.Play(emote.id, 2, startPos);
            yield return new WaitForSeconds(emote.length);
            loser.Play("Idle", 1, Random.value);
            loser.Play("Idle", 2, Random.value);
        }

        private IEnumerator AnimateCharacter()
        {
            Emote e = VictoryEmotes[winnerEmote];
            CharacterAnim.Play(e.id, 1, 0);
            CharacterAnim.Play(e.id, 2, 0);
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
            MatchCreator.LeaveGame();
        }
    }
}