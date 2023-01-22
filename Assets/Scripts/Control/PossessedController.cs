using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;
using gameracers.Movement;
using gameracers.Combat;
using UnityEngine.AI;
using UnityEngine.Animations;

namespace gameracers.Control
{ 
    public class PossessedController : MonoBehaviour
    {
        [SerializeField] RuntimeAnimatorController animController;
        [SerializeField] bool isPossessed = false;
        Transform spawnPoint;

        PossessMover mover;
        Health health;

        // Violence
        [SerializeField] float damage = 1f;
        [SerializeField] float attackMaxTimer = 1f;
        [SerializeField] Weapon foot;
        float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] bool isAttack = false;
        RotationConstraint rotCons;

        private void Awake()
        {
            spawnPoint = gameObject.transform.Find("SpawnPoint");
        }

        void Start()
        {
            GetComponent<Animator>().runtimeAnimatorController = animController;

            mover = GetComponent<PossessMover>();
            health = GetComponent<Health>();

            // temp stuff
            rotCons = transform.Find("Camera Points").GetComponent<RotationConstraint>();
            rotCons.rotationOffset = new Vector3(0, 180, 0);
        }

        void Update()
        {
            if (isPossessed != true) return;

            //if (Input.GetButtonDown("Fire1") && timeSinceLastAttack > attackMaxTimer)
            //{
            //    UpdateAttack();
            //}

            mover.UpdateMovement();

            timeSinceLastAttack += Time.deltaTime;
        }

        public void PossessState(bool val)
        {
            isPossessed = val;
            if (val == false)
            {
                GetComponent<Animator>().ResetTrigger("undie");
                GetComponent<Animator>().SetTrigger("die");
            }
            if (val == true)
            {
                GetComponent<Animator>().SetTrigger("undie");
            }
        }

        public Vector3 GetSpawnPoint()
        {
            return spawnPoint.position;
        }    

        // Animator actions
        void Hit()
        {

        }

        void StopHit()
        {
            isAttack = false;
        }
    }
}