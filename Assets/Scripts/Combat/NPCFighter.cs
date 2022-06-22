using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;
using gameracers.Movement;
using gameracers.Core;

namespace gameracers.Combat
{
    public class NPCFighter : MonoBehaviour, IAction
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] Weapon foot;
        float weaponRange = 4f;
        float damage = 1f;

        public void InitFighter(float range, float damage)
        {
            this.weaponRange = range;
            this.damage = damage;
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target != null)
            {
                if (target.GetDead()) return;
                //problem
                if (!IsInRange(target.gameObject.transform))
                {
                    GetComponent<NPCMover>().MoveTo(target.transform.position, 8f);
                }
                else
                {
                    GetComponent<NPCMover>().Cancel();
                    AttackBehaviour();
                }
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().SetTrigger("attack");
            GetComponent<Animator>().ResetTrigger("stopAttack");
        }

        // Animation event
        void Hit()
        {
            foot.WeaponHit();
        }

        void StopHit()
        {
            foot.WeaponStopHit();
        }

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!GetComponent<NPCMover>().CanMoveTo(combatTarget.transform.position) && !IsInRange(combatTarget.transform))
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.GetDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void EndAttack()
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = null;
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<NPCMover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            StopHit();
        }
    }
}
