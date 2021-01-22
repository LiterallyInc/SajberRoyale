using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public GameObject Sec;
    public GameObject Min;
    public GameObject Hour;

    private void Start()
    {
        Sec.transform.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Second * 6, 0, 0));
        Min.transform.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Minute * 6, 0, 0));
        Hour.transform.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Hour * 30, 0, 0));
        InvokeRepeating("UpdateClock", 1, 0.1666f);
    }

    private void UpdateClock()
    {
        Sec.transform.Rotate(new Vector3(-1, 0, 0));
        Min.transform.Rotate(new Vector3(-1 / 60, 0, 0));
        Hour.transform.Rotate(new Vector3(-1 / 60 / 90, 0, 0));
    }
}