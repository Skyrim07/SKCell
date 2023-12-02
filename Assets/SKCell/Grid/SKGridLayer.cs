using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
    /// <summary>
    /// Holds a layer of SKGrid to a game object.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("SKCell/Grid/SKGridLayer")]
    public class SKGridLayer : MonoBehaviour
    {
        #region Fields
        [HideInInspector] public SKGridAsset saveAsset;

        private Color clearGrey = new Color(1, 1, 1, 0.4f);
        private Color editColor = new Color(1f, 0.9f, 0.2f, 0.4f);

        public string uid = string.Empty;

        [Header("Editor")]
        [HideInInspector] public bool preview = false;
        public bool drawText = false;

        [Header("Grid")]
        [Tooltip("The X  count of the grid.")]
        public int width = 5;
        [Tooltip("The Y count of the grid.")]
        public int height = 5;

        [Tooltip("Side length of each cell.")]
        public float cellSize = 1;

        [Tooltip("Default value is the position of this game object.")]
        [HideInInspector] public Transform bottomLeftOverride;

        [HideInInspector] public SKGrid grid;

        [Header("Display")]
        [Tooltip("Resolution of this grid.")]
        [Range(1, 20)]
        public int resolution = 10;

        [Tooltip("Background of this grid.")]
        public Color defaultColor = Color.clear;
        public FilterMode filterMode = FilterMode.Point;

        [Header("Pathfinding")]
        public PathfindingAlgorithm algorithm = PathfindingAlgorithm.AStar;
        public PathfindingDirection directionAllowed = PathfindingDirection.FourDirections;

        [HideInInspector] public bool active = true, isEditing = false;
        [HideInInspector] public bool initialized = false;
        [HideInInspector] public Canvas rtCanvas;
        [HideInInspector] public GameObject rtGO;
        [HideInInspector] public Texture2D rt;

        private BoxCollider cld;

        private List<Vector2Int> dir_4 = new List<Vector2Int>() { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        private List<Vector2Int> dir_8s = new List<Vector2Int>() { new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(1, 1), new Vector2Int(1, -1) };
        private List<Vector2Int> dir_8r = new List<Vector2Int>() { new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(-1, 1) };
        private List<Vector2Int> dir_4r = new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.down, Vector2Int.up };
        #endregion

        #region Lifecycle
        private void Awake()
        {
            cld = GetComponent<BoxCollider>();
            if (Application.isPlaying)
            {
                if (grid == null)
                {
                    bottomLeftOverride = bottomLeftOverride == null ? transform : bottomLeftOverride;
                    width = Mathf.Max(1, width);
                    height = Mathf.Max(1, height);
                    cellSize = Mathf.Max(0.001f, cellSize);
                    grid = new SKGrid(width, height, cellSize, bottomLeftOverride.position);
                }
            }
        }
        private void Start()
        {
            grid.pf_Initialized = false;
        }
        private void OnEnable()
        {
#if UNITY_EDITOR
            LoadGridFromAssets(saveAsset);
            if (grid == null)
            {
                GenerateStructure();
            }
#endif
        }
        private void Update()
        {
#if UNITY_EDITOR
            // if (!Application.isPlaying)
            DrawEditorPreview();
#endif
        }
        #endregion

        #region Cell Value
        public float GetCellValue(int x, int y)
        {
            return grid.GetCellValue(x, y);
        }

        public void SetCellValue(int x, int y, float value)
        {
            grid.SetCellValue(x, y, value);
        }

        #endregion

        #region Cell Selection
        public Vector2Int CellFromWorldPos(Vector3 pos)
        {
            return grid.PositionToCell(pos);
        }
        public Vector3 WorldPosFromCell(Vector2Int cell)
        {
            return grid.CellToPosition(cell);
        }
        public Vector3 WorldPosFromCell(int x, int y)
        {
            return grid.CellToPosition(new Vector2Int(x, y));
        }
        public List<Vector2Int> CellsFromWorldPos(Vector3 pos1, Vector3 pos2)
        {
            return grid.GetCells(pos1, pos2);
        }
        public List<Vector2Int> CellsFromCoordinate(Vector2Int cell1, Vector2Int cell2)
        {
            return grid.GetCells(cell1.x, cell2.x, cell1.y, cell2.y);
        }

        #endregion

        #region Cell Color

        /// <summary>
        /// Set the color of cells defined by radius with c1 as central color and c2 as margin color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="radius"></param>
        public void SetCellColorRange_Star(CellOperator op, int x, int y, Color c1, Color c2, int radius)
        {
            if (x < 0 || y < 0 || x > grid.width || y > grid.height)
                return;
            radius = Mathf.Max(radius, 1);
            if (radius == 1)
            {
                SetCellColor(x, y, c1);
                return;
            }

            for (int i = x - radius + 1; i <= x + radius - 1; i++)
            {
                for (int j = y - radius + 1; j <= y + radius - 1; j++)
                {
                    int dist = grid.CellDistance(new Vector2Int(x, y), new Vector2Int(i, j));
                    if (dist > radius)
                        continue;
                    switch (op)
                    {
                        case CellOperator.Set:
                            SetCellColor(i, j, SKUtils.MixColor(c1, c2, (float)dist / radius), false);
                            break;
                        case CellOperator.Add:
                            AddCellColor(i, j, SKUtils.MixColor(c1, c2, (float)dist / radius), false);
                            break;
                        case CellOperator.Multiply:
                            MultiplyCellColor(i, j, SKUtils.MixColor(c1, c2, (float)dist / radius), false);
                            break;
                        default:
                            break;
                    }
                }
            }
            rt.Apply();
        }
        public void SetCellColorRange_Circle(CellOperator op, int x, int y, Color c1, Color c2, int radius)
        {
            if (x < 0 || y < 0 || x > grid.width || y > grid.height)
                return;
            radius = Mathf.Max(radius, 1);
            if (radius == 1)
            {
                SetCellColor(x, y, c1);
                return;
            }
            for (int i = x - radius + 1; i <= x + radius - 1; i++)
            {
                for (int j = y - radius + 1; j <= y + radius - 1; j++)
                {
                    float dist = grid.CellDistanceCir(new Vector2Int(x, y), new Vector2Int(i, j));
                    if (dist > radius)
                        continue;
                    switch (op)
                    {
                        case CellOperator.Set:
                            SetCellColor(i, j, SKUtils.MixColor(c1, c2, (float)dist / radius), false);
                            break;
                        case CellOperator.Add:
                            AddCellColor(i, j, SKUtils.MixColor(c1, c2, (float)dist / radius), false);
                            break;
                        case CellOperator.Multiply:
                            MultiplyCellColor(i, j, SKUtils.MixColor(c1, c2, (float)dist / radius), false);
                            break;
                        default:
                            break;
                    }
                }
            }
            rt.Apply();
        }
        public void SetCellColorRange_Square(CellOperator op, int x, int y, Color c1, Color c2, int radius)
        {
            if (x < 0 || y < 0 || x > grid.width || y > grid.height)
                return;
            radius = Mathf.Max(radius, 1);
            if (radius == 1)
            {
                SetCellColor(x, y, c1);
                return;
            }
            for (int i = x - radius + 1; i <= x + radius - 1; i++)
            {
                for (int j = y - radius + 1; j <= y + radius - 1; j++)
                {
                    float distX = grid.CellDistanceX(new Vector2Int(x, y), new Vector2Int(i, j));
                    float distY = grid.CellDistanceY(new Vector2Int(x, y), new Vector2Int(i, j));
                    Color color = Color.Lerp(c1, c2, (distX + distY) / (radius * 2));
                    switch (op)
                    {
                        case CellOperator.Set:
                            SetCellColor(i, j, color, false);
                            break;
                        case CellOperator.Add:
                            AddCellColor(i, j, color, false);
                            break;
                        case CellOperator.Multiply:
                            MultiplyCellColor(i, j, color, false);
                            break;
                        default:
                            break;
                    }
                }
            }
            rt.Apply();
        }
        public void SetCellColor(int x, int y, Color color, bool apply = true)
        {
            if (grid == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- grid is NULL.");
                return;
            }
            if (rt == null || rtCanvas == null || rtGO == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- Structure Incomplete. Invoke GenerateStructure to solve this problem.");
                return;
            }
            if (!CheckCellValidity(x, y))
                return;

            float iCellSize = 1;
            rt.SetQuad(new Vector2Int(Mathf.RoundToInt(x * iCellSize), Mathf.RoundToInt(y * iCellSize)) * resolution, new Vector2Int(Mathf.RoundToInt((x + 1) * iCellSize), Mathf.RoundToInt((y + 1) * iCellSize)) * resolution, color);
            if (apply)
                rt.Apply();
        }

        public void SetCellColor(List<Vector2Int> cells, Color color, bool apply = true)
        {
            foreach (var cell in cells)
            {
                SetCellColor(cell.x, cell.y, color, false);
            }
            if (apply)
                rt.Apply();
        }
        public void EraseCellColor(int x, int y, bool apply = true)
        {
            if (grid == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- grid is NULL.");
                return;
            }
            if (rt == null || rtCanvas == null || rtGO == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- Structure Incomplete. Invoke GenerateStructure to solve this problem.");
                return;
            }
            float iCellSize = 1;
            rt.SetQuad(new Vector2Int(Mathf.RoundToInt(x * iCellSize), Mathf.RoundToInt(y * iCellSize)) * resolution, new Vector2Int(Mathf.RoundToInt((x + 1) * iCellSize), Mathf.RoundToInt((y + 1) * iCellSize)) * resolution, defaultColor);
            if (apply)
                rt.Apply();
        }
        public void EraseCellColor(List<Vector2Int> cells, bool apply = true)
        {
            foreach (var cell in cells)
            {
                SetCellColor(cell.x, cell.y, defaultColor, false);
            }
            if (apply)
                rt.Apply();
        }
        public void AddCellColor(int x, int y, Color colorInc, bool apply = true)
        {
            if (grid == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- grid is NULL.");
                return;
            }
            if (rt == null || rtCanvas == null || rtGO == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- Structure Incomplete. Invoke GenerateStructure to solve this problem.");
                return;
            }
            float iCellSize = 1;
            rt.AddQuad(new Vector2Int(Mathf.RoundToInt(x * iCellSize), Mathf.RoundToInt(y * iCellSize)) * resolution, new Vector2Int(Mathf.RoundToInt((x + 1) * iCellSize), Mathf.RoundToInt((y + 1) * iCellSize)) * resolution, colorInc);
            if (apply)
                rt.Apply();
        }
        public void MultiplyCellColor(int x, int y, Color colorInc, bool apply = true)
        {
            if (grid == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- grid is NULL.");
                return;
            }
            if (rt == null || rtCanvas == null || rtGO == null)
            {
                SKUtils.EditorLogError("SKGridLayer.SetCellColor -- Structure Incomplete. Invoke GenerateStructure to solve this problem.");
                return;
            }
            float iCellSize = 1;
            rt.MultiplyQuad(new Vector2Int(Mathf.RoundToInt(x * iCellSize), Mathf.RoundToInt(y * iCellSize)) * resolution, new Vector2Int(Mathf.RoundToInt((x + 1) * iCellSize), Mathf.RoundToInt((y + 1) * iCellSize)) * resolution, colorInc);
            if (apply)
                rt.Apply();
        }
        public Color GetCellColor(int x, int y)
        {
            return rt.GetPixel(x * resolution + 1, y * resolution + 1);
        }
        #endregion

        #region Pathfinding

        /// <summary>
        /// Start a pathfinding session. Returns a path given by a list of grid cells. *Call PathfindingSetStartPoint, PathfindingSetDestination, etc. before calling this method.
        /// </summary>
        /// <returns></returns>
        public List<Vector2Int> PathfindingStart()
        {
            if (!PathfindingCheckInitialize())
                return null;

            List<Vector2Int> openSet = new List<Vector2Int>();
            List<Vector2Int> closeSet = new List<Vector2Int>();
            List<Vector2Int> resSet = new List<Vector2Int>();
            openSet.Add(grid.pf_startPoint);
            grid.pf_FValue[grid.pf_startPoint.x, grid.pf_startPoint.y] = 0;
            while (openSet.Count > 0)
            {
                Vector2Int n = new Vector2Int(-1, -1);
                float fMin = float.PositiveInfinity;
                for (int i = 0; i < openSet.Count; i++)
                {
                    float fVal = grid.pf_FValue[openSet[i].x, openSet[i].y];
                    if (fVal <= fMin)
                    {
                        fMin = fVal;
                        n = openSet[i];
                    }
                }

                if (n == grid.pf_endPoint)
                {
                    Vector2Int curCell = n;
                    resSet.Add(new Vector2Int(n.x, n.y));
                    SetCellColor(curCell.x, curCell.y, Color.white, false);
                    while (curCell != grid.pf_startPoint)
                    {
                        int parent = grid.pf_ParentCell[curCell.x, curCell.y];
                        curCell += parent < 4 ? dir_4[parent] : dir_8s[parent - 4];
                        resSet.Add(curCell);
                        SetCellColor(curCell.x, curCell.y, Color.white, false);
                    }
                    rt.Apply();
                    resSet.Reverse();
                    grid.EndPathfindingSession();
                    return resSet;
                }
                else
                {
                    openSet.Remove(n);
                    closeSet.Add(n);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2Int near = n + dir_4[i];
                        if (closeSet.Contains(near))
                            continue;
                        if (!CheckCellValidity(near.x, near.y) || grid.pf_CellCost[near.x, near.y] == 1)
                        {
                            closeSet.Add(near);
                            continue;
                        }
                        if (!openSet.Contains(near))
                        {
                            grid.pf_ParentCell[near.x, near.y] = dir_4r.IndexOf(dir_4[i]);
                            grid.pf_FValue[near.x, near.y] = grid.PF_GetFValue(near, n, algorithm);
                            openSet.Add(near);
                        }
                        else
                        {
                            float f = grid.PF_GetFValue(near, n, algorithm);
                            if (f < grid.pf_FValue[near.x, near.y])
                            {
                                grid.pf_ParentCell[near.x, near.y] = dir_4r.IndexOf(dir_4[i]);
                                grid.pf_FValue[near.x, near.y] = f;
                            }
                        }
                    }
                    if (directionAllowed == PathfindingDirection.EightDirections)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2Int near = n + dir_8s[i];
                            if (closeSet.Contains(near))
                                continue;
                            if (!CheckCellValidity(near.x, near.y) || grid.pf_CellCost[near.x, near.y] == 1)
                            {
                                closeSet.Add(near);
                                continue;
                            }
                            else if (!openSet.Contains(near))
                            {
                                grid.pf_ParentCell[near.x, near.y] = dir_8r.IndexOf(dir_8s[i]) + 4;
                                grid.pf_FValue[near.x, near.y] = grid.PF_GetFValue(near, n, algorithm, true);
                                openSet.Add(near);
                            }
                            else
                            {
                                float f = grid.PF_GetFValue(near, n, algorithm, true);
                                if (f < grid.pf_FValue[near.x, near.y])
                                {
                                    grid.pf_ParentCell[near.x, near.y] = dir_4r.IndexOf(dir_8s[i]) + 4;
                                    grid.pf_FValue[near.x, near.y] = f;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Start a pathfinding session with start and end points overrided. Returns a path given by a list of grid cells.
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        /// <returns></returns>
        public List<Vector2Int> PathfindingStart(Vector2Int startCell, Vector2Int endCell)
        {
            PathfindingSetStartPoint(startCell.x, startCell.y);
            PathfindingSetEndPoint(endCell.x, endCell.y);
            return PathfindingStart();
        }

        /// <summary>
        /// Set the start point of a pathfinding session.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PathfindingSetStartPoint(int x, int y)
        {
            if (!PathfindingCheckInitialize() || !CheckCellValidity(x, y))
                return;
            grid.pf_startPoint = new Vector2Int(x, y);
        }
        /// <summary>
        /// Set the end point of a pathfinding session.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PathfindingSetEndPoint(int x, int y)
        {
            if (!PathfindingCheckInitialize() || !CheckCellValidity(x, y))
                return;

            grid.pf_endPoint = new Vector2Int(x, y);
        }
        /// <summary>
        /// Set the cell cost(walkability) of a given cell (x,y). 0: Free to walk; 1: Unwalkable.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cost01"></param>
        public void PathfindingSetCellCost(int x, int y, int cost01)
        {
            if (!PathfindingCheckInitialize() || !CheckCellValidity(x, y))
                return;
            if (grid != null && grid.pf_CellCost != null)
            {
                cost01 = (int)Mathf.Clamp01(cost01);
                grid.pf_CellCost[x, y] = cost01;
            }
        }
        public void PathfindingSetCellCost(List<Vector2Int> cells, int cost01)
        {
            foreach (var item in cells)
            {
                PathfindingSetCellCost(item.x, item.y, cost01);
            }
        }
        private bool PathfindingCheckInitialize()
        {
            if (grid == null)
            {
                SKUtils.EditorLogError("SKGridLayer.Pathfinding Module -- grid is NULL.");
                return false;
            }
            if (!grid.pf_Initialized)
            {
                grid.InitializePathfinding();
            }
            return true;
        }

        #endregion

        #region Occupancy

        public void SetCellOccupancy(int x, int y, bool occupied)
        {
            if (!CheckCellValidity(x, y))
                return;

            grid.occupied[x, y] = occupied;
        }
        public void SetCellOccupancy(List<Vector2Int> cells, bool occupied)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                SetCellOccupancy(cells[i].x, cells[i].y, occupied);
            }
        }

        #endregion

        #region Structure

        public void ApplyChanges()
        {
            grid.width = width;
            grid.height = height;
            grid.cellSize = cellSize;
            grid.cellValues = SKUtils.Modify2DArray(grid.cellValues, width, height);
            grid.pf_CellCost = SKUtils.Modify2DArray(grid.pf_CellCost, width, height);
            grid.occupied = SKUtils.Modify2DArray(grid.occupied, width, height);
#if UNITY_EDITOR
            GenerateStructure(false);
            SaveGridToAssets(saveAsset);
#endif
        }

#if UNITY_EDITOR
        public void AlignDisplay()
        {
            if (rtCanvas == null)
                return;
            if (rtGO == null)
                return;

            rtCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize * width, cellSize * height);
            rtCanvas.transform.localPosition = new Vector3(-cellSize / 2, -cellSize / 2, 0);

            rtGO.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize * width, cellSize * height);
        }

        public void GenerateStructure(bool createNewGrid = true)
        {
            if (gameObject == null)
                return;

            initialized = true;

            if (createNewGrid)
            {
                bottomLeftOverride = bottomLeftOverride == null ? transform : bottomLeftOverride;
                width = Mathf.Max(1, width);
                height = Mathf.Max(1, height);
                cellSize = Mathf.Max(0.001f, cellSize);
                grid = new SKGrid(width, height, cellSize, bottomLeftOverride.position);
            }

            string pathSuffix = "/SKGridLayer.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKButton Resource Error: Button prefab lost.");
                return;
            }
            if (rtCanvas != null)
            {
                if (gameObject)
                    transform.ClearChildren();
            }
            GameObject canvas = Instantiate(prefab);
            canvas.transform.SetParent(transform);
            rtCanvas = canvas.GetComponent<Canvas>();
            rtGO = canvas.transform.GetComponentInChildren<RawImage>().gameObject;

            rt = new Texture2D(width * resolution, height * resolution);
            rt.filterMode = filterMode;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.SetColor(defaultColor);
            rt.Apply();
            rtGO.GetComponent<RawImage>().texture = rt;

            cld = GetComponent<BoxCollider>();
            cld.center = grid.LocalCenterPos - new Vector3(grid.cellSize / 2f, grid.cellSize / 2f);
            cld.size = new Vector3(grid.width * grid.cellSize, grid.height * grid.cellSize, 0);

            AlignDisplay();
        }
        public void DrawEditorPreview()
        {
            if (!preview)
            {
                return;
            }
            for (int i = 0; i < grid.width; i++)
            {
                for (int j = 0; j < grid.height; j++)
                {
                    Vector3 pos = grid.GetCellBottomLeftPos(i, j);
                    Debug.DrawLine(pos, pos + new Vector3(grid.cellSize, 0, 0), isEditing ? editColor : clearGrey);
                    Debug.DrawLine(pos, pos + new Vector3(0, grid.cellSize, 0), isEditing ? editColor : clearGrey);
                    if (j == grid.height - 1)
                        Debug.DrawLine(pos + new Vector3(0, grid.cellSize, 0), pos + new Vector3(grid.cellSize, grid.cellSize, 0), isEditing ? editColor : clearGrey);
                    if (i == grid.width - 1)
                        Debug.DrawLine(pos + new Vector3(grid.cellSize, 0, 0), pos + new Vector3(grid.cellSize, grid.cellSize, 0), isEditing ? editColor : clearGrey);
                    if (drawText)
                        SKUtils.DebugDrawText($"{i},{j}", pos, new Color(1, 1, 1, 0.7f), grid.cellSize * 0.3f, 0);

                    if (Application.isPlaying && drawText)
                    {
                        SKUtils.DebugDrawText(grid.GetCellValue(i, j).ToString("f1"), grid.GetCellCenterPos(i, j), new Color(1, 0.7f, 0.7f, 0.7f), grid.cellSize * 0.2f, 0);
                    }
                }
            }
        }

