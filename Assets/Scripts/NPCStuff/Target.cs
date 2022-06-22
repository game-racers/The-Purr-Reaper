using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;
using UnityEngine.SceneManagement;

namespace gameracers.NPCStuff
{ 
    public class Target : MonoBehaviour
    {
        Health health;
        [SerializeField] float deathTimer = 3f;
        float timer = 0f;

        void Start()
        {
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.GetDead())
            {
                timer += Time.deltaTime;
                if (timer > deathTimer)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}