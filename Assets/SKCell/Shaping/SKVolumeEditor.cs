using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Water volume editor. Can get water volume data from here.
    /// </summary>
    [ExecuteInEditMode]
    public class SKVolumeEditor: MonoBehaviour
    {
        private const string OPER_MOVE = "WaterVolumeEditor: Move";
        private const string OPER_ADD = "WaterVolumeEditor: Add";
        private const string OPER_REMOVE = "WaterVolumeEditor: Remove";

        private const float HANDLE_SELECT_DIST_THRESHOLD = 1;
        private const float SELF_LABEL_OFFSET = .5f;

        public bool displaySceneLabels = true;
        public SKVolumeDataEditor data;
        private float m_baseArea;
        private int m_selectedIndex = -1;
        private bool m_isDragging = false;

        private bool m_isInSelection;
        private bool m_selectionChanged;
        private bool m_isInEditMode;
        private bool m_isShapeValid;
        private Mesh m_baseMesh, m_topMesh;
        public Matrix4x4 trans => Matrix4x4.Translate(transform.position) * Matrix4x4.Rotate(transform.rotation);

#if UNITY_EDITOR
        [SKInspectorButton("Generate Structure", 1f, .5f, 0f)]
        public void GenerateStructure()
        {
            string pathSuffix = "/SKVolumeEditor.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKConsole Resource Error: prefab lost.");
                return;
            }
            GameObject console = Instantiate(prefab);
            console.name = $"SKVolumeEditor";
            console.transform.SetParent(transform.parent);
            console.transform.CopyFrom(transform);
            console.transform.SetSiblingIndex(transform.GetSiblingIndex());
            Selection.activeGameObject = console;
            DestroyImmediate(this.gameObject);
        }
        private GUIStyle m_labelStyle, m_selfLabelStyle, m_errorStyle;
        public void OnEnable()
        {
            if (data == null)
            {
                data = new SKVolumeDataEditor();
                transform.position = new Vector3(transform.position.x, _GetMousePosition().y, transform.position.z);
                _CalibrateToCenter();
            }

            SceneView.duringSceneGui += _OnSceneGUI;
            Selection.selectionChanged += _OnSelectionChanged;
            Undo.undoRedoPerformed += _OnUndoRedoPerformed;

            _OnSelectionChanged();
            _DrawPolygon();
            _InitStyles();
        }

        public void Start()
        {
            _InitStyles();
        }

        public void OnDisable()
        {
            SceneView.duringSceneGui -= _OnSceneGUI;
            Selection.selectionChanged -= _OnSelectionChanged;
        }

        private void _OnSelectionChanged()
        {
            m_isInSelection = Selection.Contains(gameObject.GetInstanceID());
            if (!m_isInSelection)
            {
                _ExitEditMode();
            }
            else
            {
                m_selectionChanged = true;
            }
        }

        private void _OnUndoRedoPerformed()
        {
            _OnEdit();
        }

        [SKInspectorButton("Enter Edit Mode")]
        private void _EnterEditMode()
        {
            m_isInEditMode = true;
        }

        [SKInspectorButton("Exit Edit Mode")]
        private void _ExitEditMode()
        {
            m_isInEditMode = false;
        }
        [SKInspectorButton("Calibrate Center")]
        private void _CalibrateToCenter()
        {
            Vector3 centerPos = _GetPosAverage();
            Vector3 diff = transform.position - centerPos;
            Vector3 rotatedDiff = Quaternion.Inverse(transform.rotation) * diff;

            Vector3 scaledDiff = new Vector3(rotatedDiff.x / transform.lossyScale.x, rotatedDiff.y / transform.lossyScale.y, rotatedDiff.z / transform.lossyScale.z);

            for (int i = 0; i < data.points.Count; i++)
            {
                data.points[i] += new Vector2(scaledDiff.x, scaledDiff.z);
            }

            transform.Translate(-diff, Space.World);
            _OnEdit();
        }


        private void _OnSceneGUI(SceneView sceneView)
        {
            if (!m_isInSelection)
            {
                return;
            }

            if (m_selectionChanged) // Accomodate for duplicating
            {
                _ReconstructMesh();
                _DrawPolygon();
                _DrawLabels();
                m_selectionChanged = false;
            }
            if (m_isInEditMode)
            {
                Event e = Event.current;

                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
                {
                    Vector3 mousePosition = _GetMousePosition();
                    if (mousePosition != Vector3.zero)
                    {
                        Undo.RecordObject(this, OPER_ADD);
                        transform.localScale = Vector3.one;
                        Vector3 localPosition = transform.InverseTransformPoint(mousePosition);
                        Vector2 newPoint = new Vector2(localPosition.x, localPosition.z);
                        int insertIndex = _GetInsertIndex(newPoint);
                        data.points.Insert(insertIndex, newPoint);
                        e.Use();
                        _OnEdit();
                    }
                }

                if (e.type == EventType.MouseDown && e.button == 0 && e.control)
                {
                    if (m_selectedIndex >= 0)
                    {
                        Undo.RecordObject(this, OPER_REMOVE);
                        data.points.RemoveAt(m_selectedIndex);
                        m_selectedIndex = -1;
                        e.Use();
                        _OnEdit();
                    }
                }

                if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && m_selectedIndex >= 0)
                {
                    m_isDragging = true;
                    e.Use();
                }

                if (e.type == EventType.MouseDrag && e.button == 0 && m_isDragging)
                {
                    Vector3 mousePosition = _GetMousePosition();
                    if (mousePosition != Vector3.zero)
                    {
                        Undo.RecordObject(this, OPER_MOVE);
                        transform.localScale = Vector3.one;
                        Vector3 localPosition = transform.InverseTransformPoint(mousePosition);
                        data.points[m_selectedIndex] = new Vector2(localPosition.x, localPosition.z);
                        e.Use();
                        _OnEdit();
                    }
                }

                if (e.type == EventType.MouseUp && e.button == 0 && m_isDragging)
                {
                    m_isDragging = false;
                    e.Use();
                }

                if (e.type == EventType.MouseMove)
                {
                    _UpdateNearestPointIndex();
                    e.Use();
                }

                _DrawPolygon();
            }

            _DrawLabels();
        }

        private void _OnEdit()
        {
            _ReconstructMesh();
            EditorUtility.SetDirty(gameObject);
        }

        private void _DrawPolygon()
        {
            List<(Vector3, Vector3)> lines = new List<(Vector3, Vector3)>();
            transform.localScale = Vector3.one;
            for (int i = 0; i < data.points.Count; i++)
            {
                Vector3 point = transform.TransformPoint(new Vector3(data.points[i].x, 0, data.points[i].y));
                Vector3 nextPoint = transform.TransformPoint(new Vector3(data.points[(i + 1) % data.points.Count].x, 0, data.points[(i + 1) % data.points.Count].y));
                lines.Add((point, nextPoint));
            }

            m_isShapeValid = true;
            HashSet<int> intersectingLines = new HashSet<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (_LinesIntersect(lines[i], lines[j]) && !_SharesStartOrEnd(lines[i], lines[j]))
                    {
                        intersectingLines.Add(i);
                        intersectingLines.Add(j);
                        m_isShapeValid = false;
                    }
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                Handles.color = intersectingLines.Contains(i) ? Color.red : Color.cyan;
                Handles.DrawLine(lines[i].Item1, lines[i].Item2);
            }

            Handles.color = Color.white;
            for (int i = 0; i < data.points.Count; i++)
            {
                Vector3 point = transform.TransformPoint(new Vector3(data.points[i].x, 0, data.points[i].y));
                float handleSize = HandleUtility.GetHandleSize(point) * 0.04f;
                if (Handles.Button(point, Quaternion.identity, handleSize, handleSize, Handles.DotHandleCap))
                {
                    m_selectedIndex = i;
                }
            }

            m_baseArea = _CalculatePolygonArea();
        }

        private float _CalculatePolygonArea()
        {
            float area = 0;
            int j = data.points.Count - 1;

            for (int i = 0; i < data.points.Count; i++)
            {
                area += (data.points[j].x + data.points[i].x) * (data.points[j].y - data.points[i].y);
                j = i;
            }

            return Mathf.Abs(area / 2);
        }

        private bool _LinesIntersect((Vector3, Vector3) line1, (Vector3, Vector3) line2)
        {
            return _LinesIntersect(new Vector2(line1.Item1.x, line1.Item1.z), new Vector2(line1.Item2.x, line1.Item2.z),
                                   new Vector2(line2.Item1.x, line2.Item1.z), new Vector2(line2.Item2.x, line2.Item2.z));
        }

        private bool _LinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x);
            if (d == 0) return false;

            float u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d;
            float v = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d;

            return (u >= 0 && u <= 1 && v >= 0 && v <= 1);
        }

        private bool _SharesStartOrEnd((Vector3, Vector3) line1, (Vector3, Vector3) line2)
        {
            Vector3 a1 = line1.Item1, a2 = line1.Item2;
            Vector3 b1 = line2.Item1, b2 = line2.Item2;

            return (a1 == b1 || a1 == b2 || a2 == b1 || a2 == b2);
        }

        private Vector3 _GetMousePosition()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }
            return Vector3.zero;
        }

        private void _UpdateNearestPointIndex()
        {
            float minDistance = float.MaxValue;
            m_selectedIndex = -1;
            transform.localScale = Vector3.one;
            for (int i = 0; i < data.points.Count; i++)
            {
                Vector3 point = transform.TransformPoint(new Vector3(data.points[i].x, 0, data.points[i].y));
                float distance = HandleUtility.DistanceToCircle(point, 1.0f);
                if (distance < HANDLE_SELECT_DIST_THRESHOLD && distance < minDistance)
                {
                    minDistance = distance;
                    m_selectedIndex = i;
                }
            }
        }

        private int _GetInsertIndex(Vector2 newPoint)
        {
            int nearestIndex = -1;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < data.points.Count; i++)
            {
                Vector2 pointA = data.points[i];
                Vector2 pointB = data.points[(i + 1) % data.points.Count];

                Vector2 closestPointOnSegment = _GetClosestPointOnSegment(pointA, pointB, newPoint);
                float distance = Vector2.Distance(closestPointOnSegment, newPoint);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex + 1;
        }

        private Vector2 _GetClosestPointOnSegment(Vector2 pointA, Vector2 pointB, Vector2 point)
        {
            Vector2 AB = pointB - pointA;
            Vector2 AP = point - pointA;
            float magnitudeAB = AB.sqrMagnitude;
            float ABAPproduct = Vector2.Dot(AP, AB);
            float distance = ABAPproduct / magnitudeAB;

            if (distance < 0)
            {
                return pointA;
            }
            else if (distance > 1)
            {
                return pointB;
            }
            else
            {
                return pointA + AB * distance;
            }
        }

        private Vector3 _GetPosAverage()
        {
            transform.localScale = Vector3.one;
            Vector2 total = Vector2.zero;
            for (int i = 0; i < data.points.Count; i++)
            {
                total += data.points[i];
            }
            Vector2 avg = total / data.points.Count;
            return transform.TransformPoint(new Vector3(avg.x, 0, avg.y));
        }

        private void _ReconstructMesh()
        {
            m_baseMesh = new Mesh();
            m_topMesh = new Mesh();
            Vector3[] vertices_base = new Vector3[data.points.Count];
            Vector3[] vertices_top = new Vector3[data.points.Count];
            for (int i = 0; i < data.points.Count; i++)
            {
                vertices_base[i] = new Vector3(data.points[i].x, 0, data.points[i].y);
                vertices_top[i] = new Vector3(data.points[i].x, data.maxHeight * data.initialHeightPercentage, data.points[i].y);
            }
            int[] triangles_base = new int[(data.points.Count - 2) * 3];
            for (int i = 0; i < data.points.Count - 2; i++)
            {
                triangles_base[i * 3] = 0;
                triangles_base[i * 3 + 1] = i + 1;
                triangles_base[i * 3 + 2] = i + 2;
            }
            m_baseMesh.vertices = vertices_base;
            m_baseMesh.triangles = triangles_base;
            m_baseMesh.RecalculateNormals();

            m_topMesh.vertices = vertices_top;
            m_topMesh.triangles = triangles_base;
            m_topMesh.RecalculateNormals();
        }

        private void _DrawLabels()
        {
            if (!displaySceneLabels || m_labelStyle == null)
            {
                return;
            }

            Vector3 posAvg = _GetPosAverage();
            if (!m_isShapeValid)
            {
                Handles.Label(Vector3.down * SELF_LABEL_OFFSET * 4f + posAvg, $"Invalid shape!", m_errorStyle);
            }
            else
            {
                float currentVolume = data.volume * data.initialHeightPercentage;
                string currentVolumeStr = data.initialHeightPercentage == 0 ?
                    $"Current vol: {(currentVolume).ToString("f2")} (Empty!)" :
                    data.initialHeightPercentage == 1 ?
                    $"Current vol: {(currentVolume).ToString("f2")} (Full!)" :
                    $"Current vol: {(currentVolume).ToString("f2")}";
                string totalStr = $"<b><color=#ff8d14>Volume: {(data.volume).ToString("f2")}</color> (Reference: {(m_baseArea * data.maxHeight).ToString("f2")})</b>\n{currentVolumeStr}\nSurface area: {m_baseArea.ToString("f2")}";
                Handles.Label(Vector3.up * SELF_LABEL_OFFSET * 10f + posAvg, totalStr, m_labelStyle);
            }
        }

        private void _InitStyles()
        {
            m_labelStyle = new GUIStyle();
            m_labelStyle.normal.textColor = Color.white;
            m_labelStyle.alignment = TextAnchor.MiddleCenter;

            Texture2D bgTexture = new Texture2D(1, 1);
            bgTexture.SetPixel(0, 0, new Color(0, 0, 0, .6f));
            bgTexture.Apply();

            m_labelStyle.normal.background = bgTexture;
            m_labelStyle.padding = new RectOffset(2, 2, 2, 2);
            //
            m_selfLabelStyle = new GUIStyle();
            m_selfLabelStyle.normal.textColor = new Color(1, .5f, 0);
            m_selfLabelStyle.alignment = TextAnchor.MiddleCenter;

            Texture2D bgTexture2 = new Texture2D(1, 1);
            bgTexture2.SetPixel(0, 0, new Color(0, 0, 0, .6f));
            bgTexture2.Apply();

            m_selfLabelStyle.normal.background = bgTexture2;
            m_selfLabelStyle.padding = new RectOffset(2, 2, 2, 2);
            //
            m_errorStyle = new GUIStyle();
            m_errorStyle.normal.textColor = new Color(1, 1, 1);
            m_errorStyle.fontStyle = FontStyle.Bold;
            m_errorStyle.alignment = TextAnchor.MiddleCenter;

            Texture2D bgTexture3 = new Texture2D(1, 1);
            bgTexture3.SetPixel(0, 0, new Color(1, .1f, .1f, .8f));
            bgTexture3.Apply();

            m_errorStyle.normal.background = bgTexture3;
            m_errorStyle.padding = new RectOffset(4, 4, 4, 4);
        }
