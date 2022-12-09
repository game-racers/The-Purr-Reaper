using gameracers.Appearances;
using gameracers.Architecture;
using gameracers.Combat;
using gameracers.Core;
using gameracers.Movement;
using gameracers.NPCStuff;
using gameracers.Stats;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using UnityEngine;

namespace gameracers.Control
{
    public class HumanBrain : MonoBehaviour
    {
        Health health;
        NPCMover mover;
        NPCFighter fighter;
        FieldOfView fov;
        GameObject player;

        [SerializeField] HumanDataSO humanClass;
        // Human Class variables
            // Movement
        float moveSpeed = 2f;
        float sprintSpeed = 8f;
            // Aggro
        bool canAttack = false;
        bool isGuard;

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
        [SerializeField] float shoutDistance = 10f;
        bool aggravate = false;
        Vector3 lastKnownPos;
        bool isWitness = false;
        [SerializeField] Transform evacPoint;
        Vector3 hidePoint = Vector3.zero;
        List<HidingSpot> floorSpots = new List<HidingSpot>();

        float demonCatSightTimer = -Mathf.Infinity;
        [SerializeField] float demonCatSightTimerMax = 10f;

        Vector3 humanPos;
        int currentWaypointIndex = 0;
        GameObject myBuilding;
        Floor myFloor;

        GameObject unconsciousBody = null;
        float wakeUpTime = -Mathf.Infinity;
        [SerializeField] float wakeUpDelay = 5f;

        bool hasBeenFound = false;
        float foundTimer;
        bool demonAlert = false;
        DemonAlertButton toButton;

        private void OnEnable()
        {
            EventListener.onCrossThreshold += CatEnteredBuilding;
            EventListener.onNewFloor += FloorChange;
            EventListener.onDemonButton += SetDemonAlert;
        }

        private void OnDisable()
        {
            EventListener.onCrossThreshold -= CatEnteredBuilding;
            EventListener.onNewFloor -= FloorChange;
            EventListener.onDemonButton -= SetDemonAlert;
        }

        private void CatEnteredBuilding(GameObject Entity, GameObject building, bool isEnter)
        {
            if (isEnter == false)
            {
                if (GameObject.ReferenceEquals(Entity, gameObject))
                {
                    myBuilding = null;
                    myFloor = null;
                }
                return;
            }

            if (GameObject.ReferenceEquals(Entity, player))
            {
                if (aggravate == true)
                {
                    if (GameObject.ReferenceEquals(building, myBuilding))
                    {
                        // return to hide pos
                    }
                    else
                    {
                        mover.StartMoveAction(evacPoint.transform.position, sprintSpeed);
                    }
                } 
            }
            
            if (GameObject.ReferenceEquals(Entity, gameObject))
            {
                myBuilding = building;
            }
        }

        private void FloorChange(GameObject entity, GameObject floor, bool isEnter)
        {
            if (!GameObject.ReferenceEquals(entity, gameObject)) return;
            if (isEnter == false)
            {
                if (floorSpots.Contains(floor.GetComponent<Floor>().GetHidingSpots()[0]))
                    floorSpots = new List<HidingSpot>();
            }

            if (isEnter == true)
            {
                myFloor = floor.GetComponent<Floor>();
                floorSpots = myFloor.GetHidingSpots();
            }
        }

        private void SetDemonAlert(int id)
        {
            // add timer implementation for slight randomness of preparedness in people. 
            Aggravate();
            demonAlert = true;
            toButton = null;
        }

        void Awake()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<NPCFighter>();
            mover = GetComponent<NPCMover>();
            fov = GetComponent<FieldOfView>();
            player = GameObject.FindGameObjectWithTag("Player");
            evacPoint = GameObject.Find("Evac Zone").transform;

            wanderCenter = transform.position;
            wanderTarget = transform.position;

            humanPos = transform.position;
            originalRot = transform.rotation;

            // Initialize HumanClass stuff
            GetComponent<ClothingEquip>().initClothes(humanClass.hats, humanClass.shirts, humanClass.pants, humanClass.ties);
            health.SetHealth(humanClass.health);
            moveSpeed = humanClass.moveSpeed;
            sprintSpeed = humanClass.sprintSpd;
            canAttack = humanClass.canAttack;
            isGuard = humanClass.isGuard;
        }

