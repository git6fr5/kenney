/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceGatherer;

namespace ResourceGatherer {

    ///<summary>
    ///
    ///<summary>
    public class Resource : MonoBehaviour {

        public enum Type {
            Gold,
            Wood,
            Clay,
            Stone
        }

        public Type type;
        public bool perishable;
        public int cost;

        public GameObject modelObject;

        public AudioClip growSound;
        public AudioClip destroySound;

        public bool mining;
        public float health;
        public float maxHealth;

        void Start() {
            mining = false;
            health = maxHealth;
        }

        public Resource Duplicate(int amount) {
            SoundManager.PlaySound(growSound, 0.15f);

            GameObject instance = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);

            // for (int i = 1; i < amount; i++) {
            //     GameObject extra = Instantiate(modelObject, modelObject.transform.position, Quaternion.identity, instance.transform);
            //     Vector3 offset = Random.insideUnitSphere;
            //     offset.y = 0f;
            //     offset *= 0.75f;
            //     extra.transform.position += offset;
            // }

            instance.SetActive(true);
            return instance.GetComponent<Resource>();
        }

    }

}