using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using gameracers.Core;

namespace gameracers.Movement
{
    public class PossessMover : MonoBehaviour
    {
        NPCMover mover;
        Transform camRef;

        // Movement
        float targetSpd;
        [SerializeField] float walkSpd = 2f;
        [SerializeField] float sprintSpd = 4f;
        [SerializeField] float stopBuffer = .3f;
        Vector3 destination;

        //float turnSmoothVelocity;


        private void Awake()
        {
            camRef = transform.Find("Camera Points").Find("Move Ref");
            targetSpd = walkSpd;
            mover = GetComponent<NPCMover>();
        }

        public void UpdateMovement()
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    targetSpd = sprintSpd;
                else
                    targetSpd = walkSpd;
            }
            
            if (direction.magnitude >= .1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camRef.eulerAngles.y;
                

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position + moveDir * 2, out hit, 1f, NavMesh.AllAreas))
                {
                    destination = hit.position;
                    mover.StartMoveAction(destination, targetSpd);
                    return;
                }
            }
        }
    }
}
