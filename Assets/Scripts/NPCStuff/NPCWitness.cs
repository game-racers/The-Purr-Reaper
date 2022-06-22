using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Control;

namespace gameracers.NPCStuff
{
    public class NPCWitness : MonoBehaviour
    {
        public PatrolPath newPath = null;
        public Transform newIdle = null;
        public Transform evacPoint = null;
        public float newAngle = 100;
        public float newFOVRadius = 10f;
        public bool newCanWander = false;
    }
}