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
        Transform playerCenter;
        Vector3 roamStart;
        Vector3 roamStop;
        Vector3 combatStart;
        Vector3 combatStop;
        Vector3 fpsPos;
        [SerializeField] float radius = .125f;
        [SerializeField] LayerMask mask;
        [SerializeField] float lerpMod = 3f;

        Vector3 activeStart;
        Vector3 activeStop;
        float activeDist;

        float lerpVal;
        int state = 0; // to become Enums
        bool isForward = false;

        void Start()
        {
            playerCenter = transform.parent;
            roamStart = playerCenter.GetChild(1).position;
            roamStop = playerCenter.GetChild(2).position;
            combatStart = playerCenter.GetChild(3).position;
            combatStop= playerCenter.GetChild(4).position;
            fpsPos = playerCenter.GetChild(5).position;

            ChangeFocus(0);
        }

        void Update()
        {
            if (isForward) lerpVal += Time.time * lerpMod;
            if (!isForward) lerpVal -= Time.time * lerpMod;
            lerpVal = Mathf.Clamp(lerpVal, 0, 1);

            isForward = false;

            RaycastHit[] hits = Physics.SphereCastAll(activeStop, radius, activeStart - activeStop, activeDist, mask, QueryTriggerInteraction.Ignore);

            float distToBeat = activeDist;
            Vector3 hitPoint = Vector3.zero;
            //if (hits.Any())
            //    hitPoint = hits[hits.Count() - 1].point;

            foreach (RaycastHit hit in hits)
            {
                Debug.Log("Hit!");
                isForward = true;
                hitPoint = hit.point;
            }

            Debug.Log("Hitpoint: " + hitPoint);
            if (hitPoint != Vector3.zero)
            {
                Debug.Log("Help");
                transform.position = Vector3.Lerp(activeStart, hitPoint, lerpVal);
            }
        }

        public void ChangeFocus(int newFocus)
        {
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
            activeDist = Vector3.Distance(activeStart, activeStop);
        }
    }
}
