using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace gameracers.UI
{
    public class UpdateColour : MonoBehaviour
    {
        private void OnEnable()
        {
            EventListener.onColourPicker += ChangeColor;
        }

        private void OnDisable()
        {
            EventListener.onColourPicker -= ChangeColor;
        }

        private void ChangeColor(Vector4 newColor)
        {
            GetComponent<Image>().color = newColor;
        }
    }
}
