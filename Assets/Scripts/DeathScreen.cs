using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using gameracers.Stats;
using gameracers.Control;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] float fadeInTime = 1f;
    float fadeTimer = 0f;
    [SerializeField] float fadeMod = 1f;
    [SerializeField] float maxAlpha = .5f;
    [SerializeField] Image background;
    [SerializeField] GameObject[] deathUI;


    void Start()
    {
        fadeTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        fadeTimer += Time.deltaTime / fadeMod;

        var tempColor = background.color;

        if (fadeTimer < fadeInTime)
        {
            tempColor.a = fadeTimer / fadeInTime * maxAlpha;
        }
        else
        {
            tempColor.a = maxAlpha;
            for (int i = 0; i < deathUI.Length; i++)
            {
                deathUI[i].SetActive(true);
            }
            Cursor.lockState = CursorLockMode.None;
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
        player.GetComponent<PlayerController>().Respawn();
        Cursor.lockState = CursorLockMode.Locked;

        fadeTimer = 0f;
        gameObject.SetActive(false);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
