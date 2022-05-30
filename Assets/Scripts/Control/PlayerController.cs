using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;

namespace gameracers.Control
{ 
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float speed = 6f;
        [SerializeField] float sprintMod = 2f;
        Vector3 direction;
        [SerializeField] float turnSmoothTime = 0.1f;
        [SerializeField] Transform cam;
        [SerializeField] bool demonForm = true;

        CharacterController controller;
        Health health;
        [SerializeField] ParticleSystem particles;
        [SerializeField] GameObject demonCat;
        [SerializeField] GameObject liveCat;

        float turnSmoothVelocity;
        float sprintSpd = 1f;

        // Possession
        SkinnedMeshRenderer catRenderer;
        Material ghostMat;
        Material demonMat;
        [SerializeField] Transform checkForDeadBody;
        [SerializeField] float deadBodyRadius = 2f;
        [SerializeField] LayerMask deadMask;
        float possessTimerMax = 1.1f;
        float possessTimer = Mathf.Infinity;
        GameObject possessed = null;
        [SerializeField] bool leaveBody = false;

        // Violence
        [SerializeField] float damage = 1f;
        [SerializeField] float attackMaxTimer = 1f;
        [SerializeField] Weapon scythe;
        float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] bool isAttack = false;

        // Gravity and Jump
        [SerializeField] float gravity = -9.81f;
        [SerializeField] Transform groundCheck;
        [SerializeField] float groundDistance = 0.4f;
        [SerializeField] LayerMask groundMask;
        [SerializeField] float jumpHeight = 2f;
        Vector3 velocity;
        bool isGrounded;

        Vector3 initialPos;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            health = GetComponent<Health>();
            scythe.damage = damage;
            Cursor.lockState = CursorLockMode.Locked;
            catRenderer = transform.Find("Purr Reaper").Find("Demon Cat").GetComponent<SkinnedMeshRenderer>();
            ghostMat = catRenderer.materials[0];
            demonMat = catRenderer.materials[1];
            SetSkin(0, demonMat);
            initialPos = transform.position;
        }

        void Update()
        {
            UpdateTimers();

            UpdateGravity();

            if (health.GetDead()) return;

            UpdatePossessionStart();
            UpdatePossessionEnd();
            if (possessed != null) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                UpdateForm();
            }

            UpdateAttack();

            UpdateMovement();

            UpdateSpeed(direction.magnitude * speed * sprintSpd);

            UpdateJump();
        }

        private void UpdateForm()
        {
            demonCat.SetActive(!demonForm);
            liveCat.SetActive(demonForm);
            demonForm = !demonForm;
            particles.Play();
        }

        private void UpdateGravity()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            GetComponent<Animator>().SetBool("isGrounded", isGrounded);
            if (isGrounded && velocity.y < -4f)
            {
                velocity.y = -4f;
            }
        }

        private void UpdatePossessionStart()
        {
            // F to enter body
            if (Input.GetKeyDown(KeyCode.F) && possessTimer > possessTimerMax)
            {
                Collider[] hits = Physics.OverlapSphere(checkForDeadBody.position, deadBodyRadius, deadMask);
                if (hits.Length > 0)
                {
                    foreach (Collider hit in hits)
                    {
                        if (hit.gameObject.GetComponent<Health>() != null)
                        {
                            if (hit.gameObject.GetComponent<Health>().GetDead() && hit.gameObject.GetComponent<Health>().GetVeryDead() == false)
                            {
                                particles.Play();
                                possessed = hit.gameObject;
                                // Animation starts, timer is for the end anim for teleportation and stuff. 
                                possessTimer = 0f;
                                SetSkin(0, ghostMat);
                                GetComponent<Animator>().SetBool("isPossess", true);
                                transform.LookAt(possessed.transform.position);
                            }
                        }
                    }
                }
            }

            if (leaveBody == true) return;

            // During timer, moves forward
            if (possessed != null && possessTimer < possessTimerMax)
            {
                // unnecessary
            }

            if (possessed != null && possessTimer > possessTimerMax)
            {
                // enable/disable npc controllers
                possessed.GetComponent<NPCController>().enabled = false;
                possessed.GetComponent<PossessedController>().enabled = true;
                possessed.GetComponent<PossessedController>().PossessState(true);

                // teleport cat body elsewhere
                transform.position = new Vector3(0f, -10f, 0f);

                // make camera follow possessed
                var vcam = GameObject.Find("Third Person Camera").GetComponent<Cinemachine.CinemachineFreeLook>();
                vcam.Follow = possessed.transform;
                vcam.LookAt = possessed.transform;
            }
        }

        public void UpdatePossessionEnd()
        {
            // T to leave (temporarily)
            if (possessed != null && Input.GetKeyDown(KeyCode.F))
            {
                leaveBody = true;
                // Animation starts, timer is for the material
                GetComponent<Animator>().SetBool("isPossess", false);
                possessTimer = 0f;

                possessed.GetComponent<Animator>().SetTrigger("die");
                possessed.GetComponent<NPCController>().enabled = true;
                possessed.GetComponent<PossessedController>().enabled = false;

                // Teleport Cat back
                transform.position = possessed.GetComponent<PossessedController>().GetSpawnPoint().position;
                particles.Play();
            }

            if (leaveBody == true && possessTimer < possessTimerMax)
            {
                // i think nothing happens
            }

            if (leaveBody == true && possessTimer > possessTimerMax)
            {
                // I have control again
                leaveBody = false;
                possessed.GetComponent<PossessedController>().PossessState(false);

                var vcam = GameObject.Find("Third Person Camera").GetComponent<Cinemachine.CinemachineFreeLook>();
                vcam.Follow = transform;
                vcam.LookAt = transform;

                SetSkin(0, demonMat);
                possessed = null;
            }
        }

        private void UpdateAttack()
        {
            if (Input.GetButtonDown("Fire1") && timeSinceLastAttack > attackMaxTimer)
            {
                GetComponent<Animator>().SetTrigger("attack");
                isAttack = true;
                if (!demonForm)
                {
                    UpdateForm();
                }
            }
        }

        private void UpdateMovement()
        {
            float horiz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");

            direction = new Vector3(horiz, 0f, vert).normalized;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                sprintSpd = sprintMod;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                sprintSpd = 1f;
            }

            if (direction.magnitude >= 0.1f && isAttack == false)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * sprintSpd * Time.deltaTime);
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
            controller.Move(velocity * Time.deltaTime);
        }

        private void UpdateTimers()
        {
            timeSinceLastAttack += Time.deltaTime;
            possessTimer += Time.deltaTime;
        }

        void Hit() 
        {
            scythe.WeaponHit();
        }

        void StopHit()
        {
            scythe.WeaponStopHit();
            isAttack = false;
        }

        public bool getForm()
        {
            return demonForm;
        }

        private void SetSkin(int matNum, Material newMat)
        {
            Material[] mats = catRenderer.materials;
            mats[matNum] = newMat;
            catRenderer.materials = mats;
        }

        public void Respawn()
        {
            GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = initialPos;
            GetComponent<CharacterController>().enabled = true;
            //transform.position = initialPos;
            GetComponent<Animator>().SetTrigger("revive");
        }
    }
}