        void Update()
        {
            if (health.GetDead())
            {
                if (hasBeenFound) return;
                if (foundTimer == 0f) return;
                if (Time.time - foundTimer > 60f) hasBeenFound = true;
                return;
            }

            if (health.GetKO())
            {
                return;
            }

            if (Time.time - wakeUpTime < wakeUpDelay)
                return;

            fov.FieldOfViewCheck();

            // Passive
            if (aggravate == false)
            {
                CheckForDead();

                // Human sees demonic cat
                if (fov.GetPlayerVisibility() && fov.GetCatForm())
                {
                    SetWitness();
                    UpdateSuspicion();
                    return;
                }

                Work();
                return;
            }

            if (aggravate == true)
            {

                // Panic
                if (isGuard == false)
                {
                    if (canAttack == true)
                    {
                        if (fov.GetPlayerVisibility() && fov.GetCatForm())
                        {
                            UpdateSuspicion();
                            GuardAggro();
                            return;
                        }
                        else
                        {
                            SetWitness();
                            if (demonAlert == false)
                            {
                                PullDemonSwitch();

                            }
                            if (toButton != null) return;
                            PeacefulPanic();
                            return;
                        }
                    }
                    else
                    {
                        if (demonAlert == false)
                        {
                            PullDemonSwitch();

                        }
                        if (toButton != null) return;
                        PeacefulPanic();
                    }

                    return;
                }

                if (isGuard == true)
                {
                    if (fov.GetPlayerVisibility() && fov.GetCatForm())
                    {
                        UpdateSuspicion();
                        fov.angle = 360f;
                        GuardAggro();
                        return;
                    }

                    if (Time.time - demonCatSightTimer < demonCatSightTimerMax)
                    {
                        Wander();
                    }
                    else
                    {
                        if (fov.CanSeeKO())
                        {
                            if (unconsciousBody == null)
                                unconsciousBody = fov.GetBody();

                            if (unconsciousBody != null)
                                mover.StartMoveAction(unconsciousBody.transform.position, sprintSpeed);

                            if (AtTargetPoint(unconsciousBody.transform.position))
                            {
                                // trigger animation
                                unconsciousBody.GetComponent<HumanBrain>().Heal();
                                unconsciousBody = null;
                            }
                            return;
                        }
                        if (fov.CanSeeDead() && !fov.IsAccounted())
                        {
                            SetWanderArea(transform.position);
                            Wander();
                        }
                        if (!fov.GetPlayerVisibility() || !fov.GetCatForm())
                        {
                            SetWitness();
                            Work();
                        }
                    }
                }
            }
        }

        private void CheckForDead()
        {
            if (fov.CanSeeKO())
            {
                Aggravate();
                AggravateNearbyEnemies();
                moveSpeed = sprintSpeed;
                GetComponent<ActionScheduler>().CancelCurrentAction();
                Debug.Log(unconsciousBody);
                if (unconsciousBody != null)
                    mover.StartMoveAction(unconsciousBody.transform.position, sprintSpeed);
            }

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
            wanderCenter = lastKnownPos;
            wanderTarget = lastKnownPos;
            aggravate = true;
            demonCatSightTimer = Time.time;
        }

        private void Work()
        {
            if (patrolPath != null)
            {
                PatrolBehaviour();
                return;
            }
            if (canWander == true)
            {
                Wander();
                return;
            }
            if (isGuard == true)
            {
                GuardingIdle();
                return;
            }
        }

        private void SetWanderArea(Vector3 center)
        {
            wanderTarget = center;
            wanderCenter = center;
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

        private void Wander()
        {
            if (AtTargetPoint(wanderTarget) == false)
            {
                wanderTimer = Time.time;
            }

            if (Time.time - wanderTimer > dwellTime)
            {
                float wanderx = wanderCenter.x + Random.Range(-width, width);
                float wanderz = wanderCenter.z + Random.Range(-length, length);
                wanderTarget = new Vector3(wanderx, transform.position.y, wanderz);

                GetComponent<ActionScheduler>().CancelCurrentAction();
                mover.StartMoveAction(wanderTarget, moveSpeed);
                wanderTimer = 0;
            }
        }

        private void GuardingIdle()
        {
            if (AtTargetPoint(wanderCenter) == false)
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
                mover.StartMoveAction(wanderCenter, moveSpeed);
            }
            else if (transform.rotation != originalRot)
            {
                transform.rotation = originalRot;
            }
        }

