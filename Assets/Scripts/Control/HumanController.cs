using gameracers.Appearances;
using gameracers.Architecture;
using gameracers.Combat;
using gameracers.Core;
using gameracers.Movement;
using gameracers.NPCStuff;
using gameracers.Stats;
using gameracers.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace gameracers.Control
{
    public class HumanController : MonoBehaviour
    {
        // Default is Civilian
        Health health;
        NPCMover mover;
        NPCFighter fighter;
        FieldOfView fov;
        GameObject player;
        [SerializeField] HuumanState state = HuumanState.Work;

        [SerializeField] HumanDataSO humanClass;
        // Human Class variables
        // Movement
        float moveSpeed = 2f;
        float sprintSpeed = 8f;
        bool canAttack = false;

        // Wander and Patrol
        [SerializeField] public bool canWander = false;
        [SerializeField] float width = 10f;
        [SerializeField] float length = 10f;
        [SerializeField] float dwellTime = 10f;
        // Patrol 
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float pathRadius = 1.4f;
        int currentWaypointIndex = 0;

        // Aggro
        [SerializeField] float shoutDistance = 10f;
        bool aggravate = false;
        bool isWitness = false;
        Transform evacPoint;
        List<HidingSpot> floorSpots = new List<HidingSpot>();
        [SerializeField] float SusTimer = 30f;
        float reactionTime = 3f;

        GameObject myBuilding;
        Floor myFloor;

        GameObject unconsciousBody = null;
        [SerializeField] float wakeUpDelay = 5f;

        bool hasBeenFound = false;
        DemonAlertButton toButton;
        bool demonAlert;
        bool knowsCatsAlt = false;

        // Idle
        Quaternion originalRot;

        // Vector3s
        Vector3 wanderCenter;
        Vector3 targetPos;
        Vector3 hidePoint = Vector3.zero;
        Vector3 lastKnownPos;

        // Timers
        float mainTimer;
        float secondTimer;


        bool isHiding;

        private void OnEnable()
        {
            EventListener.onCrossThreshold += CatEnteredBuilding;
            EventListener.onNewFloor += FloorChange;
        }

        private void OnDisable()
        {
            EventListener.onCrossThreshold -= CatEnteredBuilding;
            EventListener.onNewFloor -= FloorChange;
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
                        SetState(HuumanState.Evacuate);
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

        private void Awake()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<NPCFighter>();
            mover = GetComponent<NPCMover>();
            fov = GetComponent<FieldOfView>();
            player = GameObject.FindGameObjectWithTag("Player");

            wanderCenter = transform.position;
            targetPos = transform.position;
            originalRot = transform.rotation;

            # region Human Class stats initialization
            GetComponent<ClothingEquip>().initClothes(humanClass.hats, humanClass.shirts, humanClass.pants, humanClass.ties);
            health.SetHealth(humanClass.health);
            moveSpeed = humanClass.moveSpeed;
            sprintSpeed = humanClass.sprintSpd;
            canAttack = humanClass.canAttack;
            reactionTime = humanClass.reactionTime;
            #endregion
        }

        private void Update()
        {
            if (health.GetDead())
            {
                Dead();
                return;
            }

            if (health.GetKO())
            {
                KOed();
                return;
            }

            fov.FieldOfViewCheck();

            if (Time.time - secondTimer < reactionTime) return;

            CheckForBodies();

            CanSeeDemonCat();

            switch (state)
            {
                case HuumanState.Work:
                    Work();
                    break;
                case HuumanState.Suspicious:
                    Suspicious();
                    break;
                case HuumanState.Alert:
                    Alert();
                    break;
                case HuumanState.Help:
                    HelpThem();
                    break;
                case HuumanState.SeesCat:
                    Panic();
                    break;
                case HuumanState.Evacuate:
                    CalculateEvac();
                    break;
                case HuumanState.KO:
                    if (Time.time - mainTimer >= wakeUpDelay)
                        SetState(HuumanState.Alert);
                    return;
                case HuumanState.Dead:
                    Dead();
                    return;
            }
        }

        #region Generic Functions
        private void CanSeeDemonCat()
        {
            if (fov.GetPlayerVisibility() && (fov.GetCatForm() || knowsCatsAlt))
            {
                lastKnownPos = player.transform.position; 
                SetWitness();
                UpdateSuspicion();
                Aggravate();
                AggravateNearbyEnemies();
                SetState(HuumanState.SeesCat);
                return;
            }
        }

        private void CheckForBodies()
        {
            if (fov.CanSeeKO())
            {
                if (unconsciousBody == null)
                    unconsciousBody = fov.GetBody();
                if (unconsciousBody.GetComponent<HumanController>().GetFound())
                    return;
                Aggravate();
                AggravateNearbyEnemies();
                moveSpeed = sprintSpeed;
                GetComponent<ActionScheduler>().CancelCurrentAction();
                SetState(HuumanState.Help);
            }

            if (fov.CanSeeDead() && isWitness == false)
            {
                if (unconsciousBody == null)
                    unconsciousBody = fov.GetBody();
                if (unconsciousBody.GetComponent<HumanController>().GetFound())
                    return;
                Aggravate();
                AggravateNearbyEnemies();
                moveSpeed = sprintSpeed;
                isWitness = true;
                SetState(HuumanState.Help);
                if (GetComponent<NPCWitness>() != null)
                {
                    SetWitness();
                }
            }
        }

        private void UpdateSuspicion()
        {
            targetPos = transform.position;
            Aggravate();
            AggravateNearbyEnemies();
            mainTimer = Time.time;
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

        private void AggravateNearbyEnemies()
        {
            // figure out how to make sphere less vertical
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<HumanController>() != null)
                {
                    HumanController ai = hit.collider.GetComponent<HumanController>();
                    if (ai.enabled == false) continue;
                    if (ai.GetComponent<Health>().GetDead() == true) continue;
                    if (ai.GetAggravate()) continue;

                    ai.Aggravate();
                }
            }
        }
        #endregion

        #region Work Functions
        private void Work()
        {
            if (patrolPath != null)
            {
                PatrolBehaviour();
                return;
            }
            else if (canWander == true)
            {
                Wander();
                return;
            }
            else
            {
                Idle();
                return;
            }
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPos = transform.position;

            if (patrolPath != null)
            {
                if (AtTargetPoint(GetCurrentWaypoint()))
                {
                    mainTimer = Time.time;
                    //oneTimer = Time.time;
                    CycleWaypoint();
                }
                nextPos = GetCurrentWaypoint();
            }

            if (Time.time - mainTimer > dwellTime)
            {
                mover.StartMoveAction(nextPos, moveSpeed);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtTargetPoint(Vector3 target)
        {
            float distanceToPoint = Vector3.Distance(transform.position, target);
            return distanceToPoint < pathRadius;
        }

        private void Wander()
        {
            if (AtTargetPoint(targetPos) == false)
            {
                mainTimer = Time.time;
            }

            if (Time.time - mainTimer > dwellTime)
            {
                NavMeshHit hit;
                targetPos = Vector3.zero;
                while (targetPos == Vector3.zero)
                {
                    float wanderx = wanderCenter.x + Random.Range(-width, width);
                    float wanderz = wanderCenter.z + Random.Range(-length, length);
                    if (NavMesh.SamplePosition(new Vector3(wanderx, transform.position.y, wanderz), out hit, 1f, NavMesh.AllAreas))
                    {
                        NavMeshPath path = new NavMeshPath();
                        if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                            targetPos = path.corners[path.corners.Length - 1];
                    }
                }

                mover.StartMoveAction(targetPos, moveSpeed);
                mainTimer = 0;
            }
        }

        private void SetWanderArea(Vector3 center)
        {
            targetPos = center;
            wanderCenter = center;
        }

        private void Idle()
        {
            if (AtTargetPoint(wanderCenter) == false)
            {
                mover.StartMoveAction(wanderCenter, moveSpeed);
            }
            else if (transform.rotation != originalRot)
            {
                transform.rotation = originalRot;
            }
        }
        #endregion

        #region Suspicious Functions
        private void Suspicious()
        {
            // Do a double take, aka look at last known pos, and have a bar slowly fill up. Move closer after double take to investigate. The duration of suspicion is based on how full the bar gets. 
            Wander();
            if (Time.time - mainTimer > SusTimer)
                SetState(HuumanState.Work);
        }
        #endregion

        #region Alert Functions
        private void Alert()
        {
            if (myBuilding.GetComponent<Building>().GetPanic() == false)
            {
                PullDemonSwitch();
                return;
            }
            SetState(HuumanState.SeesCat);
        }

        private void PullDemonSwitch()
        {
            if (myFloor == null) return;
            if (myFloor.GetFloorID() != 0)
            {
                if (toButton != null)
                {
                    if (AtTargetPoint(toButton.transform.position))
                    {
                        //GetComponent<Animator>().SetTrigger("pullSwitch");
                        //Set Collider on pull animation to test collision for buttons. Make collider huuuuuuge
                        EventListener.DemonButton(toButton.GetButtonID());
                        demonAlert = true;
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
        #endregion

        #region Help Them Functions
        private void HelpThem()
        {
            if (unconsciousBody == null)
                unconsciousBody = fov.GetBody();

            if (unconsciousBody != null)
                mover.StartMoveAction(unconsciousBody.transform.position, sprintSpeed);

            if (AtTargetPoint(unconsciousBody.transform.position))
            {
                if (unconsciousBody.GetComponent<Health>().GetDead())
                {
                    // Animation (In shock and fear)
                }
                else
                {
                    // Animation (pull him off the ground)
                    unconsciousBody.GetComponent<HumanController>().WakeUp();
                }
                unconsciousBody = null;
                SetWitness();
                SetState(HuumanState.Alert);
                UpdateSuspicion();
            }
            return;
        }
        #endregion

        #region Sees Cat Functions
        private void Panic()
        {
            // summary: if can see cat, run away, or head to nearest room/downstairwell that is > 90* angle from cat.  

            // Push the Demon Alert Button if not already done so
            if (demonAlert == false)
            {
                if (fov.GetPlayerVisibility() && fov.GetCatForm())
                {
                    // Run to safety! Already knows lastknownpos
                }
                else
                {
                    PullDemonSwitch();
                    if (hidePoint != Vector3.zero)
                        hidePoint = Vector3.zero;
                    return;
                }
            }

            // Arrival at Hidepoint
            if (hidePoint != Vector3.zero)
            {
                if (AtTargetPoint(hidePoint) || isHiding == true)
                {
                    isHiding = true;
                    transform.LookAt(player.transform);

                    if (fov.GetPlayerVisibility() && fov.GetCatForm())
                    {
                        fighter.Attack(player);
                        if (Mathf.Abs(player.transform.position.y - hidePoint.y) > 5f || Vector3.Distance(player.transform.position, hidePoint) > 20f)
                        {
                            mover.StartMoveAction(hidePoint, sprintSpeed);
                        }
                        return;
                    }
                    //else
                    //    mover.StartMoveAction(hidePoint, sprintSpeed);
                    return;
                }
            }

            float distToCat = Vector3.Distance(transform.position, lastKnownPos);

            Vector3 tempHidePoint = Vector3.zero;
            float distToHide = Mathf.Infinity;
            #region Run Away, Cat on floor AND nearby
            if (Mathf.Abs(transform.position.y - lastKnownPos.y) < 2f && distToCat < 20f)
            {
                // Find a hide spot and test if angles > 90 between lastKnownPos, human, hidingspot
                foreach (HidingSpot spot in floorSpots)
                {
                    if (Vector3.Angle(lastKnownPos - transform.position,
                        spot.transform.position - transform.position) > 90f)
                    {
                        if (Vector3.Distance(transform.position, spot.transform.position) < distToHide)
                        {
                            tempHidePoint = spot.GetPoint();
                            distToHide = Vector3.Distance(transform.position, spot.transform.position);
                        }
                    }
                }
                if (tempHidePoint != Vector3.zero)
                {
                    hidePoint = tempHidePoint;
                    mover.StartMoveAction(hidePoint, sprintSpeed);
                    return;
                }


                // Find a downward stairwell and test if angles > 90 between lastKnownPos, human, stairwell
                if (myFloor != null)
                {
                    foreach (Stairs stair in myFloor.GetStairs())
                    {
                        if (stair.IsDownStairs(transform.position.y))
                        {
                            if (Vector3.Angle(lastKnownPos - transform.position,
                                stair.GetStairsPoint(true) - transform.position) > 90f)
                            {
                                if (Vector3.Distance(transform.position, stair.GetStairsPoint(true)) < distToHide)
                                {
                                    tempHidePoint = stair.GetStairsPoint(false);
                                    distToHide = Vector3.Distance(transform.position, stair.GetStairsPoint(true));
                                }
                            }
                        }
                    }
                    if (tempHidePoint == Vector3.zero)
                    {
                        hidePoint = tempHidePoint;
                        mover.StartMoveAction(hidePoint, sprintSpeed);
                        return;
                    }
                }

                // If all else fails, run away!
                tempHidePoint = transform.position - lastKnownPos;
                mover.StartMoveAction(transform.position + tempHidePoint, sprintSpeed);
                hidePoint = Vector3.zero;
                return;
            }
            #endregion

            #region Run Away, Cat NOT on floor OR nearby
            // Go to nearest hiding spot on floor
            foreach (HidingSpot spot in floorSpots)
            {
                if (Vector3.Distance(transform.position, spot.transform.position) < distToHide)
                {
                    tempHidePoint = spot.GetPoint();
                    distToHide = Vector3.Distance(transform.position, spot.transform.position);
                }
            }
            if (tempHidePoint != Vector3.zero)
            {
                hidePoint = tempHidePoint;
                mover.StartMoveAction(hidePoint, sprintSpeed);
                return;
            }

            // Go to nearest downward stairwell
            if (myFloor != null)
            {
                foreach (Stairs stair in myFloor.GetStairs())
                {
                    if (stair.IsDownStairs(transform.position.y))
                    {
                        if (Vector3.Distance(transform.position, stair.GetStairsPoint(true)) < distToHide)
                        {
                            tempHidePoint = stair.GetStairsPoint(false);
                            distToHide = Vector3.Distance(transform.position, stair.GetStairsPoint(true));
                        }
                    }
                }
                if (tempHidePoint != new Vector3())
                {
                    hidePoint = tempHidePoint;
                    mover.StartMoveAction(hidePoint, sprintSpeed);
                    return;
                }
            }
            #endregion

            if (myBuilding == null)
            {
                // if closer to evac, evac, if closer to building that doesnt have cat, go to building
                SetState(HuumanState.Evacuate);
                Debug.Log("EVACTUATING THIS POOR INDIVIDUAL SOUL " + gameObject.name);
                return;
            }
            // if all else fails, stay here
            hidePoint = transform.position;
            return;
        }
        #endregion

        #region Evacuation Functions
        private void CalculateEvac()
        {
            if (evacPoint != null)
            {
                Evacuate();
                // if see cat, run away, eventually building and then safe room
                return;
            }

            float otherDist = 0f;
            Transform evacPts = GameObject.Find("Evac Points").transform;
            float dist = Vector3.Distance(transform.position, evacPts.GetChild(0).position);
            evacPoint = evacPts.GetChild(0);
            for (int i = 1; i < evacPts.childCount; i++)
            {
                otherDist = Vector3.Distance(transform.position, evacPts.GetChild(i).position);
                if (otherDist < dist)
                {
                    evacPoint = evacPts.GetChild(i);
                    dist = otherDist;
                }
            }
            mover.StartMoveAction(evacPoint.position, sprintSpeed);
            hidePoint = Vector3.up * -100f;
        }

        private void Evacuate()
        {
            if (AtTargetPoint(evacPoint.position))
            {
                Destroy(gameObject);
                return;
            }

            mover.StartMoveAction(evacPoint.position, moveSpeed);
        }
        #endregion

        #region Knockout Functions
        private void KOed()
        {
            if (hasBeenFound == true)
                if (Time.time - mainTimer >= wakeUpDelay)
                {
                    SetState(HuumanState.Alert);
                    hasBeenFound = false;
                    Heal();
                }
            return;
        }

        public void WakeUp()
        {
            mainTimer = Time.time;
            hasBeenFound = true;
            GetComponent<Animator>().SetTrigger("awake");
        }

        public void Heal()
        {
            if (health.GetKO())
                GetComponent<Animator>().SetTrigger("awake");
            health.Heal();
            mainTimer = Time.time;
        }
        #endregion

        #region Dead Functions
        private void Dead()
        {
            if (hasBeenFound) return;
            if (Time.time - mainTimer > 60f) hasBeenFound = true;
            return;
        }
        #endregion

        #region Animator Events
        void Hit()
        {

        }

        void StopHit()
        {
            //GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        #endregion

        #region Getters and Setters
        // Getters
        public bool GetFound()
        {
            return hasBeenFound;
        }

        public bool GetAggravate()
        {
            return aggravate;
        }

        // Setters
        public void TriggerFound()
        {
            if (hasBeenFound == true) return;

            mainTimer = Time.time;
        }

        public void Aggravate()
        {
            if (aggravate == true) return;
            aggravate = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public void SetDemonAlert()
        {
            // add timer implementation for slight randomness of preparedness in people. 
            Aggravate();
            demonAlert = true;
            toButton = null;
            SetState(HuumanState.SeesCat);
        }

        public void SetState(HuumanState newState)
        {
            if (state == newState) return;
            secondTimer = Time.time - Random.Range(0, reactionTime);
            state = newState;
        }
        #endregion
    }

    public enum HuumanState
    {
        Work, // Normal State
        Suspicious, // When the human sees something but not fully on Alert, is timed
        Alert, // Behavior Changes, searching for demonic behaviours, might be timed
        Help, // Inspect body on ground and if able, heal and awaken them, goes to alert
        SeesCat, // Reaction to seeing Cat, either hide and stay safe or attack and chase
        Evacuate, // Run towards evac point
        KO, // is KO, if is healed, waits until timer resets to change state
        Dead, // dead 
    }
}