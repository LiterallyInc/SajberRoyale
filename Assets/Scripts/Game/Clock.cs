using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform Sec;
    public Transform Min;
    public Transform Hour;
    public int modifier;

    private void Start()
    {
        Sec.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Second * 6, 0, 0));
        Min.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Minute * 6, 0, 0));
        Hour.rotation = Quaternion.Euler(new Vector3(-90 - DateTime.Now.Hour * 30, 0, 0));
        InvokeRepeating("UpdateClock", 1, 0.16666f);
    }

    private void UpdateClock()
    {
        Sec.Rotate(new Vector3(-1 * modifier, 0, 0));
        Min.Rotate(new Vector3(-1 * modifier / 60, 0, 0));
        Hour.Rotate(new Vector3(-1 * modifier / 60 / 90 , 0, 0));
    }
}