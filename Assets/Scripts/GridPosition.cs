/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceGatherer;

namespace ResourceGatherer {

    [System.Serializable]
    public class GridPosition {

        public static float h = 0.2f;

        public static float w = 1f;
        public static float w1 => w / 2f;
        public static float w2 =>  w1 / Mathf.Cos(Mathf.PI / 6f);
        public static float w3 => w2 * Mathf.Sin(Mathf.PI / 6f);

        public int col;
        public int row;
        public Vector3 worldOrigin => ToWorld(this);

        public GridPosition(int col, int row) {
            this.col = col;
            this.row = row;
        }

        public override string ToString() {
            return "GridPosition: Col " + col.ToString() + ", Row " + row.ToString();
        }

        public bool SameAs(GridPosition gridPos) {
            return col == gridPos.col && row == gridPos.row;
        }

        public static GridPosition FromWorld(Vector3 v) {
            float _row = v.z / (w2 + w3);
            float _col = v.x / (2f * w1) - (Mathf.Abs(_row) % 2f) / 2f;
            // int row = (int)(Mathf.Sign(_row) * Mathf.Round(Mathf.Abs(_row)));
            // int col = (int)(Mathf.Sign(_col) * Mathf.Round(Mathf.Abs(_col)));
            int row = (int)Mathf.Round(_row);
            int col = (int)Mathf.Round(_col);
            return new GridPosition(col, row);
        }

        public static Vector3 ToWorld(GridPosition g) {
            float x = w1 * (2f * g.col + (g.row % 2));
            float z = g.row * (w2 + w3);
            return new Vector3(x, 0, z);
        }

    }

}