using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Core;

namespace gameracers.Stats
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 1f;
        [SerializeField] float health;
        bool isDead = false;
        bool isVeryDead = false;

        private void Start()
        {
            health = maxHealth;
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void VeryDead()
        {
            if (isVeryDead) return;

            isVeryDead = true;
            GetComponent<Animator>().SetTrigger("die");
        }

        public void SetHealth(float newHealth)
        {
            maxHealth = newHealth;
            health = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            if (health <= -1f)
            {
                VeryDead();
            }
        }

        public void Heal()
        {
            health = maxHealth;
            isDead = false;
        }

        public bool GetDead()
        {
            return isDead;
        }

        public bool GetVeryDead()
        {
            return isVeryDead;
        }

        public float GetHealthPoints()
        {
            return health;
        }

        public float GetMaxHP()
        {
            return maxHealth;
        }
    }
}
