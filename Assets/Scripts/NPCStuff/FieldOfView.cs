using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Control;
using gameracers.Stats;

namespace gameracers.NPCStuff
{
    public class FieldOfView : MonoBehaviour
    {
        public float radius;
        [Range(0, 360)]
        public float angle;

        public GameObject playerRef;

        public LayerMask playerMask;
        public LayerMask npcMask;
        public LayerMask obstructionMask;

        bool canSeePlayer;
        bool isCatEvil;
        bool seesDead = false;

        private void Start()
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }

        private IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        public void FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, playerMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        canSeePlayer = true;
                        isCatEvil = playerRef.GetComponent<PlayerBrain>().getForm();
                    }
                    else
                        canSeePlayer = false;
                }
                else
                    canSeePlayer = false;
            }
            else if (canSeePlayer)
                canSeePlayer = false;

            Collider[] npcChecks = Physics.OverlapSphere(transform.position, radius, npcMask);

            if (npcChecks.Length != 1)
            {
                foreach (Collider npc in npcChecks)
                {
                    Transform target = npc.transform;
                    Vector3 directionToTarget = (target.position - transform.position).normalized;

                    // self check
                    if (npc.gameObject == this) continue;

                    if (Vector3.Angle(directionToTarget, transform.forward) < angle / 2)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target.position);

                        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                        {
                            if (GameObject.ReferenceEquals(npc.gameObject, gameObject)) return;

                            if (npc.gameObject.GetComponent<Health>().GetDead())
                            {
                                if (npc.gameObject.GetComponent<HumanController>().enabled == true)
                                {
                                    seesDead = true;
                                }
                                    //seesDead = false;
                            }
                            //else
                                //seesDead = false;
                        }
                        //else
                            //seesDead = false;
                    }
                    //else
                        //seesDead = false;
                }
            }
        }

        public Transform getLastLocation()
        {
            return playerRef.transform;
        }

        public bool GetPlayerVisibility()
        {
            return canSeePlayer;
        }

        public bool GetCatForm()
        {
            return isCatEvil;
        }

        public bool CanSeeDead()
        {
            return seesDead;
        }
    }
}