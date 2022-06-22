using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Camera
{
    public class FindObstructions : MonoBehaviour
    {
        [SerializeField] private List<FadeObstructors> currentlyInTheWay;
        [SerializeField] private List<FadeObstructors> currentlyTransparent;
        Transform player;
        Transform cam;

        void Start()
        {
            currentlyInTheWay = new List<FadeObstructors>();
            currentlyTransparent = new List<FadeObstructors>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            cam = transform;
        }

        void Update()
        {
            GetAllObjectsInTheWay();
            MakeObjectsSolid();
            MakeObjectsTransparent();
        }

        private void GetAllObjectsInTheWay()
        {
            /*
             * Summary: 
             *      Collects a list of all GameObjects with colliders that is between the Camera and the Player and collects them into a list. 
             */

            currentlyInTheWay.Clear();
            float camToPlayer = Vector3.Magnitude(cam.position - player.position);

            Ray ray1_Forward = new Ray(cam.position, player.position - cam.position);
            Ray ray1_Backward = new Ray(player.position, cam.position - player.position);   // catches if inside

            var hits1_Forward = Physics.RaycastAll(ray1_Forward, camToPlayer);
            var hits1_Backward = Physics.RaycastAll(ray1_Backward, camToPlayer);

            foreach (var hit in hits1_Forward)
            {
                if (hit.collider.gameObject.TryGetComponent(out FadeObstructors objectFader))
                {
                    if (!currentlyInTheWay.Contains(objectFader))
                    {
                        currentlyInTheWay.Add(objectFader);
                    }
                }
            }
            foreach (var hit in hits1_Backward)
            {
                if (hit.collider.gameObject.TryGetComponent(out FadeObstructors objectFader))
                {
                    if (!currentlyInTheWay.Contains(objectFader))
                    {
                        currentlyInTheWay.Add(objectFader);
                    }
                }
            }

        }

        private void MakeObjectsTransparent()
        {
            /*
             * Summary :
             *      Iterates through currentlyInTheWay, tests if the object contains FadeObstructor and then calls on that objects FadeObstructor to go to the transparent material. 
             */

            for (int i = 0; i < currentlyInTheWay.Count; i++)
            {
                FadeObstructors objectFader = currentlyInTheWay[i];

                if (!currentlyTransparent.Contains(objectFader))
                {
                    objectFader.ShowTransparent();
                    currentlyTransparent.Add(objectFader);
                }
            }
        }

        private void MakeObjectsSolid()
        {
            /*
             * Summary :
             *      Iterates through currentlyInTheWay, tests if the object contains FadeObstructor and then calls on that objects FadeObstructor to go to the regular material. 
             */

            for (int i = 0; i < currentlyTransparent.Count; i++)
            {
                FadeObstructors objectFader = currentlyTransparent[i];

                if (!currentlyInTheWay.Contains(objectFader))
                {
                    objectFader.ShowSolid();
                    currentlyTransparent.Remove(objectFader);
                }
            }
        }
    }
}
