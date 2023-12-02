using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Grid", menuName = "SKCell/Grid Asset", order = 0)]
    public class SKGridAsset : ScriptableObject
    {
        public SKGrid grid;
        public int resolution;
        public FilterMode filterMode;
        public Color defaultColor;

        public float[] cellValues_se;
        public int[] cellCosts_se;
        public bool[] cellOccupied_se;


        public SKGridAsset() { }
        public SKGridAsset(SKGrid grid)
        {
            this.grid = grid;
        }

        public SKGridAsset(SKGrid grid, int resolution, FilterMode filterMode, Color defaultColor)
        {
            this.grid = grid;
            this.resolution = resolution;
            this.filterMode = filterMode;
            this.defaultColor = defaultColor;

            cellValues_se = SKUtils.Serialize2DArray(grid.cellValues);
            cellCosts_se = SKUtils.Serialize2DArray(grid.pf_CellCost);
            cellOccupied_se = SKUtils.Serialize2DArray(grid.occupied);
        }
    }
}
