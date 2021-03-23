using UnityEngine;

namespace SajberRoyale.Map
{
    public class H532AudioComponent : MonoBehaviour
    {
        private GameObject[] points = new GameObject[3];
        private float speed;
        private bool locked = false;
        private int current = 0;

        private void Start()
        {
            Destroy(GetComponent<MeshRenderer>());
            points[0] = CursedH532.AudioNodes[Random.Range(0, CursedH532.AudioNodes.Count)].gameObject;
            points[1] = CursedH532.AudioNodes[Random.Range(0, CursedH532.AudioNodes.Count)].gameObject;
            points[2] = CursedH532.AudioNodes[Random.Range(0, CursedH532.AudioNodes.Count)].gameObject;
        }

        public void Start(AudioClip clip, bool locked, Vector3 pos, bool loop = false)
        {
            this.locked = locked;
            transform.localPosition = pos;
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().loop = loop;
            GetComponent<AudioSource>().Play();
            name = clip.name;
        }

        private void FixedUpdate()
        {
            if (locked) return;
            if (Vector3.Distance(points[current].transform.position, transform.position) < 1)
            {
                current = Random.Range(0, points.Length - 1);
            }
            transform.position = Vector3.MoveTowards(transform.position, points[current].transform.position, Time.deltaTime / 3);
        }
    }
}