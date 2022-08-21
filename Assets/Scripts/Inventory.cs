/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceGatherer;

namespace ResourceGatherer {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class Inventory {

        public int gold;
        public int wood;
        public int clay;
        public int stone;

        public void Add(int value, Resource resource) {
            switch (resource.type) {
                case Resource.Type.Gold:
                    gold += value;
                    return;
                case Resource.Type.Wood:
                    wood += value;
                    return;
                case Resource.Type.Clay:
                    clay += value;
                    return;
                case Resource.Type.Stone:
                    stone += value;
                    return;
            }
        }

    }

}