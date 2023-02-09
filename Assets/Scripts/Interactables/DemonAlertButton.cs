using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Architecture;

namespace gameracers.Interactables
{ 
    public class DemonAlertButton : MonoBehaviour
    {
        [SerializeField] GameObject alertLight;

        private void OnEnable()
        {
            EventListener.onDemonButton += SetDemonAlert;
        }

        private void OnDisable()
        {
            EventListener.onDemonButton -= SetDemonAlert;
        }

        private void SetDemonAlert(string id)
        {
            if (id == transform.parent.parent.GetComponent<Building>().GetBuildingID())
            {
                //animator switch states
                alertLight.SetActive(true);
            }
        }

        public string GetButtonID()
        {
            return transform.parent.parent.GetComponent<Building>().GetBuildingID();
        }
    }
}