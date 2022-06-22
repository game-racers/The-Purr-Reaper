using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Appearances
{
    public class ClothingEquip : MonoBehaviour
    {
        // Clothing Randomizer
        [SerializeField] List<GameObject> hats;
        [SerializeField] List<GameObject> shirts;
        [SerializeField] List<GameObject> pants;
        [SerializeField] List<GameObject> ties;
        [SerializeField] GameObject neckBone;

        private void Start()
        {
            if (hats.Count != 0)
            {
                int randHat = Random.Range(0, hats.Count);
                Instantiate(hats[randHat], gameObject.transform);
            }
            if (shirts.Count != 0)
            {
                int randShirt = Random.Range(0, shirts.Count);
                Instantiate(shirts[randShirt], gameObject.transform);
            }
            if (pants.Count != 0)
            {
                int randPants = Random.Range(0, pants.Count);
                Instantiate(pants[randPants], gameObject.transform);
            }
            if (ties.Count != 0)
            {
                int randTie = Random.Range(0, ties.Count);
                Instantiate(ties[randTie], neckBone.transform);
            }
        }
    }
}
