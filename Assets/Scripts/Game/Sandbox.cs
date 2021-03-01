using SajberRoyale.Map;
using UnityEngine;

namespace SajberRoyale.Game
{
    /// <summary>
    /// removes weapons n inappropriate shit from the showcase scene
    /// </summary>
    public class Sandbox : MonoBehaviour
    {
        public CharacterController Character;
        public GameObject[] Objects;

        private void Start()
        {
            foreach (GameObject node in GameObject.FindGameObjectsWithTag("ItemNode"))
            {
                if (!node.GetComponent<Locker>())
                {
                    Destroy(node);
                }
                else
                {
                    Destroy(node.GetComponent<Locker>().light);
                    Destroy(node.GetComponent<Locker>().particles);
                }
            }
            foreach (GameObject obj in Objects) Destroy(obj);
            Character.radius = 0.21f;
        }
    }
}