using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Core;
using UnityEngine.SceneManagement;

namespace gameracers.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Resume()
        {
            GameManager.gm.UpdateGameState(GameState.Play);
            Deactivate();
        }

        public void RestartCheckpt()
        {
            Deactivate();
            //GameManager.gm.UpdateGameState(GameState.Load);
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SaveGame()
        {
            //GameManager.gm.UpdateGameState(GameState.Save);
        }

        public void PhotoMode()
        {
            Deactivate();
            Debug.Log("TODO: Photo Mode");
            //GameManager.gm.UpdateGameState(GameState.PhotoMode);
        }

        public void ExitToMain()
        {
            Deactivate();
            SceneManager.LoadScene(0);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                EventListener.PauseGame(false);
        }
    }
}
