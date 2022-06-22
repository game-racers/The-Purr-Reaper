using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;

namespace gameracers.Abilities
{ 
    public class PlayerPossess : MonoBehaviour
    {
        // Possession
        SkinnedMeshRenderer catRenderer;
        Material ghostMat;
        Material demonMat;

        [SerializeField] Transform deadBodyCheck;
        [SerializeField] float bodyRadius = 3f;
        [SerializeField] LayerMask deadMask;

        float possessAnimTimerMax = 1.1f;
        float possessTimer;
        GameObject possessed = null;
        bool possessing = false;

        // Start is called before the first frame update
        void Awake()
        {
            catRenderer = transform.Find("Purr Reaper").Find("Demon Cat").GetComponent<SkinnedMeshRenderer>();
            ghostMat = catRenderer.materials[0];
            demonMat = catRenderer.materials[1];
            SetSkin(0, demonMat);
        }

        public bool PossessionStart()
        {
            /*
            Summary: 
                Starts the possession ability. Tests for a dead body, and teleports body elsewhere. 

            Returns:
                A bool that acts more like a trigger. Telling the controller that calls this function when to enable/disable the controllers.
             */

            if (Input.GetKeyDown(KeyCode.F))
            {
                Collider[] hits = Physics.OverlapSphere(deadBodyCheck.position, bodyRadius, deadMask);
                if (hits.Length > 0)
                {
                    foreach (Collider hit in hits)
                    {
                        if (hit.gameObject.GetComponent<Health>().GetDead() && hit.gameObject.GetComponent<Health>().GetVeryDead() == false)
                        {
                            possessed = hit.gameObject;
                            possessTimer = Time.time;
                            SetSkin(0, ghostMat);
                            GetComponent<Animator>().SetBool("isPossess", true);
                            transform.LookAt(possessed.transform.position);
                        }
                    }
                }
            }

            if (possessed != null && Time.time - possessTimer > possessAnimTimerMax)
            {
                // make camera follow possessed
                var vcam = GameObject.Find("Third Person Camera").GetComponent<Cinemachine.CinemachineFreeLook>();
                vcam.Follow = possessed.transform;
                vcam.LookAt = possessed.transform;

                return true;
            }

            return false;
        }

        public bool PossessionEnd()
        {
            /*
            Summary: 
                Ends the possession ability. Sets the possessed entity's animator to die and sets the camera to follow the player
            Returns:
                A bool that acts more like a trigger. Telling the controller that calls this function when to enable/disable the controllers.
             */

            if (possessed != null && Input.GetKeyDown(KeyCode.F))
            {
                // Animation starts, timer is for the material
                GetComponent<Animator>().SetBool("isPossess", false);
                possessTimer = Time.time;

                possessed.GetComponent<Animator>().SetTrigger("die");
                possessing = true;
            }

            if (possessing == true && Time.time - possessTimer > possessAnimTimerMax)
            {
                var vcam = GameObject.Find("Third Person Camera").GetComponent<Cinemachine.CinemachineFreeLook>();
                vcam.Follow = transform;
                vcam.LookAt = transform;

                SetSkin(0, demonMat);
                possessed = null;
                possessing = false;

                return true;
            }

            return false;
        }

        public void SetSkin(int matNum, Material newMat)
        {
            /*
            Summary: 
                Changes the Material of the Purr Reaper to and from the ghostly material and the demon material.
            */

            Material[] mats = catRenderer.materials;
            mats[matNum] = newMat;
            catRenderer.materials = mats;
        }

        public GameObject GetPossessed()
        {
            /*
            Summary:
                Getter for the possessed GameObject
            Returns:
                possessed GameObject
             */
            return possessed;
        }
    }
}
