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

        private void OnEnable()
        {
            EventListener.onColourPicker += AdjustSlider;
        }

        private void OnDisable()
        {
            EventListener.onColourPicker -= AdjustSlider;
        }

        private void AdjustSlider(Vector4 colour)
        {
            if (originName == "Crosshair Size") return;
            if (originName == "Red")
                slider.value = colour.x;
            if (originName == "Green")
                slider.value = colour.y;
            if (originName == "Blue")
                slider.value = colour.z;
            if (originName == "Alpha")
                slider.value = colour.w;
        }

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
