using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    Animator anim;
    [SerializeField] LayerMask npcMask;

    void Start()
    {
        anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (IsInLayerMask(collision.gameObject, npcMask))
        {
            anim.SetBool("isOpen", true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (IsInLayerMask(collision.gameObject, npcMask))
        {
            anim.SetBool("isOpen", false);
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
}