        private void PeacefulPanic()
        {
            if (AtTargetPoint(evacPoint.position))
            {
                Destroy(gameObject);
                return;
            }

            

            if (hidePoint != null)
            {
                if (AtTargetPoint(hidePoint))
                {
                    /* if (first floor) Exit by window, door, doesnt matter, exit!
                     * 
                     */

                    
                    fov.angle = 360f;
                    if (fov.GetPlayerVisibility() && fov.GetCatForm())
                    {
                        fighter.Attack(player);
                        if (player.transform.position.z - hidePoint.z > 5f || Vector3.Distance(player.transform.position, hidePoint) > 20f)
                        {
                            mover.StartMoveAction(hidePoint, sprintSpeed);
                        }
                    }
                    else
                        mover.StartMoveAction(hidePoint, sprintSpeed);
                    return;
                }
            }

            // Run away from cat!
            float distToCat = Vector3.Distance(transform.position, player.transform.position);
            if (Mathf.Abs(transform.position.y - player.transform.position.y) < 4 && distToCat < 10f)
            {
                // Might be unnecessary
                //if (floorSpots.Count == 0)
                //{
                //    Debug.Log("just flippin' run away");
                //    Vector3 runAwayPoint = new Vector3(transform.position.x - player.transform.position.x, transform.position.y, transform.position.z - player.transform.position.z);
                //    mover.StartMoveAction(runAwayPoint, sprintSpeed);
                //    hidePoint = Vector3.zero;
                //    return;
                //}

                Vector2 humanPos = new Vector2(transform.position.x, transform.position.z);
                Vector2 catPos = new Vector2(player.transform.position.x, player.transform.position.z);
                Vector3 tempHidePoint = new Vector3();
                foreach (HidingSpot spot in floorSpots)
                {
                    Vector2 hitPos = new Vector2(spot.transform.position.x, spot.transform.position.z);
                    if (Vector2.Angle(new Vector2(0,0), new Vector2(0,0)) > 90f)
                    {
                        tempHidePoint = spot.GetPoint();
                        hidePoint = tempHidePoint;
                        mover.StartMoveAction(tempHidePoint, sprintSpeed);
                        return;
                    }
                }

                if (tempHidePoint == new Vector3())
                {
                    Vector3 runAwayPoint = new Vector3(transform.position.x - player.transform.position.x, transform.position.y, transform.position.z - player.transform.position.z);
                    mover.StartMoveAction(runAwayPoint, sprintSpeed);
                    hidePoint = Vector3.zero;
                    return;
                }

                /* if (pov.SeesStairs())
                 *      if (stairs.downstairs)
                 *          StartMoveAction(stairs.GetPoint)
                 *  
                 *  if (pov.SeesExit())
                 *      StartMoveAction(exit)
                 */

                return;
            }

            // go to nearest safe room that isnt full
            if (hidePoint == Vector3.zero)
            {
                if (floorSpots.Count == 0)
                {
                    mover.StartMoveAction(evacPoint.position, sprintSpeed);
                    hidePoint = Vector3.up * -100f;
                    return;
                }

                Vector3 tempHidePoint = floorSpots[0].GetPoint();
                float distToHide = Vector3.Distance(transform.position, floorSpots[0].transform.position);
                float tempDist;

                for (int i = 1; i < floorSpots.Count; i++)
                {
                    tempDist = Vector3.Distance(floorSpots[i].transform.position, transform.position);
                    if (tempDist < distToHide)
                    {
                        tempHidePoint = floorSpots[i].GetPoint();
                        distToHide = tempDist; 
                    }
                }

                // Go to safe room
                hidePoint = tempHidePoint;
                mover.StartMoveAction(hidePoint, sprintSpeed);
            }
        }

        private void PullDemonSwitch()
        {
            if (myFloor.GetFloorID() != 0)
            {
                if (toButton != null)
                {
                    if (AtTargetPoint(toButton.transform.position))
                    {
                        //GetComponent<Animator>().SetTrigger("pullSwitch");
                        //Set Collider on pull animation to test collision for buttons. Make collider huuuuuuge
                        EventListener.DemonButton(toButton.GetButtonID());
                        toButton = null;
                    }
                    return;
                }

                List<DemonAlertButton> demonButtons = myFloor.GetDemonButtons();
                if (demonButtons.Count == 0)
                {
                    return;
                }

                toButton = demonButtons[0];
                float toButtonDist = Vector3.Distance(transform.position, toButton.transform.position);
                float tempDist;

                for (int i = 1; i < demonButtons.Count; i++)
                {
                    tempDist = Vector3.Distance(transform.position, demonButtons[i].transform.position);
                    if (tempDist < toButtonDist)
                    {
                        toButton = demonButtons[i];
                        toButtonDist = tempDist;
                    }
                }
                mover.StartMoveAction(toButton.transform.position, sprintSpeed);
            }
        }

        private void GuardAggro()
        {
            if (lastKnownPos != null)
            {
                if (!AtTargetPoint(lastKnownPos))
                {
                    mover.StartMoveAction(lastKnownPos, sprintSpeed);
                }
                else
                {
                    fighter.Attack(player);
                }
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
                if (hit.collider.GetComponent<HumanBrain>() != null)
                {
                    HumanBrain ai = hit.collider.GetComponent<HumanBrain>();
                    if (ai.enabled == false) continue;
                    if (ai.GetComponent<Health>().GetDead() == true) continue;

                    ai.Aggravate();
                }
            }
        }

        // Animator Events
        void Hit()
        {

        }

        void StopHit()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public void Aggravate()
        { 
            aggravate = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        //public void Hide(Vector3 hidePos)
        //{
        //    hidePoint = hidePos;
        //    GetComponent<ActionScheduler>().CancelCurrentAction();
        //    mover.StartMoveAction(hidePos, sprintSpeed);
        //}

        public void SetAttack(bool defendOneself)
        {
            if (defendOneself == true)
            {
                canAttack = true;
                return;
            }
            if (defendOneself == false)
            {
                canAttack = humanClass.canAttack;
            }
        }

        public void TriggerFound()
        {
            if (hasBeenFound == true) return;

            foundTimer = Time.time;
        }

        public void TriggerFoundKO(GameObject body)
        {
            unconsciousBody = body;
        }

        public bool GetFound()
        {
            return hasBeenFound;
        }

        public void Heal()
        {
            if (health.GetKO())
                GetComponent<Animator>().SetTrigger("awake");
            health.Heal();
            wakeUpTime = Time.time;
        }
    }
}
