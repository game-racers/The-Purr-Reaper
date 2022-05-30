using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointGizmoRadius = 0.3f;
        [SerializeField] bool isLoop = true; // Last index to first index or go down
        bool isForward = true;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWayPoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
            }
        }

        public int GetNextIndex(int i)
        {
            if (isLoop == true)
                return (i + 1) % (transform.childCount);
            if (isForward == true && i + 1 < transform.childCount)
                return (i + 1);
            if (isForward == true && i + 1 >= transform.childCount)
                isForward = false;
            if (i - 1 >= 0)
                return i - 1;
            isForward = true;
            return i + 1;

        }

        public Vector3 GetWayPoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}