using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.UI
{
    public class UpdateColour : MonoBehaviour
    {
        [SerializeField] string size = "Crosshair Size";

        private void OnEnable()
        {
            EventListener.onColourPicker += ChangeColor;
            EventListener.onSliderChange += AdjustSize;
        }

        private void OnDisable()
        {
            EventListener.onColourPicker -= ChangeColor;
            EventListener.onSliderChange -= AdjustSize;
        }

        private void ChangeColor(Vector4 newColor)
        {
            GetComponent<Image>().color = newColor;
        }

        private void AdjustSize(string origin, float val)
        {
            if (origin == size)
            {
                GetComponent<RectTransform>().parent.localScale = Vector3.one * val;
            }
        }
    }
}
