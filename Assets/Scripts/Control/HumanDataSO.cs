using gameracers.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Control
{
    [CreateAssetMenu(fileName = "Human Class", menuName = "Data/Human Data", order = 0)]
    public class HumanDataSO : ScriptableObject
    {
        public float health = 1;
        public float moveSpeed = 2;
        public float sprintSpd = 8;
        public bool canAttack = false;
        public bool isGuard = false;
        public float reactionTime = 3f;

        [SerializeField] public List<GameObject> hats;
        [SerializeField] public List<GameObject> shirts;
        [SerializeField] public List<GameObject> pants;
        [SerializeField] public List<GameObject> ties;
    }
}