using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.UI
{
    public class MyToggle : MonoBehaviour
    {
        bool isOn = false;

        public void Toggle()
        {
            isOn = !isOn;
            transform.GetChild(1).GetChild(0).gameObject.SetActive(isOn);
        }
    }
}
