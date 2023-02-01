using System.Collections;
using System.Collections.Generic;
using gameracers.Core;
using UnityEngine;

namespace gameracers.Camera
{
    public class Cutscene : MonoBehaviour
    {
        [SerializeField][Tooltip("if isSmooth, needs in multiple of 3 (pt1, pt2, pt3)")]
        Transform[] points;
        [SerializeField][Tooltip("Duration of each segment, set to 0 if instant jump. ")]
        float[] duration;
        [SerializeField] float endDuration = 0f;
        [SerializeField] bool isSmooth = false;
        [SerializeField] bool isStart = false;
        [SerializeField][Tooltip("If this is part of a sequence, put the next part here!")]
        Cutscene next;
        bool isEnd = false;
        int index = 0;
        int durIndex = 0;
        float timer;
        Transform mainCam;



        private void Start()
        {
            mainCam = GameObject.Find("Main Camera").transform;
            mainCam.gameObject.SetActive(false);
            timer = Time.time;
            if (points.Length >= 1)
            {
                transform.position = points[0].position;
                transform.eulerAngles = points[0].eulerAngles;
            }
        }

        private void Update()
        {
            if (isEnd == true)
            {
                EndCutscene();
                return;
            }

            if (isSmooth == false)
            {
                if (index < points.Length - 1)
                {
                    transform.position = Vector3.Lerp(points[index].position, points[index + 1].position, (Time.time - timer) / duration[durIndex]);
                    transform.eulerAngles = Vector3.Lerp(points[index].eulerAngles, points[index + 1].eulerAngles, (Time.time - timer) / duration[durIndex]);
                }
                else
                {
                    timer = Time.time;
                    isEnd = true;
                    EndCutscene();
                    return;
                }
            }

            if (isSmooth == true)
            {
                if (index < points.Length - 2)
                {
                    transform.position = Vector3.Lerp(
                        Vector3.Lerp(points[index].position, points[index + 1].position, (Time.time - timer) / duration[durIndex]),
                        Vector3.Lerp(points[index + 1].position, points[index + 2].position, (Time.time - timer) / duration[durIndex]),
                        (Time.time - timer) / duration[durIndex]);
                    transform.eulerAngles = Vector3.Lerp(
                        Vector3.Lerp(points[index].eulerAngles, points[index + 1].eulerAngles, (Time.time - timer) / duration[durIndex]),
                        Vector3.Lerp(points[index + 1].eulerAngles, points[index + 2].eulerAngles, (Time.time - timer) / duration[durIndex]),
                        (Time.time - timer) / duration[durIndex]);
                }
                else
                {
                    timer = Time.time;
                    isEnd = true;
                    EndCutscene();
                    return;
                }
            }

            if ((Time.time - timer) / duration[index] > duration[durIndex])
            {
                index++;
                durIndex++;
                if (isSmooth == true)
                    index++;
            }
        }

        private void EndCutscene()
        {
            if (next != null)
            {
                next.gameObject.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            if (isStart && Mathf.Approximately(Time.time - timer, 0f))
            {
                // Tell Cat to run the Spawn Animation
                Debug.Log("Cat! Run your spawn animation!");
            }
            if (Time.time - timer >= endDuration)
            {
                mainCam.gameObject.SetActive(true);
                if (isStart == true)
                {
                    GameManager.gm.UpdateGameState(GameState.Play);
                }
                gameObject.SetActive(false);
            }
        }
    }
}