using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Appearances
{
    public class ClothingEquip : MonoBehaviour
    {
        // Clothing Randomizer
        List<GameObject> hats;
        List<GameObject> shirts;
        List<GameObject> pants;
        List<GameObject> ties;
        [SerializeField] GameObject neckBone;

        private void Start()
        {
            if (hats.Count != 0)
            {
                int randHat = Random.Range(0, hats.Count);
                if (hats[randHat] != null)
                    Instantiate(hats[randHat], gameObject.transform);
            }
            if (shirts.Count != 0)
            {
                int randShirt = Random.Range(0, shirts.Count);
                if (shirts[randShirt] != null)
                    Instantiate(shirts[randShirt], gameObject.transform);
            }
            if (pants.Count != 0)
            {
                int randPants = Random.Range(0, pants.Count);
                if (pants[randPants] != null)
                    Instantiate(pants[randPants], gameObject.transform);
            }
            if (ties.Count != 0)
            {
                int randTie = Random.Range(0, ties.Count);
                if (ties[randTie] != null)
                    Instantiate(ties[randTie], neckBone.transform);
            }
        }

        public void initClothes(List<GameObject> newHats, List<GameObject> newShirts, List<GameObject> newPants, List<GameObject> newTies)
        {
            hats = newHats;
            shirts = newShirts;
            pants = newPants;
            ties = newTies;
        }
    }
}
