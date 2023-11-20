using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// 2D grid system.
    /// </summary>
    [Serializable]
    public class SKGrid
    {
        public float cellSize;
        public int width, height;
        public Vector3 bottomLeft;

        [SerializeField]
        public float[,] cellValues;
        
        //Occupancy
        public bool[,] occupied;


        //Pathfinding
        public bool pf_Initialized;
        public int[,] pf_ParentCell; //0:Left, 1:Right, 2:Up, 3:Down, 4:LU, 5:LD, 6:RU, 7:RD
        public float[,] pf_FValue;//F(x)=G(x)+H(x)
        public int[,] pf_CellCost;//0:Free to walk, 1:Unwalkable
        public Vector2Int pf_startPoint, pf_endPoint;

        public Vector3 WorldCenterPos
        {
            get
            {
                return new Vector3(bottomLeft.x + width * cellSize / 2f, bottomLeft.y + height * cellSize / 2f);
            }
        }
        public Vector3 LocalCenterPos
        {
            get
            {
                return WorldCenterPos - bottomLeft;
            }
        }
        public SKGrid(int width, int height, float cellSize, Vector3 bottomLeft)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.bottomLeft = bottomLeft;

            cellValues = new float[width, height];
            pf_CellCost = new int[width, height];
            occupied = new bool[width, height];
        }

        public void InitializePathfinding()
        {
            pf_ParentCell = new int[width, height];
            pf_FValue = new float[width, height];
            pf_startPoint = new Vector2Int(-1, -1);
            pf_endPoint = new Vector2Int(-1, -1);

            pf_Initialized = true;
        }

        public void EndPathfindingSession()
        {
            pf_Initialized = false;
        }

        public void SetCellValue(int x, int y, float value)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;
            cellValues[x, y] = value;
        }

        public float GetCellValue(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return -1;
            return cellValues[x, y];
        }
        public Vector2Int PositionToCell(Vector3 pos)
        {
            float x = pos.x - bottomLeft.x;
            float y = pos.y - bottomLeft.y;
            if (x < -cellSize/2f || x >= width * cellSize || y < -cellSize / 2f || y >= height * cellSize)
                return new Vector2Int(-1, -1);

            return new Vector2Int(Mathf.Clamp(Mathf.RoundToInt(x / cellSize),0,width-1), Mathf.Clamp(Mathf.RoundToInt(y / cellSize),0,height-1));
        }

        public Vector3 CellToPosition(Vector2Int cell)
        {
            return GetCellCenterPos(cell.x, cell.y);
        }

        /// <summary>
        /// Get cells from cell coordinates.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public List<Vector2Int> GetCells(int x1, int x2, int y1, int y2)
        {
            List<Vector2Int> res = new List<Vector2Int>();
            int xmin = Mathf.Min(x1, x2);
            int xmax = Mathf.Max(x1, x2);
            int ymin = Mathf.Min(y1, y2);
            int ymax = Mathf.Max(y1, y2);
            for (int i = xmin; i <= xmax; i++)
            {
                for (int j = ymin; j <= ymax; j++)
                {
                    res.Add(new Vector2Int(i, j));
                }
            }
            return res;
        }

        /// <summary>
        /// Get cells from world position.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public List<Vector2Int> GetCells(Vector3 v1, Vector3 v2)
        {
            Vector2Int bl = PositionToCell(v1);
            Vector2Int tr = PositionToCell(v2);
            return GetCells(bl.x, tr.x, bl.y, tr.y);
        }

        public Vector3 GetCellCenterPos(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return Vector3.zero;

            return new Vector3(bottomLeft.x + x * cellSize, bottomLeft.y + y * cellSize);
        }

        public Vector3 GetCellBottomLeftPos(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return Vector3.zero;

            return new Vector3(bottomLeft.x + x * cellSize - cellSize / 2, bottomLeft.y + y * cellSize - cellSize / 2);
        }
        public Vector3 GetGridCenterPos()
        {
            return new Vector3(bottomLeft.x + width / 2f, bottomLeft.y + height / 2f);
        }

        public Vector2 GetMousePosCell(Camera cam)
        {
            Vector2 mouseW = cam.ScreenToWorldPoint(Input.mousePosition);
            return PositionToCell(mouseW);
        }

        public int CellDistance(Vector2Int c1, Vector2Int c2)
        {
            return Mathf.Abs(c1.x - c2.x) + Mathf.Abs(c1.y - c2.y);
        }
        public float CellDistanceCir(Vector2Int c1, Vector2Int c2)
        {
            return Mathf.Sqrt(Mathf.Pow(c1.x - c2.x, 2) + Mathf.Pow(Mathf.Abs(c1.y - c2.y), 2));
        }
        public float CellDistanceX(Vector2Int c1, Vector2Int c2)
        {
            return Mathf.Abs(c1.x - c2.x);
        }
        public float CellDistanceY(Vector2Int c1, Vector2Int c2)
        {
            return Mathf.Abs(c1.y - c2.y);
        }
        public float PF_GetFValue(Vector2Int cell, Vector2Int parent, PathfindingAlgorithm algo, bool isCross = false)
        {
            switch (algo)
            {
                case PathfindingAlgorithm.AStar:
                    return pf_FValue[parent.x, parent.y] + PF_GetHValue(cell);
                case PathfindingAlgorithm.BStar:
                    return PF_GetHValue(cell);
                case PathfindingAlgorithm.BreadthFirst:
                    return PF_GetGValue(cell);
                default:
                    break;
            }
            return float.PositiveInfinity;
        }
        public float PF_GetGValue(Vector2Int cell)
        {
            return CellDistance(cell, pf_startPoint);
        }
        public float PF_GetHValue(Vector2Int cell)
        {
            return CellDistance(cell, pf_endPoint);
        }
    }
}
