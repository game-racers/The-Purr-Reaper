using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;
using gameracers.Movement;
using gameracers.Combat;
using UnityEngine.AI;

namespace gameracers.Control
{ 
    public class PossessedController : MonoBehaviour
    {
        [SerializeField] RuntimeAnimatorController animController;
        [SerializeField] bool isPossessed = false;
        Transform spawnPoint;

        // Movement
        [SerializeField] float deadSpeedMod = 100f;
        [SerializeField] float speed = 2f;
        [SerializeField] float sprintSpeed = 4f;
        [SerializeField] Transform cam;
        float sprintMod = 2f;
        Vector3 direction;
        [SerializeField] float turnSmoothTime = 0.1f;
        float turnSmoothVelocity;

        NPCMover mover;
        Health health;

        // Violence
        [SerializeField] float damage = 1f;
        [SerializeField] float attackMaxTimer = 1f;
        [SerializeField] Weapon foot;
        float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] bool isAttack = false;

        private void Awake()
        {
            spawnPoint = gameObject.transform.Find("SpawnPoint");
        }

        void Start()
        {
            GetComponent<Animator>().runtimeAnimatorController = animController;
            mover = GetComponent<NPCMover>();
            health = GetComponent<Health>();
            //foot.damage = damage;
            cam = GameObject.Find("Main Camera").transform;
        }

        void Update()
        {
            if (isPossessed != true) return;
            if (health.GetVeryDead()) return;


            //if (Input.GetButtonDown("Fire1") && timeSinceLastAttack > attackMaxTimer)
            //{
            //    UpdateAttack();
            //}

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                sprintMod = sprintSpeed;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                sprintMod = speed;
            }

            UpdateMovement();
            UpdateSpeed(direction.magnitude * sprintMod);

            timeSinceLastAttack += Time.deltaTime;
        }

        private void UpdateAttack()
        {
            GetComponent<Animator>().SetTrigger("attack");
            isAttack = true;
        }

        private void UpdateMovement()
        {
            float horiz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");

            direction = new Vector3(horiz, 0f, vert).normalized;

            //float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            //mover.MoveTo(transform.position + moveDir * 10f, sprintMod);

            if (direction.magnitude >= .1f)
            {
                Vector3 destination = transform.position + transform.right * direction.x * 10 + transform.forward * direction.z * 10;
                //mover.StartMoveAction(destination, sprintMod);
                mover.MoveTo(destination, sprintMod);
                //GetComponent<NavMeshAgent>().destination = destination;
                return;
            }
            else
            {
                mover.Cancel();
            }
        }

        void UpdateSpeed(float forwardSpeed)
        {
            GetComponent<Animator>().SetFloat("forwardSpeed", forwardSpeed);
        }

        public void PossessState(bool val)
        {
            isPossessed = val;
            if (val == false)
            {
                GetComponent<Animator>().ResetTrigger("unDie");
                GetComponent<Animator>().SetTrigger("die");
            }
            if (val == true)
            {
                GetComponent<Animator>().SetTrigger("unDie");
            }
        }

        public Vector3 GetSpawnPoint()
        {
            return spawnPoint.position;
        }    

        // Animator actions
        void Hit()
        {

        }

        void StopHit()
        {
            isAttack = false;
        }
    }
}