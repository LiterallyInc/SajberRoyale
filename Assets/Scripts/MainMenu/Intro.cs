﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SajberRoyale.MainMenu
{
    public class Intro : MonoBehaviour
    {
        public float introLenght;

        private void Start()
        {
            StartCoroutine(Wait());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(1);
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(introLenght);
            SceneManager.LoadScene(1);
        }
    }
}