/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceGatherer;

namespace ResourceGatherer {

    ///<summary>
    ///
    ///<summary>
    public class Player : MonoBehaviour {

        public enum Mode {
            Growing, Building, Count
        }

        public Mode mode;

        private Plane controlPlane;
        
        public int indicatorIndex;
        public Hex[] indicatorHexes;
        private Hex indicatorHex => indicatorHexes[indicatorIndex];
        public List<Hex> hexes;

        public int buildingIndex;
        public Building[] buildingIndicators;
        private Building building => buildingIndicators[buildingIndex];
        public List<Building> buildings;

        public GridPosition hoverGridPos;
        public Hex hoveredHex;

        public Inventory inventory;

        public bool leftMouseDown;
        public bool rightMouseDown;

        public Transform actualIndicator;
        public Material actualIndicatorMat;

        public AudioClip stackTileSound;
        public AudioClip startMiningSound;
        public AudioClip cancelMiningSound;
        public AudioClip startBuildingSound;
        public AudioClip buyTileSound;


        void Start() {
            controlPlane = new Plane(Vector3.up, Vector3.zero);
        }

        void Update() {

            leftMouseDown = Input.GetMouseButtonDown(0);
            rightMouseDown = Input.GetMouseButtonDown(1);
            bool next = Input.GetKeyDown(KeyCode.Tab);
            bool switchMode = Input.GetKeyDown(KeyCode.Space);

            // Switch Modes.
            if (switchMode) {
                mode = (Mode)(((int)mode + 1) % (int)Mode.Count);
                for (int i = 0; i < indicatorHexes.Length; i++) {
                    indicatorHexes[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < buildingIndicators.Length; i++) {
                    buildingIndicators[i].gameObject.SetActive(false);
                }
                switch (mode) {
                    case Mode.Growing:
                        break;
                    case Mode.Building:
                        building.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }

            // Scroll between the options.
            if (next) {

                for (int i = 0; i < indicatorHexes.Length; i++) {
                    indicatorHexes[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < buildingIndicators.Length; i++) {
                    buildingIndicators[i].gameObject.SetActive(false);
                }

                switch (mode) {
                    case Mode.Growing:
                        indicatorIndex = (indicatorIndex + 1) % indicatorHexes.Length;
                        break;
                    case Mode.Building:
                        buildingIndex = (buildingIndex + 1) % buildingIndicators.Length;
                        building.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }

            }

            // Get the point that the player is hovering over.
            float cast = 0.0f;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool successfulCast = controlPlane.Raycast(ray, out cast);
            if (successfulCast) {

                Vector3 hitPoint = ray.GetPoint(cast);
                hoverGridPos = GridPosition.FromWorld(hitPoint);
                Vector3 snappedworldPos = hoverGridPos.worldOrigin;

                Hex newHoveredHex = hexes.Find(hex => hex.gridPosition.SameAs(hoverGridPos));
                if (newHoveredHex != hoveredHex) {
                    if (hoveredHex != null) {
                        hoveredHex.active = false;
                    }
                    if (newHoveredHex != null) {
                        newHoveredHex.active = true;
                    }
                }

                Vector3 snappedworldPosWHeight = snappedworldPos;
                if (hoveredHex != null) {
                    snappedworldPosWHeight += GridPosition.h * hoveredHex.height * Vector3.up;
                }

                hoveredHex = newHoveredHex;

                actualIndicator.transform.position = snappedworldPos;
                switch (mode) {
                    case Mode.Growing:
                        indicatorHex.transform.localScale = new Vector3(0.75f, 1f, 0.75f);
                        indicatorHex.transform.position = snappedworldPosWHeight;
                        break;
                    case Mode.Building:
                        building.transform.position = snappedworldPosWHeight;
                        break;
                    default:
                        break;
                }
                
                // When the player is hovering over nothing.
                // Buying new hexes.
                if (hoveredHex == null) {

                    switch (mode) {
                        case Mode.Growing:
                            indicatorHex.gameObject.SetActive(true);
                            bool affordable = inventory.gold >= indicatorHex.cost;

                            // Color indicator.
                            if (affordable) {
                                actualIndicatorMat.color = Color.yellow; 
                            }
                            else {
                                actualIndicatorMat.color = Color.red; 
                            }

                            if (leftMouseDown && affordable) {
                                Buy();
                            }
                            break;
                        case Mode.Building:
                            // Color indicator.
                            actualIndicatorMat.color = Color.red; 
                            break;
                        default:
                            break;
                    }
                }
                // When the player is hovering a tile.
                // Collecting resources or growing hexes.
                else {
                    indicatorHex.gameObject.SetActive(false);

                    bool hasResource = hoveredHex.activeResource != null;
                    // bool sameType = indicatorHex 

                    // Color indicator.
                    if (hasResource) {
                        actualIndicatorMat.color = Color.red; 
                        if (mode == Mode.Building) {
                            building.gameObject.SetActive(false);
                        }
                    }
                    else {
                        actualIndicatorMat.color = Color.green; 
                        indicatorHex.gameObject.SetActive(true);
                        bool stackAffordable = inventory.gold >= (hoveredHex.cost * hoveredHex.height);
                        bool canStack = hoveredHex.height < 3;
                        if (mode == Mode.Growing && (!stackAffordable || !canStack)) {
                            actualIndicatorMat.color = Color.red; 
                        }
                        if (mode == Mode.Building && !building.Affordable(inventory)) {
                            actualIndicatorMat.color = Color.red;
                        }
                        if (mode == Mode.Building) {
                            building.gameObject.SetActive(true);
                        }
                    }

                    if (rightMouseDown && hasResource) {
                        Mine();
                    }
                    else if (leftMouseDown && !hasResource) {
                        
                        switch (mode) {
                            case Mode.Growing:
                                // indicatorHex.transform.position = snappedworldPosWHeight;
                                bool affordable = inventory.gold >= (hoveredHex.cost * hoveredHex.height);
                                bool stackable = hoveredHex.height < 3;
                                if (affordable && stackable) {
                                    Stack();
                                }
                                break;
                            case Mode.Building:
                                bool affordable0 = building.Affordable(inventory);
                                if (affordable0) {
                                    Build();
                                }                                
                                break;
                            default:
                                break;
                        }
                        
                        

                    }
                }
                
            }

            switch (mode) {
                case Mode.Growing:
                    building.gameObject.SetActive(false);
                    break;
                case Mode.Building:
                    indicatorHex.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }

        }

        private void Stack() {
            SoundManager.PlaySound(stackTileSound, 0.15f);
            inventory.gold -= hoveredHex.height;
            hoveredHex.Stack();
        }

        private void Buy() {
            SoundManager.PlaySound(buyTileSound, 0.15f);
            Hex hexInstance = Add(transform, indicatorHex, hoverGridPos, ref hexes);
            hexInstance.player = this;
            inventory.gold -= indicatorHex.cost;
        }

        private void Mine() {
            if (hoveredHex.activeResource.mining) {
                return;
            }

            if (hoveredHex.activeResource.mining) {
                SoundManager.PlaySound(startMiningSound, 0.15f);
            }
            else {
                SoundManager.PlaySound(cancelMiningSound, 0.15f);
            }
            hoveredHex.activeResource.mining = !hoveredHex.activeResource.mining;
        }

        private void Build() {
            SoundManager.PlaySound(startBuildingSound, 0.15f);
            hoveredHex.Addbuilding(building);
            building.Use(inventory);
        }

        public static Hex Add(Transform parent, Hex hex, GridPosition gridPos, ref List<Hex> hexes) {
            Vector3 snappedworldPos = gridPos.worldOrigin;
            GameObject instance = Instantiate(hex.gameObject, snappedworldPos, Quaternion.identity, parent);

            Hex hexInstance = instance.GetComponent<Hex>();
            hexInstance.enabled = true;
            hexInstance.gridPosition = gridPos;
            hexInstance.height = 1;
            hexes.Add(hexInstance);
            return hexInstance;
        }

        // public static void CreateRow(Transform parent, Hex hex, int row = 0, int length = 10, bool bidirectional = false) {
        //     int origin = bidirectional ? -length : 0;
        //     for (int i = origin; i < length; i++) {
        //         GridPosition gridPos = new GridPosition(i, row);
        //         Add(parent, gameObject, gridPos);
        //     }
        // }


    }

}