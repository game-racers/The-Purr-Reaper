using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Movement
{
    public class PlayerMover : MonoBehaviour
    {
        CharacterController charController;
        [SerializeField] Transform cam;

        // Movement
        [SerializeField] float speed = 8f;
        [SerializeField] float sprintMod = 2f;
        Vector3 direction;
        [SerializeField] float turnSmoothTime = 0.1f;

        float turnSmoothVelocity;
        float sprintSpd = 1f;

        // Gravity and jump
        float gravity = -9.81f;
        [SerializeField] Transform groundCheck;
        float groundDistance = 0.4f;
        [SerializeField] LayerMask groundMask;
        [SerializeField] float jumpHeight = 2f;
        Vector3 velocity;
        bool isGrounded;

        // Start is called before the first frame update
        void Start()
        {
            charController = GetComponent<CharacterController>();
        }

        public void UpdateMover(bool isAttack)
        {
            if (isAttack == false)
            {
                UpdateMovement();
                UpdateSpeed(direction.magnitude * speed * sprintSpd);
                UpdateJump();
            }
            else
            {
                UpdateSpeed(0f);
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
            float horiz = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            direction = new Vector3(horiz, 0f, vert);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                sprintSpd = sprintMod;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                sprintSpd = 1f;
            }

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                charController.Move(moveDir.normalized * speed * sprintSpd * direction.magnitude * Time.deltaTime);
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
