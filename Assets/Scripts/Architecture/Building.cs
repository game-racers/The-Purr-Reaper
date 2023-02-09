using System.Collections;
using System.Collections.Generic;
using gameracers.Interactables;
using UnityEngine;

namespace gameracers.Architecture
{
    public class Building : MonoBehaviour
    {
        [SerializeField] string buildingID;
        GameObject player;

        List<Floor> floors = new List<Floor>();
        List<DemonAlertButton> demonButtons = new List<DemonAlertButton>();
        bool panicButton = false;

        private void OnEnable()
        {
            EventListener.onDemonButton += DemonAlert;
        }

        private void OnDisable()
        {
            EventListener.onDemonButton -= DemonAlert;
        }

        private void DemonAlert(string id)
        {
            if (id == buildingID)
                panicButton = true;
        }

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Floor>() != null)
                {
                    floors.Add(child.GetComponent<Floor>());
                    for (int i = 0; i < child.childCount; i++)
                    {
                        if (child.GetChild(i).GetComponent<DemonAlertButton>() != null)
                        {
                            demonButtons.Add(child.GetChild(i).GetComponent<DemonAlertButton>());
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            EventListener.CrossBuilding(other.gameObject, gameObject, true);
        }

        private void OnTriggerExit(Collider other)
        {
            EventListener.CrossBuilding(other.gameObject, gameObject, false);
        }

        public string GetBuildingID()
        {
            return buildingID;
        }

        public bool GetPanic()
        {
            return panicButton;
        }
    }
}