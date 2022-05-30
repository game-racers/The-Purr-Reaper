using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObstructors : MonoBehaviour
{
    [SerializeField] Material transparentMat;
    Material solidMat;

    void Awake()
    {
        solidMat = GetComponent<Renderer>().material;
    }

    public void ShowTransparent()
    {
        GetComponent<Renderer>().material = transparentMat;
    }

    public void ShowSolid()
    {
        GetComponent <Renderer>().material = solidMat;
    }
}
