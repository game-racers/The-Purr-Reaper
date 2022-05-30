using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gameracers.Stats;

public class Weapon : MonoBehaviour
{
    public float damage = 1f;

    public void WeaponHit()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    public void WeaponStopHit()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Health>() != null)
        {
            other.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
