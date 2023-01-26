using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;
using gameracers.Combat;
using gameracers.Movement;
using gameracers.Abilities;


namespace gameracers.Control
{ 
    public class PlayerBrain : MonoBehaviour
    {
        // Player stuff
        Health health;
        PlayerFighter fighter;
        PlayerMoverTemp mover;
        PlayerPossess possessAbility;
        PlayerForms formsAbility;
        [SerializeField] ParticleSystem particles;

        // Possession
        GameObject possessed = null;
        bool canPossess = true;

        // new Possession
        GameObject possessedEntity = null;

        // Violence
        bool isAttack = false;

        //Respawn Point
        Vector3 spawnPoint;

        // Jump
        bool isGrounded = true;

        private void OnEnable()
        {
            EventListener.onJump += Jump;
        }

        private void OnDisable()
        {
            EventListener.onJump -= Jump;
        }

        private void Jump(Vector3 jumpVector)
        {
            isGrounded = false;
            mover.Jump(jumpVector);
        }

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            health = GetComponent<Health>();
            fighter = GetComponent<PlayerFighter>();
            mover = GetComponent<PlayerMoverTemp>();
            possessAbility = GetComponent<PlayerPossess>();
            formsAbility = GetComponent<PlayerForms>();

            spawnPoint = transform.position;
        }

        void Update()
        {
            if (health.GetDead()) return;

            if (isGrounded == false)
            {
                isGrounded = mover.UpdateJump();
                return;
            }

            UpdatePossession();
            if (possessed != null) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                UpdateForm();
            }

            UpdateAttack();

            mover.UpdateMover(isAttack);
        }

        private void UpdatePossession()
        {
            if (canPossess == true)
            {
                if (possessAbility.PossessionStart())
                {
                    PossessControl(true);
                }
            }
            else
            {
                if (possessAbility.PossessionEnd())
                {
                    PossessControl(false);
                }
            }
        }

        private void PossessControl(bool enabled)
        {
            if (!formsAbility.GetForm())
            {
                UpdateForm();
            }

            particles.Play();

            if (enabled == true)
            {
                possessedEntity = possessAbility.GetPossessed();
                GetComponent<CharacterController>().enabled = false;
                transform.position = new Vector3(0f, -10f, 0f);
                GetComponent<CharacterController>().enabled = true;
            }
            possessedEntity.GetComponent<HumanBrain>().enabled = !enabled;
            possessedEntity.GetComponent<PossessedController>().enabled = enabled;
            possessedEntity.GetComponent<PossessedController>().PossessState(enabled);
            canPossess = !enabled;

            if (enabled == false)
            {
                GetComponent<CharacterController>().enabled = false;
                transform.position = possessedEntity.GetComponent<PossessedController>().GetSpawnPoint();
                GetComponent<CharacterController>().enabled = true;
                possessedEntity = null;
            }
        }

        private void UpdateForm()
        {
            formsAbility.ChangeForm();
            particles.Play();
        }

        private void UpdateAttack()
        {
            // If in the air, cannot attack
            //if (mover.GetGrounded() == false) return;

            if (Input.GetButtonDown("Fire1"))
            {
                //isAttack = true;
                fighter.AttackBehavior();
                if (!formsAbility.GetForm())
                {
                    UpdateForm();
                }
            }
        }

        // Animator Functions maybe dont need
        void Hit()
        {
            isAttack = true;
        }

        void StopHit()
        {
            isAttack = false;
        }

        public void Respawn()
        {
            GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = spawnPoint;
            GetComponent<CharacterController>().enabled = true;
            //transform.position = initialPos;
            GetComponent<Animator>().SetTrigger("revive");
        }

        public bool getForm()
        {
            return formsAbility.GetForm();
        }
    }
}