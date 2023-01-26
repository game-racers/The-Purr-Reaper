using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace gameracers.Movement
{
    public class PlayerMoverTemp : MonoBehaviour
    {
        NavMeshAgent agent;
        Transform moveRef;

        // Movement
        float speed = 0f;
        float targetSpd;
        float speedCounter = 0f;
        [SerializeField] float walkSpd = 4f;
        [SerializeField] float sprintSpd = 10f;
        [SerializeField] float speedUp = .4f;
        [SerializeField] float slowDown = .3f;
        [SerializeField] float turnSmoothTime = 0.1f;
        Vector3 lastDir;

        float turnSmoothVelocity;

        // Gravity
        [SerializeField] LayerMask groundMask;

        // Jump
        [SerializeField] float jumpSpd = 1f;
        float jumpTime;
        [SerializeField] float searchRad = 1f;
        [SerializeField] GameObject jumpHighLight;
        bool isJump = false;
        Vector3 jumpDir;
        Vector3 startJump;
        Transform cam;
        Transform lookAtPoint;
        [SerializeField] float jumpDist = 5f;
        

        void Awake()
        {
            targetSpd = walkSpd;
            moveRef = transform.Find("Player Center").Find("Move Ref");
            cam = GameObject.Find("Main Camera").transform;
            lookAtPoint = transform.Find("Player Center").GetChild(1).GetChild(0);
            Application.targetFrameRate = 60;
            agent = GetComponent<NavMeshAgent>();
        }

        private void UpdateMovement()
        {
            if (isJump) return;
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    targetSpd = sprintSpd;
                else
                    targetSpd = walkSpd;
                lastDir = direction;
            }
            else
            {
                targetSpd = 0;
                if (speedCounter > slowDown)
                    speedCounter = slowDown;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (targetSpd > speed)
                    speedCounter = 0;
            }

            if (speedCounter > speedUp)
                speedCounter = speedUp;
            if (speedCounter < 0)
                speedCounter = 0;

            if (speed > targetSpd)
            {
                speedCounter -= Time.deltaTime;
                speed = Mathf.Lerp(speed, targetSpd, (slowDown - speedCounter) / slowDown);
            }
            if (speed < targetSpd)
            {
                speedCounter += Time.deltaTime;
                speed = Mathf.Lerp(speed, targetSpd, speedCounter / speedUp);
            }

            if (speed > .1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + moveRef.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                agent.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }

        private void UpdateSpeed(float forwardSpeed)
        {
            GetComponent<Animator>().SetFloat("forwardSpeed", forwardSpeed);
        }

        public bool UpdateJump()
        {
            jumpTime += jumpSpd * Time.deltaTime;
            transform.position = Vector3.Lerp(startJump, jumpDir, jumpTime);
            if (jumpTime > 1f)
            {
                jumpTime = 0;
                isJump = false;
                jumpDir = new Vector3();
                agent.enabled = true;
                return true;
            }
            return false;
        }

        public void Jump(Vector3 jumpVector)
        {
            agent.enabled = false;
            isJump = true;
            jumpDir = transform.position + jumpVector;
            startJump = transform.position;
        }

        /*
        private bool CheckJump()
        {
            Vector3 jumpTarget;
            Vector3 temp;
            float distToBeat = 100f;
            float dist;
            RaycastHit[] hits = Physics.SphereCastAll(lookAtPoint.position, searchRad, lookAtPoint.position - cam.position, jumpDist, groundMask, QueryTriggerInteraction.UseGlobal);
            if (hits.Length == 0) return false;
            
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject.layer == 12) // 12 is Jump Target
                {
                    temp = hits[i].point;
                    float dist = Vector3.Distance(lookAtPoint.position, hits[i].point);
                    if (dist < distToBeat)
                    {
                        distToBeat = dist;
                        jumpTarget = hits[i].point;
                    }
                }
                float dist = Vector3.Distance(lookAtPoint.position, temp.position);
                Debug.Log(temp.mask);
                if (temp.mask != 3) continue;
                if (dist < distToBeat)
                {
                    distToBeat = dist;
                    jumpTarget = temp;
                }
            }

            NavMesh.SamplePosition(jumpTarget.position, out temp, .1f, 3);
            if (temp.mask != 3) return false;
            if (distToBeat <= 10f)
            {
                jumpHighLight.SetActive(true);
                jumpHighLight.transform.position = temp.position;
                return true;
            }
            return false;
        }
        */

        public void SetJump()
        {
            isJump = false;
            agent.ResetPath();
        }

        public void UpdateMover(bool isAttack)
        {
            if (isJump == true)
            {
                return;
            }
            if (isAttack == false)
            {
                UpdateMovement();
                UpdateSpeed(speed);
            }
            else
            {
                UpdateSpeed(speed);
            }
        }
    }
}