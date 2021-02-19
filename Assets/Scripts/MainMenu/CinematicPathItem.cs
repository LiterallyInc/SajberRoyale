using Cinemachine;
using System;
using UnityEngine;

namespace SajberRoyale.MainMenu
{
    [CreateAssetMenu(fileName = "Cinematic Path", menuName = "SajberRoyale/Cinematic Path", order = 1)]
    [Serializable]
    public class CinematicPathItem : ScriptableObject
    {
        /// <summary>
        /// GameObject the camera should focus on
        /// </summary>
        public GameObject Target;

        /// <summary>
        /// Path camera should follow
        /// </summary>
        public CinemachineSmoothPath Path;
    }
}