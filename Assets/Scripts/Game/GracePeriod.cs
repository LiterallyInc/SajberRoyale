using System.Collections;
using UnityEngine;

namespace SajberRoyale.Game
{
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
            if (TimeLeft > 0) Core.Instance.UI.StatusText.text = $"Items will be activated in {TimeLeft}s!";
            if (TimeLeft == 0)
            {
                Game.Instance.GracePeriod = false;
                StartCoroutine(Status());
            }
        }

        private IEnumerator Status()
        {
            Core.Instance.UI.StatusText.text = "Items activated!";
            yield return new WaitForSeconds(5);
            Core.Instance.UI.StatusText.text = "";
            Destroy(gameObject);
        }
    }
}