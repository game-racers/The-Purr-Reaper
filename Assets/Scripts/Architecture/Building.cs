using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Architecture
{
    public class Building : MonoBehaviour
    {
        [SerializeField] string buildingID;
        GameObject player;

        List<Floor> floors = new List<Floor>();

        

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            foreach(Transform child in transform)
            {
                if (child.GetComponent<Floor>() != null)
                    floors.Add(child.GetComponent<Floor>());
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
    }

}