/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceGatherer;

namespace ResourceGatherer {

    ///<summary>
    ///
    ///<summary>
    public class Building : Resource {

        public int requiredWood;
        public int requiredClay;
        public int requiredStone;

        public bool Affordable(Inventory inventory) {
            bool wood = requiredWood <= inventory.wood;
            bool clay = requiredClay <= inventory.clay;
            bool stone = requiredStone <= inventory.stone;
            return wood && clay && stone;
        }

        public void Use(Inventory inventory) {
            inventory.wood -= requiredWood;
            inventory.clay -= requiredClay;
            inventory.stone -= requiredStone;
        }

    }

}