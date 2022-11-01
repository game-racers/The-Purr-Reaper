using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Combat
{
    [CreateAssetMenu(fileName = "Weapons", menuName = "Data/Weapon Data", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        public int damage = 1;
        public float weaponRange = 4f;
        public GameObject equipPrefab;
    }
}
