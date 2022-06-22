using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Movement;
using gameracers.Stats;
using gameracers.NPCStuff;
using gameracers.Core;
using gameracers.Combat;

namespace gameracers.Control
{
    public class HumanController : MonoBehaviour
    {
        Health health;
        NPCMover mover;
        NPCFighter fighter;
        //HumanBehavior brain;
        FieldOfView fov;
        GameObject player;

        // Wander Radius and Return
        [SerializeField] public bool canWander = false;
        [SerializeField] float width = 10f;
        [SerializeField] float length = 10f;
        Vector3 wanderCenter;
        Vector3 wanderTarget;
        float wanderTimer;

        // Idle
        [SerializeField] bool canIdle = false;
        Quaternion originalRot;

        // Patrol 
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float pathRadius = 1.4f;
        [SerializeField] float dwellTime = 10f;

        // Aggro
        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damage = 1f;

        Vector3 humanPos;
        int currentWaypointIndex = 0;

        void Awake()
        {
            humanPos = transform.position;
            health = GetComponent<Health>();
            fighter = GetComponent<NPCFighter>();
            mover = GetComponent<NPCMover>();
            fov = GetComponent<FieldOfView>();
            player = GameObject.FindGameObjectWithTag("Player");

            wanderCenter = transform.position;
            wanderTarget = transform.position;

            humanPos = transform.position;
            originalRot = transform.rotation;
        }

        private void Start()
        {
            fighter.InitFighter(attackRadius, damage);
        }

        void Update()
        {
            if (health.GetDead()) return;

            fov.FieldOfViewCheck();

            // Stumbles upon the Dead

            // Witness and Can Attack
        }
    }

}