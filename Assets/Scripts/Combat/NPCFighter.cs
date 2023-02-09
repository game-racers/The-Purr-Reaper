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
        float timeSinceLastAttack = -5f;
        [SerializeField] WeaponDataSO weaponData;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform weaponHolster;
        Weapon heldWeapon;
        Weapon holsteredWeapon;
        float weaponRange = 4f;

        private void Start()
        {
            weaponRange = weaponData.weaponRange;
            heldWeapon = Spawn(weaponData.equipPrefab, rightHandTransform, false);
            holsteredWeapon = Spawn(weaponData.equipPrefab, weaponHolster, true);
            heldWeapon.tag = gameObject.tag;
        }

        void UpdateAttack()
        {
            if (Time.time - timeSinceLastAttack < timeBetweenAttacks) return;

            if (target != null)
            {
                if (target.GetDead()) return;

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

        private Weapon Spawn(GameObject weapon, Transform location, bool visible)
        {
            GameObject spawnnedWeapon = Instantiate(weapon, location);
            spawnnedWeapon.SetActive(visible);
            return spawnnedWeapon.GetComponent<Weapon>();
        }

        private void AttackBehaviour()
        {

            if (Time.time - timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = Time.time;
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
            heldWeapon.WeaponHit();
        }

        void StopHit()
        {
            heldWeapon.WeaponStopHit();
        }

        void DrawWeapon()
        {
            heldWeapon.gameObject.SetActive(true);
            holsteredWeapon.gameObject.SetActive(false);
            GetComponent<Animator>().SetBool("agro", false);
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
            if (heldWeapon.gameObject.activeSelf == false)
            {
                GetComponent<Animator>().SetBool("agro", true);
            }
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            UpdateAttack();
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
            //StopHit();
        }
    }
}
