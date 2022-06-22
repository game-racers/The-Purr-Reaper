using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Appearances
{ 
    public class ClothSkinnedMesh : MonoBehaviour
    {
        SkinnedMeshRenderer skinnedMeshPrefab;
        SkinnedMeshRenderer originalSkin;
        [SerializeField] Transform rootBone;

        void Start()
        {
            skinnedMeshPrefab = gameObject.GetComponent<SkinnedMeshRenderer>();
            originalSkin = gameObject.transform.parent.Find("Human").GetComponent<SkinnedMeshRenderer>();
            rootBone = gameObject.transform.parent.Find("Human").GetComponent<ClothSkinnedMesh>().rootBone;

            skinnedMeshPrefab.bones = originalSkin.bones;
            skinnedMeshPrefab.rootBone = rootBone;
        }
    }
}