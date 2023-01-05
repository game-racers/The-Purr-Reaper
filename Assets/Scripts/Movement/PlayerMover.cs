using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

namespace gameracers.Movement
{
    public class PlayerMover : MonoBehaviour
    {
        CharacterController charController;
        [SerializeField] Transform camRef;

        // Movement
        float speed = 0f;
        float targetSpd;
        float speedCounter = 0f;
        [SerializeField] float walkSpd = 4f;
        [SerializeField] float sprintSpd = 10f;
        [SerializeField] float speedUp = .4f;
        [SerializeField] float slowDown = .3f;
        Vector3 direction;
        float speedup = 0;
        [SerializeField] float turnSmoothTime = 0.1f;

        float turnSmoothVelocity;

        // Gravity and jump
        float gravity = -9.81f;
        [SerializeField] Transform groundCheck;
        float groundDistance = 0.4f;
        [SerializeField] LayerMask groundMask;
        [SerializeField] float jumpHeight = 2f;
        Vector3 velocity;
        bool isGrounded;

        private void Awake()
        {
            targetSpd = walkSpd;
        }


        void Start()
        {
            Application.targetFrameRate = 60;
            charController = GetComponent<CharacterController>();
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
            if (isGrounded == false && velocity.y < -4f)
            {
                velocity.y = -4f;
            }
        }

        private void UpdateMovement()
        {
            direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

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
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camRef.eulerAngles.y;
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
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            velocity.y += gravity * Time.deltaTime;
            charController.Move(velocity * Time.deltaTime);
        }

        public bool GetGrounded()
        {
            return isGrounded;
        }
    }
}
