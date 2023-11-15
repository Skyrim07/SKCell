using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    //Object with this component occupies a certain amount of space in a grid.
    [RequireComponent(typeof(BoxCollider2D))]
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Grid/SKGridOccupier")]
    public class SKGridOccupier : MonoBehaviour
    {
        [Header("Settings")]
        public SKGridLayer gridLayer;
        [Tooltip("If true, cells occupied by this will be set as cost 1(unwalkable).")]
        public bool isCellCost = true;
        public List<Vector2Int> occupiedCells = new List<Vector2Int>();

        private BoxCollider2D cld;
        private void Start()
        {
            CalculateOccupiedCells();
        }

        public void CalculateOccupiedCells()
        {
            if (gridLayer == null)
                return;

            cld = GetComponent<BoxCollider2D>();
            Vector2 min = gridLayer.grid.PositionToCell(cld.bounds.min);
            Vector2 max = gridLayer.grid.PositionToCell(cld.bounds.max);
            occupiedCells = gridLayer.CellsFromCoordinate(min.ToVector2Int(), max.ToVector2Int());

            if(isCellCost)
                gridLayer.PathfindingSetCellCost(occupiedCells, 1);
            gridLayer.SetCellOccupancy(occupiedCells, true);
        }

        public void CalculateAndDisplayOccupiedCells()
        {
            StopEditorDisplay();
            CalculateOccupiedCells();
            gridLayer.SetCellColor(occupiedCells, new Color(1,1,1,0.7f));
        }

        public void StopEditorDisplay()
        {
            gridLayer.EraseCellColor(occupiedCells);
        }
    }
}
