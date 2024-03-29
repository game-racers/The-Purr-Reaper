using System.Collections;
using System.Collections.Generic;
using gameracers.Interactables;
using UnityEngine;

namespace gameracers.Architecture
{
    public class Floor : MonoBehaviour
    {
        [SerializeField] int floorID = 1;
        List<HidingSpot> hidingSpots = new List<HidingSpot>();
        List<DemonAlertButton> demonButtons = new List<DemonAlertButton>();
        List<Stairs> stairs = new List<Stairs>();

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<HidingSpot>() != null)
                    hidingSpots.Add(child.GetComponent<HidingSpot>());
                if (child.GetComponent<DemonAlertButton>() != null)
                    demonButtons.Add(child.GetComponent<DemonAlertButton>());
                if (child.GetComponent<Stairs>() != null)
                    stairs.Add(child.GetComponent<Stairs>());
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

        public List<DemonAlertButton> GetDemonButtons()
        {
            return demonButtons;
        }

        public List<Stairs> GetStairs()
        {
            return stairs;
        }
    }
}
