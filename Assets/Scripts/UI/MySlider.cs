using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.UI
{
    public class MySlider : MonoBehaviour
    {
        Slider slider;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField][Tooltip("Origin and recieving names must match!")] string originName;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        void Start()
        {
            slider.onValueChanged.AddListener((v) =>
            {
                EventListener.SliderChange(originName, v);
                if (text != null)
                    text.text = v.ToString("0.00");
            });
        }
    }
}
