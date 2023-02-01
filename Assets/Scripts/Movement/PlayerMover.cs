using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace gameracers.Movement
{
    public class PlayerMover : MonoBehaviour
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
        [SerializeField] float airSpd = 1f;
        [SerializeField] float jumpDelay = .5f;
        float jumpSpd;
        float jumpTime;
        [SerializeField] float searchRad = 1f;
        [SerializeField] GameObject jumpHighLight;
        bool isJump = false;
        Vector3[] jumpPts;
        int jumpI = 0;
        Vector3 startJump;
        

        void Awake()
        {
            targetSpd = walkSpd;
            jumpSpd = jumpDelay;
            moveRef = transform.Find("Player Center").Find("Move Ref");
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
            if (jumpI == 1)
            {
                transform.position = Vector3.Lerp(Vector3.Lerp(startJump, startJump + jumpPts[1], jumpTime), Vector3.Lerp(startJump + jumpPts[1], startJump + jumpPts[1] + jumpPts[2], jumpTime), jumpTime);
            }
            else
            {
                NavMeshHit hit;
                NavMesh.SamplePosition(startJump + jumpPts[jumpI], out hit, .5f, NavMesh.AllAreas);
                transform.position = Vector3.Lerp(startJump, hit.position, jumpTime);
            }    
            if (jumpTime > 1f)
            {
                startJump = transform.position;
                jumpSpd = airSpd;
                jumpTime = 0;
                jumpI++;
                if (jumpI == 2)
                {
                    jumpSpd = jumpDelay;
                    jumpI = 3;
                }
                if (jumpI == 4)
                {
                    jumpI = 0;
                    jumpSpd = jumpDelay;
                    agent.enabled = true;
                    isJump = false;
                    return true;
                }
            }
            return false;
        }

        public void Jump(List<Vector3> jumpPts)
        {
            Vector3 lookDir = jumpPts[jumpPts.Count - 1].normalized;
            transform.LookAt(lookDir + transform.position);
            agent.enabled = false;
            isJump = true;
            this.jumpPts = jumpPts.ToArray();
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