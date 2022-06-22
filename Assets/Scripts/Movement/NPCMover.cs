using UnityEngine;
using UnityEngine.AI;
using gameracers.Core;

namespace gameracers.Movement
{
    public class NPCMover : MonoBehaviour, IAction
    {
        NavMeshAgent navMeshAgent;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
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