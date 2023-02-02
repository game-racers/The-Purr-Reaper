using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Camera
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] Transform playerCenter;
        [Range(270, 350)]
        [SerializeField] float maxForwardTilt = 315f;
        [Range(10, 90)]
        [SerializeField] float maxBackwardTilt = 45f;
        [SerializeField] public float mouseXRotMod = -1f;
        [SerializeField] public float mouseYRotMod = 1f;
        [SerializeField] string slideOrigin;
        bool isPaused = false;
        float deltaX;
        float deltaY;

        private void OnEnable()
        {
            EventListener.onSliderChange += ChangeSensitivity;
        }

        private void OnDisable()
        {
            EventListener.onSliderChange -= ChangeSensitivity;
        }

        private void Awake()
        {
            transform.eulerAngles = playerCenter.parent.eulerAngles;
        }

        void LateUpdate()
        {
            if (isPaused) return;
            deltaX = Input.GetAxis("Mouse X") * mouseXRotMod;
            deltaY = Input.GetAxis("Mouse Y") * mouseYRotMod;
            transform.RotateAround(transform.position, Vector3.up, deltaX);

            if (transform.eulerAngles.x + deltaY < maxForwardTilt && transform.eulerAngles.x > 180f)
            {
                transform.eulerAngles = new Vector3(maxForwardTilt, transform.eulerAngles.y, 0);
                return;
            }

            if (transform.eulerAngles.x + deltaY > maxBackwardTilt && transform.eulerAngles.x < 180f)
            {
                transform.eulerAngles = new Vector3(maxBackwardTilt, transform.eulerAngles.y, 0);
                return;
            }

            transform.RotateAround(transform.position, transform.right, deltaY);
        }

        private void ChangeSensitivity(string origin, float val)
        {
            Debug.Log("Hello " + origin);
            if (origin == slideOrigin)
            {
                Debug.Log("World");
                float temp = 1;
                if (mouseXRotMod < 0f)
                    temp = -1;
                mouseXRotMod = val * temp;
                if (mouseYRotMod < 0f)
                    temp = -1;
                mouseYRotMod = val * temp;
            }
        }

        public void FlipSensitivity(bool isX)
        {
            if (isX == true)
                mouseXRotMod *= -1;
            if (isX == false)
                mouseYRotMod *= -1;
        }
    }
}
