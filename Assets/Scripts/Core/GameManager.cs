using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using gameracers.Architecture;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace gameracers.Core
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] List<GameObject> buildings;
        List<BuildingOccupant> occupants = new List<BuildingOccupant>();
        List<GameObject> outsidePeople = new List<GameObject>();
        GameObject player;
        
        public class BuildingOccupant
        {
            public string buildingID;
            public GameObject personObj;

            public BuildingOccupant(string buildingID, GameObject personObj)
            {
                this.buildingID = buildingID;
                this.personObj = personObj;
            }
        }

        private void OnEnable()
        {
            EventListener.onCrossThreshold += EntityCrossThreshold;
        }

        private void OnDisable()
        {
            EventListener.onCrossThreshold -= EntityCrossThreshold;
        }

        private void EntityCrossThreshold(GameObject entity, GameObject building, bool isEnter)
        {
            if (GameObject.ReferenceEquals(entity, player))
            {
                return;
            }
            if (isEnter == true)
            {
                BuildingOccupant person = new BuildingOccupant(building.GetComponent<Building>().GetBuildingID(), entity);
                occupants.Add(person);
                outsidePeople.Remove(entity);
            }
            if (isEnter == false)
            {
                for (int i = 0; i < occupants.Count; i++)
                {
                    if (occupants[i].buildingID == building.GetComponent<Building>().GetBuildingID())
                    {
                        if (GameObject.ReferenceEquals(occupants[i].personObj, entity))
                        {
                            occupants.RemoveAt(i);
                            outsidePeople.Add(entity);
                            return;
                        }
                    }
                }
            }
        }

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            outsidePeople = GameObject.FindGameObjectsWithTag("Human").ToList<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}