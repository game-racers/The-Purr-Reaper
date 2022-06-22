using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.UI
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] Slider volumeSlider;
        private float musicVolume = 1f;

        private void Start()
        {
            musicVolume = PlayerPrefs.GetFloat("volume");
        }

        private void Update()
        {
            AudioListener.volume = volumeSlider.value;
            PlayerPrefs.SetFloat("volume", musicVolume);
        }

        public void ChangeVolume(float volume)
        {
            musicVolume = volume;
        }
    }
}
