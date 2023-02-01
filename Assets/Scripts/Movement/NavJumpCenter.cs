using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Movement
{
    [ExecuteInEditMode]
    public class NavJumpCenter : MonoBehaviour
    {
        [SerializeField] float height = 0f;
        Transform start;
        Transform stop;

        void Awake()
        {
            start = transform.parent.Find("Start").Find("Start Land");
            stop = transform.parent.Find("End").Find("End Land");
        }

        void Update()
        {
            float newY = Mathf.Max(start.position.y, stop.position.y);
            newY += Mathf.Abs(start.position.z - stop.position.z) / 2 * ((Mathf.Cos(Mathf.PingPong(transform.eulerAngles.y, 90f) / 90f) + 1) / 2);
            newY += Mathf.Abs(start.position.x - stop.position.x) / 2 * ((Mathf.Sin(Mathf.PingPong(transform.eulerAngles.y, 90f) / 90f) + 1) / 2);
            newY += height;
            transform.position = new Vector3(Mathf.Lerp(start.position.x, stop.position.x, .5f), newY, Mathf.Lerp(start.position.z, stop.position.z, .5f));
        }
    }
}
