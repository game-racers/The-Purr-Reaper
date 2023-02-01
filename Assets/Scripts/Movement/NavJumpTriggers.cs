using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Movement
{
    public class NavJumpTriggers : MonoBehaviour
    {
        [SerializeField] bool canJump = true;
        [SerializeField] Transform startJump;
        [SerializeField] Transform midPoint;
        [SerializeField] Transform landPoint;
        [SerializeField] Transform endJump;
        List<Vector3> jumpPts = new List<Vector3>();

        private void Awake()
        {
            GetComponent<MeshRenderer>().enabled = false;
            jumpPts.Add(Vector3.zero);
            jumpPts.Add(midPoint.position - startJump.position);
            jumpPts.Add(landPoint.position - midPoint.position);
            jumpPts.Add(endJump.position - landPoint.position);
        }

        private void OnTriggerStay(Collider other)
        {
            JumpScript(other);
        }

        private void JumpScript(Collider other)
        {
            if (canJump != true || other.tag != "Player") return;
            if (Input.GetButton("Jump"))
            {
                Vector3 jumpDir = startJump.position - transform.position;
                Vector3 dir0 = startJump.position - other.transform.position;
                float adjacent = dir0.magnitude * Mathf.Cos(Vector3.Angle(jumpDir, dir0) * Mathf.Deg2Rad);
                Vector3 actual = jumpDir.normalized * adjacent;
                jumpPts[0] = actual;
                EventListener.JumpAction(jumpPts);
            }
        }
    }
}
