using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is independent from Reactor Console
/// </summary>

public class ConsoleActivator : MonoBehaviour
{

    private void Start()
    {
        Console.DeveloperConsole.active = false;
    }
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.F4) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.F4) && Input.GetKeyDown(KeyCode.LeftShift)))
        {
            Console.DeveloperConsole.active = !Console.DeveloperConsole.active;
        }
    }
}
