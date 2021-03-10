using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SplashScreenController : MonoBehaviour
{
    private static bool played = false;

    private IEnumerator Start()
    {
        if (!Application.isEditor && !played)
        {
            played = true;
            Time.timeScale = 0f;
            SplashScreen.Begin();
            while (!SplashScreen.isFinished)
            {
                SplashScreen.Draw();
                yield return null;
            }
            Time.timeScale = 1;
        }
    }
}