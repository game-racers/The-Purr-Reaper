using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Architecture
{
    public class Floor : MonoBehaviour
    {
        [SerializeField] int floorID = 1;
        List<HidingSpot> hidingSpots = new List<HidingSpot>();

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<HidingSpot>() != null)
                    hidingSpots.Add(child.GetComponent<HidingSpot>());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Human Enters
            if (other.gameObject.tag == "Human")
            {
                EventListener.NewFloor(other.gameObject, gameObject, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Human Exits
            if (other.gameObject.tag == "Human")
            {
                EventListener.NewFloor(other.gameObject, gameObject, false);
            }
        }

        public int GetFloorID()
        {
            return floorID;
        }

        public List<HidingSpot> GetHidingSpots()
        {
            return hidingSpots;
        }
    }
}
