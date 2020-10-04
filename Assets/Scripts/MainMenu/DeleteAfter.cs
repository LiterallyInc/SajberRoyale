using System.Collections;
using UnityEngine;

public class DeleteAfter : MonoBehaviour
{
    public float Delay;

    private void Start()
    {
        StartCoroutine(DeleteIn());
    }

    private IEnumerator DeleteIn()
    {
        yield return new WaitForSeconds(Delay);
        Destroy(this.gameObject);
    }
}