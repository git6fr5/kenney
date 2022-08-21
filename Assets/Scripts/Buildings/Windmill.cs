/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceGatherer;

namespace ResourceGatherer {

    ///<summary>
    ///
    ///<summary>
    public class Windmill : Building {

        public float goldRate;
        public Player player;

        private float ticks;

        public AudioClip goldSound;
        
        void FixedUpdate() {
            ticks += Time.fixedDeltaTime;
            if (ticks >= goldRate) {
                SoundManager.PlaySound(goldSound, 0.15f);
                player.inventory.gold += 1;
                ticks -= goldRate;
            }
        }

    }

}