using UnityEngine;

namespace SajberRoyale.Items
{
    [CreateAssetMenu(fileName = "Emote", menuName = "SajberRoyale/Emote", order = 1)]
    [System.Serializable]
    public class Emote : ScriptableObject
    {
        /// <summary>
        /// Animation clip name
        /// </summary>
        public string id;
        /// <summary>
        /// Audio clip to play during emote
        /// </summary>
        public AudioClip audio;
        /// <summary>
        /// Length of emote. 0 for infinity
        /// </summary>
        public float length;
        /// <summary>
        /// Freeze emote after x seconds. 0 for none
        /// </summary>
        public float freeze;
        /// <summary>
        /// Icon shown in emote wheel
        /// </summary>
        public Sprite Icon;
    }
}