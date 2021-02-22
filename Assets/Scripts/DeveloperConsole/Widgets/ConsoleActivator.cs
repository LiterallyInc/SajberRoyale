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
        if (Input.GetKeyDown(KeyCode.F4) && Helper.IsDev)
        {
            Console.DeveloperConsole.active = !Console.DeveloperConsole.active;
            vp_Utility.LockCursor = !Console.DeveloperConsole.active;
        }
    }
}
