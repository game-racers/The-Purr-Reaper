using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Combat
{
    public class PlayerFighter : MonoBehaviour
    {
        [SerializeField] float attackDuration = 1f;
        [SerializeField] Weapon scythe;
        float timeSinceLastAttack;

        void Start()
        {
            timeSinceLastAttack = Time.time;
        }

        // Animator events
        void Hit()
        {
            scythe.WeaponHit();
        }

        void StopHit()
        {
            scythe.WeaponStopHit();
        }

        public void AttackBehavior()
        {
            if (Time.time - timeSinceLastAttack > attackDuration)
            {
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = Time.time;
            }
        }
    }
}