#endif
        #endregion

        #region Data Control
        public void SaveGridToAssets(SKGridAsset asset)
        {
            if (asset == null)
                return;

            asset.grid = grid;
            asset.filterMode = filterMode;
            asset.defaultColor = defaultColor;
            asset.resolution = resolution;


            asset.cellValues_se = SKUtils.Serialize2DArray(grid.cellValues);
            asset.cellCosts_se = SKUtils.Serialize2DArray(grid.pf_CellCost);
            asset.cellOccupied_se = SKUtils.Serialize2DArray(grid.occupied);
#if UNITY_EDITOR
            EditorUtility.SetDirty(asset);
#endif
        }


        public void LoadGridFromAssets(SKGridAsset asset, bool generateStructure = true)
        {
            if (asset == null)
                return;

            grid = asset.grid;

            width = grid.width;
            height = grid.height;
            cellSize = grid.cellSize;
            bottomLeftOverride = null;
            filterMode = asset.filterMode;
            defaultColor = asset.defaultColor;
            resolution = asset.resolution;


            grid.cellValues = SKUtils.Deserialize2DArray(asset.cellValues_se, grid.width, grid.height);
            grid.pf_CellCost = SKUtils.Deserialize2DArray(asset.cellCosts_se, grid.width, grid.height);
            grid.occupied = SKUtils.Deserialize2DArray(asset.cellOccupied_se, grid.width, grid.height);

#if UNITY_EDITOR
            if (!Application.isPlaying && generateStructure)
                SKUtils.InvokeActionEditor(0.1f, () =>
                {
                    if (!Application.isPlaying)
                        GenerateStructure(false);
                });
#endif
        }

        #endregion

        #region Misc
        private bool CheckCellValidity(int x, int y)
        {
            if (x < 0 || y < 0 || x >= grid.width || y >= grid.height)
                return false;
            return true;
        }
        public void SetActive(bool active)
        {
            this.active = active;
        }
        #endregion

    }


    public enum CellOperator
    {
        Set,
        Add,
        Multiply
    }
    public enum PathfindingAlgorithm
    {
        AStar,
        BStar,
        BreadthFirst
    }
    public enum PathfindingDirection
    {
        FourDirections,
        EightDirections
    }
}
