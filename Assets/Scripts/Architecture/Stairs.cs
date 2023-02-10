using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Architecture
{
    public class Stairs : MonoBehaviour
    {
        Transform top;
        Transform bottom;

        private void Awake()
        {
            top = transform.Find("Top");
            bottom = transform.Find("Bottom");
        }

        public bool IsDownStairs(float posY)
        {
            if (posY - top.position.y > -1.5f) return true;
            return true;
        }

        public Vector3 GetStairsPoint(bool isTop)
        {
            if (isTop == true)
                return top.position;
            else
                return bottom.position;
        }
    }
}
