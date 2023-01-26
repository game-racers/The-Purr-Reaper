using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Movement
{
    public class NavJumpTriggers : MonoBehaviour
    {
        [SerializeField] bool canJump = true;
        [SerializeField] Transform otherEnd;
        Vector3 direction;

        private void Awake()
        {
            direction = otherEnd.position - transform.position;
        }

        private void OnTriggerStay(Collider other)
        {
            if (canJump != true || other.tag != "Player") return;
            if (Input.GetButtonDown("Jump"))
            {
                EventListener.JumpAction(direction);
            }
        }

    }
}
