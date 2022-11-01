using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;

namespace gameracers.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] WeaponDataSO weapon;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Health>() != null)
            {
                other.gameObject.GetComponent<Health>().TakeDamage(weapon.damage);
            }
        }

        public WeaponDataSO GetWeapon()
        {
            return weapon;
        }

        // animator
        public void WeaponHit()
        {
            GetComponent<BoxCollider>().enabled = true;
        }

        public void WeaponStopHit()
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
