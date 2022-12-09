using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAlertButton : MonoBehaviour
{
    [SerializeField] GameObject alertLight;
    [SerializeField] int buttonID;

    private void OnEnable()
    {
        EventListener.onDemonButton += SetDemonAlert;
    }

    private void OnDisable()
    {
        EventListener.onDemonButton -= SetDemonAlert;
    }

    private void SetDemonAlert(int id)
    {
        if (buttonID == id)
        {
            //animator switch states
        }
        alertLight.gameObject.SetActive(true);
    }

    public int GetButtonID()
    {
        return buttonID;
    }
}
