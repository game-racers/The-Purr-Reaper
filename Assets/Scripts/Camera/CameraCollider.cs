using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

namespace gameracers.Camera
{
    public class CameraCollider : MonoBehaviour
    {
        [SerializeField] Transform playerCenter;
        Transform tempCenter;
        // Cam positions for player
        Transform roamStart;
        Transform camStop;
        Transform combatStart;
        Transform fpsPos;
        // Cam look at positions for player
        Transform roamLook;
        Transform fpsLook;

        // Focus options
        Transform focusPoint;
        [SerializeField] float radius = .125f;
        [SerializeField] LayerMask mask;
        [SerializeField] float lerpMod = 3f;

        Transform activeStart;
        Transform activeStop;
        float activeDist;

        float lerpVal;
        int state = 0; // to become Enums
        bool isObstructed = false;

        void Start()
        {
            Transform camPoints = playerCenter.GetChild(0);
            roamStart = camPoints.GetChild(0);
            combatStart = camPoints.GetChild(1);
            camStop = camPoints.GetChild(2);
            fpsPos = camPoints.GetChild(3);
            camPoints = playerCenter.GetChild(1);
            roamLook = camPoints.GetChild(0);
            fpsLook = camPoints.GetChild(1);

            ChangePOV(0);
        }

        void LateUpdate()
        {
            if (isObstructed) lerpVal += Time.time * lerpMod;
            if (!isObstructed) lerpVal -= Time.time * lerpMod;
            lerpVal = Mathf.Clamp(lerpVal, 0, 1);

            isObstructed = false;

            RaycastHit[] hits = Physics.SphereCastAll(activeStop.position, radius, activeStart.position - activeStop.position, activeDist, mask, QueryTriggerInteraction.Ignore);

            float distToBeat = activeDist;
            Vector3 hitPoint = Vector3.zero;
            if (hits.Any())
                hitPoint = hits[0].point;

            //foreach (RaycastHit hit in hits)
            //{
            //    isObstructed = true;
            //    hitPoint = hit.point;
            //}

            if (focusPoint != null)
            {
                transform.LookAt(focusPoint);
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
                ChangePOV(0);
                return;
            }
            tempCenter = tempHost;
            ChangePOV(3);
        }

        public void ChangePOV(int newFocus)
        {
            state = newFocus;

            if (newFocus == 3)
            {
                if (tempCenter == null)
                {
                    ChangePOV(0);
                    Debug.Log("Ya fucked up");
                    return;
                }
                activeStart = tempCenter.GetChild(0);
                activeStop = tempCenter.GetChild(1);
                transform.LookAt(tempCenter.GetChild(3));
                activeDist = Vector3.Distance(activeStart.position, activeStop.position);
                return;
            }
            if (newFocus == 0)
            {
                activeStart = roamStart;
                activeStop = camStop;
                transform.LookAt(roamLook);
            }
            if (newFocus == 1)
            {
                activeStart = combatStart;
                activeStop = camStop;
                transform.LookAt(fpsLook);
            }
            if (newFocus == 2)
            {
                activeStart = fpsPos;
                activeStop = fpsPos;
                transform.LookAt(fpsLook);
            }
            activeDist = Vector3.Distance(activeStart.position, activeStop.position);
        }

        public void FocusOn(Transform newFocus)
        {
            if (newFocus == focusPoint)
            {
                newFocus = null;
                ChangePOV(state);
                return;
            }
            focusPoint = newFocus;
            transform.LookAt(focusPoint);
        }
    }
}
