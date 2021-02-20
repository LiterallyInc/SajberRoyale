using SajberRoyale.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GracePeriod : MonoBehaviour
{
    public int TimeLeft = 30;
    
    private void Start()
    {
        InvokeRepeating(nameof(UpdateTimer), 0, 1);
        Game.Instance.GracePeriod = true;
    }
    private void UpdateTimer()
    {
        TimeLeft--;
        if(TimeLeft > 0) Core.Instance.UI.StatusText.text = $"Föremål kommer aktiveras om {TimeLeft}s!";
        if (TimeLeft == 0)
        {
            Game.Instance.GracePeriod = false;
            StartCoroutine(Status());
        }
    }
    IEnumerator Status()
    {
        Core.Instance.UI.StatusText.text = "Föremål aktiverade!";
        yield return new WaitForSeconds(5);
        Core.Instance.UI.StatusText.text = "";
        Destroy(gameObject);
    }
}
