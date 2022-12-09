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
        bool seesKO = false;
        bool isAccounted = false;

        GameObject unconsciousBody;

        private void Start()
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }

        public void FieldOfViewCheck()
        {
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Check for Cat
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

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Check for Unconscious Human
            isAccounted = false;
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
                            if (GameObject.ReferenceEquals(npc.gameObject, gameObject)) continue;

                            if (npc.gameObject.GetComponent<Health>().GetKO())
                            {
                                HumanBrain ai = npc.gameObject.GetComponent<HumanBrain>();
                                if (ai.enabled == true)
                                {
                                    unconsciousBody = npc.gameObject;
                                    seesKO = true;
                                    //ai.TriggerFoundKO(npc.gameObject);
                                    return;
                                }
                            }

                            if (npc.gameObject.GetComponent<Health>().GetDead())
                            {
                                HumanBrain ai = npc.gameObject.GetComponent<HumanBrain>();
                                if (ai.enabled == true)
                                {
                                    seesDead = true;
                                    ai.TriggerFound();
                                    isAccounted = ai.GetFound();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            seesKO = false;
            seesDead = false;
            isAccounted = false;
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

        public bool CanSeeKO()
        {
            return seesKO;
        }

        public GameObject GetBody()
        {
            return unconsciousBody;
        }

        public bool IsAccounted()
        {
            return isAccounted;
        }
    }
}