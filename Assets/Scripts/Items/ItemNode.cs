using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemNode : MonoBehaviour
{
    [Header("Item properties")]
    [Tooltip("Multiplies the default spawn rate. If the spawn odds are 0.4 and modifier 2, this node will spawn an item 80% of the time.")]
    public float rarityModifier = 1f;

    [Tooltip("Bias towards a category. Selected cateogry will have 50% higher drop rate here.")]
    public Bias bias = Bias.Anything;

    [Tooltip("If checked, the biased item will have a 100% drop rate here.")]
    public bool forceBias = false;

    [Header("GameObjects")]
    [Tooltip("The item gameobject will be attached to this object floating up and down")]
    public GameObject itemHolder;

    [Tooltip("Speed of the floating")]
    public float speed = 2f;

    [Tooltip("Height of the floating")]
    public float height = 2.5f;

    [Header("ItemNode")]
    public ParticleSystem particles;

    public new Light light;

    public bool hasItem = false;
    public Item item;
    public bool isLocker = false;

    public enum Bias
    {
        Anything = 0,
        Weapon = 1,
        Ammo = 2,
        Healing = 3
    }

    private void Start()
    {
        isLocker = !name.Contains("ItemNode");
        if (isLocker) height /= 11;
        if (SceneManager.GetActiveScene().name == "main")
        {
            Destroy();
        }
    }

    private void Update()
    {
        //Make the item float
        itemHolder.transform.localPosition = new Vector3(0, Mathf.Sin(Time.time * speed) * height + (isLocker ? 0 : 15), 0);
        itemHolder.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Generates the item that should spawn here.
    /// Ran by the master client only.
    /// </summary>
    public Item MGetItem()
    {
        //calculate spawn odds
        if (UnityEngine.Random.Range(0, 100) > Core.Instance.SpawnOdds * rarityModifier * 100 / (isLocker ? 15 : 1)) return null;

        //try 20 times to spawn here
        for (int i = 0; i < 20; i++)
        {
            Item rnd = ItemDatabase.Instance.weightedItems[Random.Range(0, ItemDatabase.Instance.weightedItems.Count)];
            //if anything can spawn, just pick the first
            if (bias == Bias.Anything) return rnd;

            //if bias should be forced, take first item if correct
            else if (forceBias && (int)rnd.type == (int)bias) return rnd;

            //bias is active but not forced, 60% chance to discard incorrect items
            else if ((int)rnd.type == (int)bias && Random.Range(0, 100) < 60) return rnd;
        }
        return null;
    }

    /// <summary>
    /// Changes this node to a specific item. If it's null, this node gets destroyed
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(Item item)
    {
        itemHolder.GetComponent<MeshRenderer>().enabled = false;
        if (gameObject.GetComponent<MeshRenderer>()) this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (item == null)
        {
            Destroy();
            return;
        }
        hasItem = true;
        this.item = item;
        if (!isLocker)
            ShowItem();
    }

    public void ShowItem()
    {
        Debug.Log("item set " + name);
        hasItem = true;
        GameObject itemobject = Instantiate(item.item);
        itemobject.transform.SetParent(itemHolder.transform);
        itemobject.transform.localPosition = Vector3.zero;
    }

    private void Destroy()
    {
        Destroy(particles);
        Destroy(light);
        Destroy(itemHolder);
        gameObject.tag = "Untagged";
        if (GetComponent<MeshRenderer>()) Destroy(GetComponent<MeshRenderer>());
        Destroy(this);
    }
}