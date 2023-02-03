using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace gameracers.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioMixer master;
        [SerializeField] AudioMixerGroup sounds;
        [SerializeField] AudioMixerGroup voices;
        [SerializeField] AudioMixerGroup background;
        [SerializeField] string masterVol = "Master Vol";
        [SerializeField] string soundsVol = "Sounds Vol";
        [SerializeField] string voicesVol = "Voices Vol";
        [SerializeField] string bgVol = "Music Vol";

        private void OnEnable()
        {
            EventListener.onSliderChange += AdjustVolume;
        }

        private void OnDisable()
        {
            EventListener.onSliderChange -= AdjustVolume;
        }

        private void AdjustVolume(string origin, float val)
        {
            if (origin == masterVol)
                master.SetFloat(origin, Mathf.Log10(val) * 20);
            if (origin == soundsVol)
                master.SetFloat(origin, Mathf.Log10(val) * 20);
            if (origin == voicesVol)
                master.SetFloat(origin, Mathf.Log10(val) * 20);
            if (origin == bgVol)
                master.SetFloat(origin, Mathf.Log10(val) * 20);
        }
    }
}