#endif

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }
            if (data != null && data.points.Count > 0)
            {
                if (m_isShapeValid)
                {
                    Color color;
                    if (m_baseMesh != null)
                    {
                        color = Color.cyan;
                        color.a = 0.5f;
                        Gizmos.color = color;
                        Gizmos.DrawMesh(m_baseMesh, transform.position, transform.rotation, transform.lossyScale);
                        color = new Color(1f, 1f, 1f, .5f);
                        Gizmos.color = color;
                        Gizmos.DrawMesh(m_topMesh, transform.position, transform.rotation, transform.lossyScale);
                    }
                    color = Color.cyan;
                    color.a = 0.5f;
                    SKGizmoDrawUtil.DrawPolyLineUnityGizmos(trans, data.points, 0,
                                                               data.maxHeight * data.initialHeightPercentage,
                                                               0.1f, color, true);
                    color = Color.white;
                    color.a = 0.5f;
                    SKGizmoDrawUtil.DrawPolyLineUnityGizmos(trans, data.points, 0 + data.maxHeight * data.initialHeightPercentage,
                                                               data.maxHeight - data.maxHeight * data.initialHeightPercentage,
                                                               0.1f, color, true);
                }
            }
        }
#endif

        [Serializable]
        public class SKVolumeDataEditor
        {
            public List<Vector2> points = new List<Vector2>()
            {
                new Vector2(-5, -5),
                new Vector2(0, 5 * 0.866f), // make equilateral triangle
                new Vector2(5, -5)
            };
            public int volume = 10;
            public int volumeCurrent => Mathf.FloorToInt(volume * initialHeightPercentage);
            public float maxHeight = 5f;
            [Range(0f, 1f)]
            public float initialHeightPercentage = 0.5f;
        }

        #region Public API
        /// <summary>
        /// Retrieve the water volume data. Positions are in world space.
        /// </summary>
        /// <returns></returns>
        public SKVolumeData GetWaterVolumeData()
        {
            if (data == null)
            {
                SKUtils.EditorLogError("Water volume data is null!");
                return null;
            }

            SKVolumeData copy = new SKVolumeData();
            copy.maxHeight = data.maxHeight;
            copy.initialHeightPercentage = data.initialHeightPercentage;
            copy.maxVolume = data.volume;
            transform.localScale = Vector3.one;
            for (int i = 0; i < data.points.Count; i++)
            {
                copy.points.Add(transform.TransformPoint(new Vector3(data.points[i].x, 0, data.points[i].y)));
            }
            return copy;
        }

        #endregion
    }

    [Serializable]
    public class SKVolumeData
    {
        public List<Vector3> points = new List<Vector3>();
        public float maxHeight = 5f;
        [Range(0f, 1f)]
        public float initialHeightPercentage = 0.5f;
        public float maxVolume;
    }
}
