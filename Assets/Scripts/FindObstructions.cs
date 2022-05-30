using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
