using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAnimation : MonoBehaviour
{
    public string anim;
    void Start()
    {
        float offset = Random.value * 30;
        Destroy(GetComponent<vp_FPBodyAnimator>());
        GetComponent<Animator>().Play(anim, 1, offset);
        GetComponent<Animator>().Play(anim, 2, offset);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<Animator>().Play(anim, 1, 0);
            GetComponent<Animator>().Play(anim, 2, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GetComponent<Animator>().speed = 0;
            GetComponent<Animator>().speed = 0;
        }
    }
}
