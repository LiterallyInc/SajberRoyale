using SajberRoyale.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SajberRoyale.Game
{
    public class EmoteWheel : MonoBehaviour
    {
        public Image[] Icons;

        private void Start()
        {
            PlayerManager m = Core.Instance.UI.GetComponent<PlayerManager>();
            for (int i = 0; i < m.Emotes.Length; i++)
            {
                Icons[i].sprite = m.Emotes[i].Icon;
            }
        }
    }
}