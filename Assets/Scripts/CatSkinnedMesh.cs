using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSkinnedMesh : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshPrefab;
    SkinnedMeshRenderer originalSkin;
    [SerializeField] Transform rootBone;

    void Start()
    {
        skinnedMeshPrefab = gameObject.GetComponent<SkinnedMeshRenderer>();
        originalSkin = gameObject.transform.parent.Find("Demon Cat").GetComponent<SkinnedMeshRenderer>();
        rootBone = gameObject.transform.parent.Find("Demon Cat").GetComponent<CatSkinnedMesh>().rootBone;

        skinnedMeshPrefab.bones = originalSkin.bones;
        skinnedMeshPrefab.rootBone = rootBone;
    }
}
