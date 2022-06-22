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
        FieldOfView fov;
        GameObject player;

        // Movement
        [SerializeField] float moveSpeed = 2f;
        [SerializeField] float sprintSpeed = 8f;

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
        float arrivalTime;

        // Aggro
        [SerializeField] bool canAttack = false;
        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damage = 1f;
        [SerializeField] float shoutDistance = 10f;
        bool aggravate = false;
        float chaseTime;
        Vector3 lastKnownPos;
        bool isWitness = false;
        [SerializeField] Transform evacPoint;

        float aggroTimer;

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
            if (fov.CanSeeDead() && isWitness == false)
            {
                Aggravate();
                AggravateNearbyEnemies();
                moveSpeed = sprintSpeed;
                isWitness = true;
                if (GetComponent<NPCWitness>() != null)
                {
                    SetWitness();
                }
            }

            // fov can see player && cat is demon
            if (fov.GetPlayerVisibility() && fov.GetCatForm())
            {
                UpdateSuspicion();
                return;
            }

            //// Witness and can not Attack
            //if (isWitness && canAttack == false)
            //{
            //    PeacefulPanic();
            //    return;
            //}

            // is aggravate && can attack
            if (aggravate && canAttack)
            {
                if (lastKnownPos != null)
                {
                    GetComponent<ActionScheduler>().CancelCurrentAction();
                    mover.StartMoveAction(lastKnownPos, sprintSpeed);
                    if (AtTargetPoint(lastKnownPos))
                    {
                        aggravate = false;
                        GetComponent<ActionScheduler>().CancelCurrentAction();
                        mover.StartMoveAction(wanderTarget, moveSpeed);
                    }
                }
            }

            // is aggravate && can not attack
            if (aggravate && canAttack == false)
            {
                PeacefulPanic();
                return;
            }

            // can wander && not aggravate
            if (canWander && aggravate == false)
            {
                Wander();
                return;
            }

            // can idle && not aggravate
            if (canIdle && aggravate == false)
            {
                Idle();
                return;
            }

            PatrolBehaviour();
        }

        private void SetWitness()
        {
            NPCWitness witSet = GetComponent<NPCWitness>();
            if (witSet.newPath != null)
            {
                patrolPath = witSet.newPath;
            }
            if (witSet.newIdle != null)
            {
                wanderCenter = witSet.newIdle.position;
            }
            fov.radius = witSet.newFOVRadius;
            fov.angle = witSet.newAngle;
            canWander = witSet.newCanWander;
            moveSpeed = witSet.moveSpeed;
            if (witSet.evacPoint != null)
            {
                evacPoint = witSet.evacPoint;
            }
        }

        private void UpdateSuspicion()
        {
            lastKnownPos = player.transform.position;
            aggravate = true;
            aggroTimer = Time.time;

            if (canAttack)
            {
                AttackBehaviour();
            }
            if (canAttack == false)
            {
                PeacefulPanic();
            }
        }

        private void AttackBehaviour()
        {
            aggroTimer = 0f;
            fighter.Attack(player);
        }

        private void PeacefulPanic()
        {
            /*
             * Summary:
             *      This method decides the best location for the Human to retreat to. 
             *      Wether that be inside a building or to the maps edge, it goes there if the Cat is not in the way. 
             */

            // Test if path to evac collides with Cat
            //      Run away from Cat and head towards an safe space/evac, meaning more calculations to see if one leads into Cat
            // Set that as the evac point or use the default one and go there until you are there.  Possibly pick a random point in that room from the room evac point. 
            // If outside, pick the nearest evac point and go there, could be inside but if far enough away, run away for good. 

            Debug.Log("Peaceful Panic is currently not written");
        }

        private void Wander()
        {
            if (AtTargetPoint(wanderTarget) == false)
            {
                wanderTimer = 0;
            }

            if (wanderTimer > dwellTime)
            {
                float wanderx = wanderCenter.x + Random.Range(-width, width);
                float wanderz = wanderCenter.z + Random.Range(-length, length);
                wanderTarget = new Vector3(wanderx, transform.position.y, wanderz);

                mover.StartMoveAction(wanderTarget, moveSpeed);
                wanderTimer = 0;
            }
        }

        private void Idle()
        {
            if (AtTargetPoint(wanderCenter) == false)
            {
                mover.StartMoveAction(wanderCenter, 2f);
            }
            else if (transform.rotation != originalRot)
            {
                transform.rotation = originalRot;
            }
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPos = humanPos;

            if (patrolPath != null)
            {
                if (AtTargetPoint(GetCurrentWaypoint()))
                {
                    arrivalTime = Time.time;
                    CycleWaypoint();
                }
                nextPos = GetCurrentWaypoint();
            }

            if (Time.time - arrivalTime > dwellTime)
            {
                mover.StartMoveAction(nextPos, moveSpeed);
            }
        }

        private bool AtTargetPoint(Vector3 target)
        {
            float distanceToPoint = Vector3.Distance(transform.position, target);
            return distanceToPoint < pathRadius;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void AggravateNearbyEnemies()
        {
            // figure out how to make sphere less vertical
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<NPCController>() != null)
                {
                    NPCController ai = hit.collider.GetComponent<NPCController>();
                    if (ai.enabled == false) continue;
                    if (ai.GetComponent<Health>().GetDead() == true) continue;

                    ai.Aggravate();
                }
            }
        }

        // Animator Events
        void Hit()
        {
            //foot.WeaponHit();
        }

        void StopHit()
        {
            // foot.WeaponStopHit();
            chaseTime = Time.time;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public void Aggravate()
        {
            aggravate = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}