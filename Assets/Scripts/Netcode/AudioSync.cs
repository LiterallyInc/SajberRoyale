using UnityEngine;

public class AudioSync : MonoBehaviour
{
    public void Restart()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Restarts all AudioSources in scene with an AudioSync component
    /// </summary>
    public static void RestartAll()
    {
        foreach (AudioSync sync in FindObjectsOfType<AudioSync>())
        {
            sync.Restart();
        }
    }
}