using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class CompassHandler : MonoBehaviour
    {
        public RawImage Compass;

        private void Update()
        {
            if (Camera.main) Compass.uvRect = new Rect((Camera.main.gameObject.transform.parent.localEulerAngles.y - 90) / 360, 0, 1, 1);
        }
    }
}