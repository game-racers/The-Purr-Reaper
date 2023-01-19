using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace gameracers.Camera
{
    public class CameraCollider : MonoBehaviour
    {
        [SerializeField] Transform playerCenter;
        Transform tempCenter;
        Transform roamStart;
        Transform roamStop;
        Transform combatStart;
        Transform combatStop;
        Transform fpsPos;
        [SerializeField] float radius = .125f;
        [SerializeField] LayerMask mask;
        [SerializeField] float lerpMod = 3f;

        Transform activeStart;
        Transform activeStop;
        float activeDist;

        float lerpVal;
        int state = 0; // to become Enums
        bool isForward = false;

        void Start()
        {
            roamStart = playerCenter.GetChild(0);
            roamStop = playerCenter.GetChild(1);
            combatStart = playerCenter.GetChild(2);
            combatStop= playerCenter.GetChild(3);
            fpsPos = playerCenter.GetChild(4);

            ChangeFocus(0);
        }

        void LateUpdate()
        {
            if (isForward) lerpVal += Time.time * lerpMod;
            if (!isForward) lerpVal -= Time.time * lerpMod;
            lerpVal = Mathf.Clamp(lerpVal, 0, 1);

            isForward = false;

            RaycastHit[] hits = Physics.SphereCastAll(activeStop.position, radius, activeStart.position - activeStop.position, activeDist, mask, QueryTriggerInteraction.Ignore);

            float distToBeat = activeDist;
            Vector3 hitPoint = Vector3.zero;
            //if (hits.Any())
            //    hitPoint = hits[hits.Count() - 1].point;

            foreach (RaycastHit hit in hits)
            {
                isForward = true;
                hitPoint = hit.point;
            }

            if (hitPoint != Vector3.zero)
            {
                transform.position = GetClosestPointOnFiniteLine(hitPoint, activeStart.position, activeStop.position);
                return;
            }
            transform.position = activeStart.position;
        }

        private Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            float length = dir.magnitude;
            dir.Normalize();
            float projectLength = Mathf.Clamp(Vector3.Dot(point - start, dir), 0f, length);
            return start + dir * projectLength;
        }

        public void ChangeHost(Transform tempHost)
        {
            if (tempCenter != null)
            {
                tempCenter = null;
                ChangeFocus(0);
                return;
            }
            tempCenter = tempHost;
            ChangeFocus(3);
        }

        public void ChangeFocus(int newFocus)
        {
            if (newFocus == 3)
            {
                if (tempCenter == null)
                {
                    ChangeFocus(0);
                    Debug.Log("Ya fucked up");
                    return;
                }
                activeStart = tempCenter.GetChild(0);
                activeStop = tempCenter.GetChild(1);
                
            }
            if (newFocus == 0)
            {
                activeStart = roamStart;
                activeStop = roamStop;
            }
            if (newFocus == 1)
            {
                activeStart = combatStart;
                activeStop = combatStop;
            }
            if (newFocus == 2)
            {
                activeStart = fpsPos;
                activeStop = fpsPos;
            }
            activeDist = Vector3.Distance(activeStart.position, activeStop.position);
        }
    }
}
