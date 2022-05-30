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
    public class NPCController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 2f;
        [SerializeField] float sprintSpeed = 8f;
        //[SerializeField] float rotSpeed = 100f;

        Health health;
        NPCMover mover;
        NPCFighter fighter;
        FieldOfView fov;
        GameObject player;

        // Wander Radius and Return
        [SerializeField] public bool canWander = false;
        Vector3 wanderCenter;
        [SerializeField] float width = 10f;
        [SerializeField] float length = 10f;
        Vector3 wanderTarget;
        float wanderTimer = Mathf.Infinity;

        // Idle
        [SerializeField] bool canIdle = false;
        Quaternion originalRot;

        // Patrol 
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float pathRadius = 1.4f;
        [SerializeField] float dwellTime = 10f;

        Vector3 npcPos;
        int currentWaypointIndex = 0;

        // Aggro
        [SerializeField] bool canAttack = false;
        [SerializeField] float peacefulPanicDuration = 5f;
        [SerializeField] float attackRadius = 4f;
        [SerializeField] float agroCoolDownTime = 1.8f;
        [SerializeField] float shoutDistance = 10f;
        [SerializeField] float chaseTimer = 2f;
        [SerializeField] Weapon foot;
        [SerializeField] float damage;
        bool finishedAlert = true;
        bool responceToCat = false;
        bool aggravate = false;
        [SerializeField] bool isWitness = false;
        float attackTimer = 1f;
        Vector3 lastKnownPos = Vector3.zero;
        [SerializeField] bool trespass = false;
        [SerializeField] Transform evacuationPoint;

        float timeOfPanic = 0f;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceLastSawPlayer = Mathf.Infinity; 
        float timeSinceAggravated = Mathf.Infinity;
        float timeSinceChase = Mathf.Infinity;

        private void Awake()
        {
            npcPos = transform.position;
            foot.damage = damage;
            wanderCenter = transform.position;
            wanderTarget = transform.position;
            health = GetComponent<Health>();
            fov = GetComponent<FieldOfView>();
            player = GameObject.FindGameObjectWithTag("Player");
            mover = GetComponent<NPCMover>();
            fighter = GetComponent<NPCFighter>();
            originalRot = transform.rotation;
        }

        private void Start()
        {
            fighter.InitFighter(attackRadius, damage);
        }


        void Update()
        {
            UpdateTimers();

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
                // Update patrol path and speed
            }

            if (isWitness && canAttack == false)
            {
                PeacefulPanic();
                return;
            }

            // New stuff
            if (fov.GetPlayerVisibility() && fov.GetCatForm())
            {
                UpdateSuspicion();
                return;
            }
            if (aggravate && canAttack)
            {
                if (lastKnownPos != Vector3.zero)
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

            if (aggravate && canAttack == false)
            {
                PeacefulPanic();
                return;
            }

            if (canWander && aggravate == false)
            {
                Wander();
                return;
            }
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
            if (witSet.evacPoint != null)
            {
                evacuationPoint = witSet.evacPoint;
            }    
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void UpdateSuspicion()
        {
            if (responceToCat == false) // initially aggroed
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
                responceToCat = true;
                timeSinceAggravated = 0f;
                finishedAlert = false;
                transform.LookAt(fov.getLastLocation());
                if (canAttack)
                {
                    GetComponent<Animator>().SetTrigger("agroAlert");
                }
                else
                {
                    GetComponent<Animator>().SetTrigger("friendlyAlert");
                }
            }
            if (timeSinceAggravated < agroCoolDownTime)
            {
                transform.LookAt(fov.getLastLocation());
            }

            if (timeSinceAggravated > agroCoolDownTime)
            {
                GetComponent<Animator>().ResetTrigger("agroAlert");
                GetComponent<Animator>().ResetTrigger("friendlyAlert");
                finishedAlert = true;
                aggravate = true;
            }

            if (finishedAlert && canAttack)
            {
                AttackBehaviour();
            }
            if (finishedAlert && canAttack == false)
            {
                RunAway();
            }
            if (canAttack == true)
            {
                lastKnownPos = player.transform.position;
            }
            else
            {
                lastKnownPos = evacuationPoint.position;
            }
            timeSinceLastSawPlayer = 0;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0f;
            fighter.Attack(player);
            //AggravateNearbyEnemies();
        }

        void AggravateNearbyEnemies()
        {
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

        private void UpdateTimers()
        {
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
            wanderTimer += Time.deltaTime;
            timeSinceChase -= Time.deltaTime;
            timeOfPanic += Time.deltaTime;
        }

        void Wander()
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

        void Idle()
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

        void PatrolBehaviour()
        {
            Vector3 nextPos = npcPos;

            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPos = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > dwellTime)
            {
                mover.StartMoveAction(nextPos, moveSpeed);
            }
        }

        private void PeacefulPanic()
        {
            if (AtTargetPoint(lastKnownPos))
            {
                mover.Cancel();
                if (timeOfPanic > peacefulPanicDuration)
                {
                    aggravate = false;
                    GetComponent<ActionScheduler>().CancelCurrentAction();
                }
                return;
            }
            timeOfPanic = 0f;
            GetComponent<ActionScheduler>().CancelCurrentAction();
            mover.StartMoveAction(lastKnownPos, sprintSpeed);
        }

        void RunAway()
        {
            timeOfPanic = 0f;
            lastKnownPos = evacuationPoint.position; // send npc to safe location. evacuationPoint.position
            PeacefulPanic();
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < pathRadius;
        }

        public bool AtTargetPoint(Vector3 target)
        {
            float distanceToPoint = Vector3.Distance(transform.position, target);
            return distanceToPoint < pathRadius;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(currentWaypointIndex);
        }

        void Hit()
        {
            foot.WeaponHit();
        }

        void StopHit()
        {
            foot.WeaponStopHit();
            timeSinceChase = chaseTimer;

            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public void Aggravate()
        {
            aggravate = true;
            SuspicionBehaviour();
        }
    }
}