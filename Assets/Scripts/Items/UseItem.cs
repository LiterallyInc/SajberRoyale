using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    [SerializeField] _Weapon weapon_stats;
    [SerializeField] _Melee melee_stats;
    [SerializeField] _Healing healing_stats;

    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (weapon_stats.type == Item.Type.Weapon)
                Shoot();
            else if (weapon_stats.type == Item.Type.Melee)
                Swing();
            else if (weapon_stats.type == Item.Type.Healing)
                Heal();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, weapon_stats.range))        {
            Debug.Log(hit.transform.name);

            EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
            if (target != null)
            {
                float result = weapon_stats.maxDamage - ((weapon_stats.maxDamage - weapon_stats.minDamage) / (weapon_stats.range - 1)) * (hit.distance - 1);
                result = Mathf.Round(result);
                HitMeDaddy(target.name, result, weapon_stats.name);
            }
        }
    }
    void Swing()
    {

    }
    void Heal()
    {

    }

    public static void HitMeDaddy(string player, float dmg, string weapon)
    {
        Debug.Log($"UwU!!! Daddy shot {player} for {dmg}hp with {weapon}!");
    }
}
