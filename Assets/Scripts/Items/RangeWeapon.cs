using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RangeWeapon : MonoBehaviour
{
    [SerializeField] _Weapon weapon_stats;

    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
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
                target.TakeDamage(weapon_stats.maxDamage);
            }
        }
    }
}
