using System.Collections;
using UnityEngine;

namespace SajberRoyale.Items
{
    [System.Obsolete]
    public class UseItem : MonoBehaviour
    {
        [SerializeField] private Weapon weapon_stats;
        [SerializeField] private _Melee melee_stats;
        [SerializeField] private Healing healing_stats;

        public Camera fpsCam;

        private int tempClipSize;

        private void Start()
        {
            tempClipSize = weapon_stats.clipSize;
            weapon_stats.canShoot = true;
            melee_stats.onCooldown = false;
        }

        // Update is called once per frame
        private void Update()
        {
            // Checking if _Weapon script is available, shoot cooldown is off, and its not reloading
            if (weapon_stats != null && weapon_stats.canShoot && weapon_stats.isReloading == false)
            {
                if (weapon_stats.isAuto == true)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        StartCoroutine(ShootDelay(weapon_stats.shootingDelay));
                        Shoot();
                    }
                }
                else if (weapon_stats.isAuto == false)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        StartCoroutine(ShootDelay(weapon_stats.shootingDelay));
                        Shoot();
                    }
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (melee_stats != null && melee_stats.onCooldown == false)
                {
                    Slash();
                }
                else if (healing_stats != null)
                    Heal();
            }
        }

        private void Shoot()
        {
            if (tempClipSize <= 0)
            {
                weapon_stats.isReloading = true;
                StartCoroutine(ReloadTimer(weapon_stats.reloadTime));
            }
            else tempClipSize -= 1;

            AudioSource.PlayClipAtPoint(weapon_stats.shootSFX, gameObject.transform.position);

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, weapon_stats.range))
            {
                Debug.Log(hit.transform.name);

                /*EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
                if (target != null)
                {
                    float result = weapon_stats.maxDamage - ((weapon_stats.maxDamage - weapon_stats.minDamage) / (weapon_stats.range - 1)) * (hit.distance - 1);
                    result = Mathf.Round(result);

                    HitMeDaddy(target.name, result, weapon_stats.name);
                }*/
            }
        }

        private void Slash()
        {
            melee_stats.onCooldown = true;

            AudioSource.PlayClipAtPoint(melee_stats.swingSFW, gameObject.transform.position);

            StartCoroutine(SlashCooldown());

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, melee_stats.range))
            {
                Debug.Log(hit.transform.name);

                /*EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
                if (target != null)
                {
                    AudioSource.PlayClipAtPoint(melee_stats.hitSFX, gameObject.transform.position);

                    float result = UnityEngine.Random.Range(melee_stats.minDamage, melee_stats.maxDamage);
                    result = Mathf.Round(result);

                    StabMeDaddy(target.name, result, melee_stats.name);
                }*/
            }
        }

        private void Heal()
        {
        }

        public static void HitMeDaddy(string player, float dmg, string weapon)
        {
            Debug.Log($"UwU!!! Daddy shot {player} for {dmg}hp with {weapon}!");
        }

        public static void StabMeDaddy(string player, float dmg, string weapon)
        {
            Debug.Log($"UWU I stabbed {player} for {dmg}hp with {weapon}!");
        }

        private IEnumerator SlashCooldown()
        {
            // 1 sec cooldown for test
            yield return new WaitForSeconds(1);
            melee_stats.onCooldown = false;
        }

        private IEnumerator ShootDelay(float delay)
        {
            //Debug.Log(Time.time);
            weapon_stats.canShoot = false;
            yield return new WaitForSeconds(delay);
            weapon_stats.canShoot = true;
            //Debug.Log(Time.time);
        }

        private IEnumerator ReloadTimer(float reloading)
        {
            Debug.Log(Time.time);
            yield return new WaitForSeconds(reloading);
            tempClipSize = weapon_stats.clipSize;
            weapon_stats.isReloading = false;
            Debug.Log(Time.time);
        }
    }
}