using gameracers.Control;
using UnityEngine;
using UnityEngine.UI;
using gameracers.Stats;

namespace gameracers.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        [SerializeField] GameObject deathScreen;
        [SerializeField] float healthPoints;
        [SerializeField] float maxHealthPoints;

        public Image[] hearts;
        [SerializeField] Sprite emptyHeart;
        [SerializeField] Sprite fullHeart;

        [SerializeField] Image demonPic;
        [SerializeField] Image livingPic;

        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            maxHealthPoints = health.GetMaxHP();
            healthPoints = health.GetHealthPoints();
        }

        void Update()
        {
            healthPoints = health.GetHealthPoints();

            for (int i = 0; i < hearts.Length; i++)
            {
                if (i < healthPoints)
                {
                    hearts[i].sprite = fullHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }

                if (i < maxHealthPoints)
                {
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }

            if (health.GetComponent<PlayerBrain>().getForm())
            {
                demonPic.enabled = true;
                livingPic.enabled = false;
            }
            else
            {
                demonPic.enabled = false;
                livingPic.enabled = true;
            }

            if (health.GetDead())
            {
                Die();
            }
        }

        private void Die()
        {
            deathScreen.SetActive(true);
        }
    }
}
