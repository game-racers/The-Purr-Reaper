using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Movement
{
    public class PlayerMover : MonoBehaviour
    {
        CharacterController charController;
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

        float turnSmoothVelocity;

        // Gravity
        [SerializeField] float gravity = -9.81f;
        [SerializeField] Transform groundCheck;
        float groundDistance = 0.4f;
        [SerializeField] LayerMask groundMask;
        Vector3 velocity;

        // Jump
        [SerializeField] float searchRad = 1f;
        [SerializeField] GameObject jumpHighLight;
        Transform cam;
        Transform lookAtPoint;
        [SerializeField] float jumpDist = 1.8f;
        bool isGrounded;

        private void Awake()
        {
            targetSpd = walkSpd;
            moveRef = transform.Find("Player Center").Find("Move Ref");
            cam = GameObject.Find("Main Camera").transform;
            lookAtPoint = transform.Find("Player Center").GetChild(1).GetChild(0);
            Application.targetFrameRate = 60;
            charController = GetComponent<CharacterController>();
        }

        void Start()
        {
            
        }

        public void UpdateMover(bool isAttack)
        {
            if (isAttack == false)
            {
                UpdateMovement();
                UpdateSpeed(speed);
                UpdateJump();
            }
            else
            {
                UpdateSpeed(speed);
            }
        }

        public void UpdateGravity()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            GetComponent<Animator>().SetBool("isGrounded", isGrounded);
            velocity.y += gravity * Time.deltaTime;
            if (isGrounded == true && velocity.y < -4f)
            {
                velocity.y = -4f;
            }
            charController.Move(velocity * Time.deltaTime);
        }

        private void UpdateMovement()
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    targetSpd = sprintSpd;
                else
                    targetSpd = walkSpd;
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

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + moveRef.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                charController.Move(moveDir.normalized * speed * Time.deltaTime);
            }
        }

        private void UpdateSpeed(float forwardSpeed)
        {
            GetComponent<Animator>().SetFloat("forwardSpeed", forwardSpeed);
        }

        private void UpdateJump()
        {
            if (CheckJump() && Input.GetButtonDown("Jump"))
            {
                transform.position = jumpHighLight.transform.position;
                jumpHighLight.SetActive(false);
            }
        }

        private bool CheckJump()
        {
            if (isGrounded == false) return false;

            float distToBeat = 100f;
            int hitPoint = -1;
            RaycastHit[] hits = Physics.SphereCastAll(lookAtPoint.position, searchRad, lookAtPoint.position - cam.position, jumpDist, groundMask, QueryTriggerInteraction.Ignore);
            if (hits.Length != 0)
            {
                distToBeat = Vector3.Distance(lookAtPoint.position, hits[0].point);
                hitPoint = 0;
            }
            for (int i = 1; i < hits.Length; i++)
            {
                float dist = Vector3.Distance(lookAtPoint.position, hits[i].point);
                if (dist < distToBeat)
                {
                    distToBeat = dist;
                    hitPoint = i;
                }
            }

            if (hitPoint >= 0)
            {
                jumpHighLight.SetActive(true);
                jumpHighLight.transform.position = hits[hitPoint].point;
                return true;
            }
            return false;
        }

        public void SetSpeed(float newSpd, float newSprintSpd)
        {
            walkSpd = newSpd;
            sprintSpd = newSprintSpd;
        }

        public bool GetGrounded()
        {
            return isGrounded;
        }
    }
}
