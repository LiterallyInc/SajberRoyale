using SajberRoyale.Map;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SajberRoyale.Game
{
    /// <summary>
    /// removes weapons n inappropriate shit from the showcase scene
    /// </summary>
    public class Sandbox : MonoBehaviour
    {
        public CharacterController Character;
        public GameObject[] Objects;
        public GameObject HelpText;
        public GameObject CutsceneCamera;
        public Animator Animator;
        public Animator Fade;

        private string currentAnim = null;

        private void Start()
        {
            Character.gameObject.SetActive(true);
            CutsceneCamera.SetActive(false);
            Fade.speed = 3f;
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("ItemNode"))
            {
                if (!node.GetComponent<Locker>())
                {
                    Destroy(node);
                }
                else
                {
                    Destroy(node.GetComponent<Locker>().light);
                    Destroy(node.GetComponent<Locker>().particles);
                }
            }
            foreach (GameObject obj in Objects) Destroy(obj);
            Character.radius = 0.21f;
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "sandbox")
            {
                if (Input.GetKeyDown(KeyCode.Alpha0)) HelpText.SetActive(!HelpText.activeSelf);
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Character.gameObject.SetActive(true);
                    CutsceneCamera.SetActive(false);
                    currentAnim = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(StartAnim("Stairwell"));
                else if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(StartAnim("StairwellBTS"));
                else if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(StartAnim("Building"));
                else if (Input.GetKeyDown(KeyCode.Alpha5)) StartCoroutine(StartAnim("BuildingBTS"));
                else if (Input.GetKeyDown(KeyCode.Alpha6)) StartCoroutine(StartAnim("Enviroment"));
                else if (Input.GetKeyDown(KeyCode.Alpha7)) StartCoroutine(StartAnim("EnviromentBTS"));
                
            }

            if(currentAnim != null && Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
            {
                StartCoroutine(StartAnim(currentAnim));
                currentAnim = null;
            }
        }
        private IEnumerator StartAnim(string name)
        {
            
            Fade.Play("Fadeout", 0,0);
            yield return new WaitForSeconds(1);
            Animator.Play(name, 0, 0);
            currentAnim = name;
            Character.gameObject.SetActive(false);
            CutsceneCamera.SetActive(true);
            
            Fade.Play("Fadein",0,0);
        }
    }
}