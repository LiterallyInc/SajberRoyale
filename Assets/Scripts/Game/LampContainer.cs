using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
public class LampContainer : MonoBehaviour
{
    public Light[] lights;

    public void TurnOff(string room)
    {
        Debug.Log($"Lamp in room {room} turned off.");
        foreach (Light lamp in lights)
        {
            lamp.intensity = 0.1f;
        }
    }
}
