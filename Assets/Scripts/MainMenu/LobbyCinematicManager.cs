using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCinematicManager : MonoBehaviour
{
    public Animator BackFade;
    public Animator Camera;
    public List<string> PathNames = new List<string>();
    private int currentPath;
    private bool resetQueued = false;

    private void Start()
    {
        currentPath = Random.Range(0, PathNames.Count);
        SetRandomAnimation();

        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio.transform.root.name == "Map")
            {
                audio.clip = null;
                audio.Stop();
                audio.volume = 0;
            }
        }
    }

    private void Update()
    {
        if (Camera.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f && !resetQueued) StartCoroutine(ChangeCinematic());
    }

    private IEnumerator ChangeCinematic()
    {
        resetQueued = true;
        BackFade.Play("Fadeout");
        yield return new WaitForSeconds(2);
        SetRandomAnimation();
        BackFade.Play("Fadein");
    }

    private void SetRandomAnimation()
    {
        int i = 0;
        while (i < 5)
        {
            int newPath = Random.Range(0, PathNames.Count);
            if (newPath != currentPath)
            {
                currentPath = newPath; break;
            }
            i++;
        }
        Debug.Log($"LobbyCinematicManager/SetRandom: Path set to {PathNames[currentPath]}");

        Camera.Play(PathNames[currentPath], 0, 0);
        resetQueued = false;
    }
}