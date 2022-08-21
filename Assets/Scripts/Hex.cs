/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResourceGatherer;

namespace ResourceGatherer {

    ///<summary>
    ///
    ///<summary>
    public class Hex : MonoBehaviour {

        public int cost;
        public GridPosition gridPosition;

        public Resource resource;
        [HideInInspector] public Resource activeResource;

        public int height;

        public float growTicks;
        public float growRate;

        public bool active;

        public GameObject modelObject;

        public Slider slider;

        public Player player;

        void FixedUpdate() {
            slider.transform.parent.parent.gameObject.SetActive(false);

            if (active) {
                transform.localScale = new Vector3(0.85f, 1f, 0.85f);
                if (activeResource == null) {
                    return;
                }
            }
            else {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (activeResource == null) {
                UpdateGrow(Time.fixedDeltaTime);
            }
            else {
                UpdateMine(Time.fixedDeltaTime);
            }

        }

        private void UpdateGrow(float dt) {
            
            slider.transform.parent.parent.gameObject.SetActive(true);
            slider.value = growTicks / growRate;

            growTicks += dt;
            if (growTicks >= growRate) {
                Grow(height);
                growTicks = 0f;
            }

        }

        private void UpdateMine(float dt) {
            if (!activeResource.mining) {
                activeResource.health = activeResource.maxHealth;
                return;
            }

            
            slider.transform.parent.parent.gameObject.SetActive(true);
            slider.value = activeResource.health / activeResource.maxHealth;

            activeResource.health -= dt;
            if (activeResource.health <= 0) {
                FinishMine();
            }

        }

        private void FinishMine() {

            player.inventory.Add(activeResource.cost, activeResource);
            SoundManager.PlaySound(activeResource.destroySound, 0.15f);
            Destroy(activeResource.gameObject);

        }

        public void Grow(int height) {
            activeResource = resource.Duplicate(height);
            activeResource.cost = height;
        }

        public void Addbuilding(Resource building) {
            activeResource = building.Duplicate(height);
            activeResource.enabled = true;
        }

        public void Stack() {
            growTicks = 0f;
            transform.position += GridPosition.h * Vector3.up;

            GameObject underObject = Instantiate(modelObject, Vector3.zero, Quaternion.identity, transform);
            Vector3 underPosition = height * GridPosition.h * Vector3.down;
            underObject.transform.localPosition = underPosition;

            height += 1;
            resource.cost = height;

        }

    }

}