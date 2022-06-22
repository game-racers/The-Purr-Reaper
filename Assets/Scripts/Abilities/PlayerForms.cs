using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerForms : MonoBehaviour
{
    bool isDemonForm = true;
    GameObject demonCat;
    GameObject regularCat;

    void Awake()
    {
        demonCat = transform.Find("Purr Reaper").Find("Demon Cat").gameObject;
        regularCat = transform.Find("Purr Reaper").Find("Live Cat").gameObject;
    }

    public void ChangeForm()
    {
        /*
        Summary: 
            Changes the overall appearance and model of the Cat between the Demonic appearance and the Tuxedo Cat style. 
         */

        demonCat.SetActive(!isDemonForm);
        regularCat.SetActive(isDemonForm);
        isDemonForm = !isDemonForm;
    }

    public bool GetForm()
    {
        /*
        Summary: 
            Getter for isDemonForm
        Returns: 
            True if in Demon Form; 
         */

        return isDemonForm;
    }
}
