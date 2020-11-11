using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

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
    public bool hasItem;


    public enum Bias
    {
        Anything = 0,
        Weapon = 1,
        Ammo = 2,
        Healing = 3
    }
    void Start()
    {
        //itemHolder.GetComponent<Renderer>().material.color = Color.clear;
        //GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        //Move the item
        itemHolder.transform.localPosition = new Vector3(0, Mathf.Sin(Time.time * speed)*height+15, 0);
        itemHolder.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 10f);
    }
}
