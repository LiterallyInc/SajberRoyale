using System.Collections;
using UnityEngine;

public class DeleteAfter : MonoBehaviour
{
    public float Delay;

    private void Start()
    {
        Destroy(this.gameObject, Delay);
    }
}