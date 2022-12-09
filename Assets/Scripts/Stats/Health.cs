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
        bool isKO = false;
        bool isDead = false;

        private void Start()
        {
            health = maxHealth;
        }

        private void KnockOut()
        {
            if (isDead) return;
            if (isKO) return;
            Debug.Log("I am asleep");
            isKO = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            isKO = false;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
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
                KnockOut();
            }
            if (health <= -1)
            {
                Die();
            }
        }

        public void Heal()
        {
            health = maxHealth;
            isKO = false;
            isDead = false;
        }

        public bool GetKO()
        {
            return isKO;
        }

        public bool GetDead()
        {
            return isDead;
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
