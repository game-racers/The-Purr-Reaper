using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

namespace gameracers.Architecture
{
    public class HidingSpot : MonoBehaviour
    {
        [SerializeField] string hidingID;
        [SerializeField] int maxPeople = 5;
        List<GameObject> people = new List<GameObject>();

        private void OnTriggerEnter(Collider other)
        {
            // Human Enters
            if (other.gameObject.tag == "Human")
            { 
                //EventListener.CrossHidingSpot(other.gameObject, hidingID, true);
                people.Add(other.gameObject);
                return;
            }

            // Cat Enters
            if (other.gameObject.tag == "Player")
            {
                //EventListener.CrossHidingSpot(other.gameObject, hidingID, true);
                return;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Human Exits
            if (other.gameObject.tag == "Human")
            {
                //EventListener.CrossHidingSpot(other.gameObject, hidingID, false);
                people.Remove(other.gameObject);
                return;
            }

            // Cat Exits
            if (other.gameObject.tag == "Player")
            {
                //EventListener.CrossHidingSpot(other.gameObject, hidingID, false);
                return;
            }
        }

        public Vector3 GetPoint()
        {
            Collider collider = GetComponent<Collider>();
            Vector3 center = collider.bounds.center;
            Vector3 size = collider.bounds.size;

            Vector3 targetPoint = new Vector3();
            targetPoint.x = center.x - Random.Range(-size.x / 2, size.x / 2);
            targetPoint.y = center.y;
            targetPoint.z = center.z - Random.Range(-size.z / 2, size.z / 2);

            return targetPoint;
        }

        public float GetOccupancy()
        {
            return people.Count / maxPeople;
        }
    }
}
