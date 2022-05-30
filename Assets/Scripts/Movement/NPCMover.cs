using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using gameracers.Control;
using gameracers.Core;
using gameracers.Stats;

namespace gameracers.Movement
{
    public class NPCMover : MonoBehaviour, IAction
    {
        [SerializeField] float moveSpeed = 2f;
        [SerializeField] float maxSpeed = 8f;
        float sprintMod = 1f;
        NavMeshAgent navMeshAgent;
        Health health;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedMod)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedMod);
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedMod)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = speedMod * Mathf.Clamp01(speedMod);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }
    }
}