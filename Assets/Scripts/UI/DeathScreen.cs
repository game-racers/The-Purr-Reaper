using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using gameracers.Stats;
using gameracers.Control;

namespace gameracers.UI
{ 
    public class DeathScreen : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;
        float fadeTimer;
        [SerializeField] float maxAlpha = .5f;
        [SerializeField] Image background;
        [SerializeField] GameObject[] deathUI;


        void Start()
        {
            fadeTimer = Time.time;
        }

        void Update()
        {
            var tempColor = background.color;

            if (Time.time - fadeTimer < fadeInTime)
            {
                tempColor.a = (Time.time - fadeTimer) / fadeInTime * maxAlpha;
            }
            else
            {
                tempColor.a = maxAlpha;
                for (int i = 0; i < deathUI.Length; i++)
                {
                    deathUI[i].SetActive(true);
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            background.color = tempColor;
        }

        // Buttons!

        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void RespawnPlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");

            player.GetComponent<Health>().Heal();
            player.GetComponent<PlayerBrain>().Respawn();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            fadeTimer = 0f;
            gameObject.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}