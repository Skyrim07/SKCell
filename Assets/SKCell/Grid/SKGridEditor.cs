using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

namespace SKCell
{
    [ExecuteInEditMode]
    public class SKGridEditor : EditorWindow
    {
        private static SKGridLayer layer;

        private static SKGridEditorView view = SKGridEditorView.None;

        private static Vector2Int activeCell;
        private static Vector2Int selectedCell;
        private static Color selectColor = new Color(0.9f, 0.8f, 0.7f, 0.3f);
        private static Color selectTextColor = new Color(1f, 0.9f, 0.75f);

        private static Color highColor = new Color(1, 0.2f, 0.2f, 0.7f);
        private static Color lowColor = new Color(0.2f, 1f, 0.2f, 0.7f);

        private static Color fullColor = new Color(1, 1, 1, 0.7f);
        private static Color emptyColor = new Color(0, 0, 0, 0.7f);

        private static string sceneViewStr = "Scene";
        private static string hierViewStr = "UnityEditor.SceneHierarchyWindow,UnityEditor.dll";
        private static bool isMouseInSceneView, oIsMouseInSceneView;

        private static GUIContent viewButtonCnt, drawButtonCnt;

        public static void Initialize(SKGridLayer gridLayer)
        {
            selectedCell = new Vector2Int(-1, -1);
            layer = gridLayer;
            layer.isEditing = true;

            selectTextColor = new Color(1f, 0.9f, 0.75f);
            activeCell = new Vector2Int(-1, -1);

            viewButtonCnt = new GUIContent(SKAssetLibrary.LoadTexture("Eye"));
            drawButtonCnt = new GUIContent(SKAssetLibrary.LoadTexture("Pen"));

            SceneView.duringSceneGui += OnSceneView;
            view = SKGridEditorView.None;

            GetWindow<SKGridEditor>("Grid Editor", new Type[] { Type.GetType(hierViewStr)
        });
        }
        private void Update()
        {
            isMouseInSceneView = mouseOverWindow != null && mouseOverWindow.titleContent != null && mouseOverWindow.titleContent.text.Equals(sceneViewStr);
            if (isMouseInSceneView != oIsMouseInSceneView)
            {
                if (isMouseInSceneView)
                    OnMouseEnterScene();
                else
                    OnMouseLeaveScene();
            }
            oIsMouseInSceneView = isMouseInSceneView;
            Repaint();
        }
        private static void OnMouseEnterScene()
        {

        }
        private static void OnMouseLeaveScene()
        {
            if (activeCell != new Vector2(-1, -1))
            {
                layer.SetCellColor(activeCell.x, activeCell.y, layer.defaultColor);
            }
        }
        private void OnGUI()
        {
            if (!layer.isEditing)
            {
                Close();
            }

            DrawTopBar();

            DrawViewMenu();
            DrawSelection();

            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.LowerLeft;
        }

        private static void DrawTopBar()
        {
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontSize = 15;
            if (!layer.isEditing)
            {
                GUILayout.Label("No active grid.");
            }
            else
            {
                GUILayout.Label($"Active Grid: {layer.name}");
            }
            EditorGUI.DrawRect(new Rect(0, 25, 4000, 3), Color.gray);
            GUILayout.Space(10);

            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(viewButtonCnt, GUILayout.Width(30), GUILayout.Height(30)))
            {

            }
            if (GUILayout.Button(drawButtonCnt, GUILayout.Width(30), GUILayout.Height(30)))
            {

            }
            EditorGUILayout.EndHorizontal();
            //  GUI.skin.button.alignment = TextAnchor.UpperLeft;
        }

        private static void DrawViewMenu()
        {
            EditorGUILayout.LabelField("Grid View:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("View Cell Value"))
            {
                SetCellValueView();
                view = SKGridEditorView.CellValue;
            }
            if (GUILayout.Button("View Cell Cost"))
            {
                SetCellCostView();
                view = SKGridEditorView.CellCost;
            }
            if (GUILayout.Button("View Cell Occupancy"))
            {
                SetCellOccupancyView();
                view = SKGridEditorView.Occupancy;
            }
            EditorGUILayout.EndHorizontal();

            GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
            if (GUILayout.Button("Erase Grid View"))
            {
                EraseGridView();
                view = SKGridEditorView.None;
            }
            GUI.contentColor = Color.white;
            GUILayout.Space(30);
        }

        private static void SetCellValueView()
        {
            float maxVal = float.NegativeInfinity, minVal = float.PositiveInfinity;
            for (int i = 0; i < layer.grid.width; i++)
            {
                for (int j = 0; j < layer.grid.height; j++)
                {
                    maxVal = layer.grid.GetCellValue(i, j) >= maxVal ? layer.grid.GetCellValue(i, j) : maxVal;
                    minVal = layer.grid.GetCellValue(i, j) <= minVal ? layer.grid.GetCellValue(i, j) : minVal;
                }
            }
            float diff = maxVal - minVal;

            for (int i = 0; i < layer.grid.width; i++)
            {
                for (int j = 0; j < layer.grid.height; j++)
                {
                    if (diff == 0)
                    {
                        layer.SetCellColor(i, j, Color.gray, false);
                        continue;
                    }
                    layer.SetCellColor(i, j, Color.Lerp(lowColor, highColor, (layer.grid.GetCellValue(i, j) - minVal) / diff), false);
                }
            }
            layer.rt.Apply();
        }
        private static void SetCellCostView()
        {
            for (int i = 0; i < layer.grid.width; i++)
            {
                for (int j = 0; j < layer.grid.height; j++)
                {
                    layer.SetCellColor(i, j, layer.grid.pf_CellCost[i, j] == 0 ? fullColor : emptyColor, false);
                }
            }
            layer.rt.Apply();
        }
        private static void SetCellOccupancyView()
        {
            for (int i = 0; i < layer.grid.width; i++)
            {
                for (int j = 0; j < layer.grid.height; j++)
                {
                    layer.SetCellColor(i, j, layer.grid.occupied[i, j] ? emptyColor : fullColor, false);
                }
            }
            layer.rt.Apply();
        }
        private static void EraseGridView()
        {
            for (int i = 0; i < layer.grid.width; i++)
            {
                for (int j = 0; j < layer.grid.height; j++)
                {
                    layer.EraseCellColor(i, j, false);
                }
            }
            layer.rt.Apply();
        }
        private static void DrawSelection()
        {
            GUI.skin.label.fontSize = 12;
            GUILayout.Label("Selected Cell:");
            GUI.skin.label.fontSize = 30;
            GUI.contentColor = selectTextColor;
            GUILayout.Label($"{selectedCell.x} , {selectedCell.y}");
            GUI.contentColor = Color.white;


            if (selectedCell != new Vector2(-1, -1))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cell Value:");

                if (layer.grid != null && layer.grid.cellValues != null)
                    layer.grid.cellValues[selectedCell.x, selectedCell.y] = EditorGUILayout.FloatField(layer.grid.cellValues[selectedCell.x, selectedCell.y]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cell Cost:");
                if (layer.grid != null && layer.grid.pf_CellCost != null)
                    layer.grid.pf_CellCost[selectedCell.x, selectedCell.y] = EditorGUILayout.IntField(layer.grid.pf_CellCost[selectedCell.x, selectedCell.y]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Occupied:");
                if (layer.grid != null && layer.grid.occupied != null)
                    layer.grid.occupied[selectedCell.x, selectedCell.y] = SKUtils.IntToBool(EditorGUILayout.IntField(SKUtils.BoolToInt(layer.grid.occupied[selectedCell.x, selectedCell.y])));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Cell Value: N/A");
                EditorGUILayout.LabelField("Cell Cost: N/A");
                EditorGUILayout.LabelField("Occupied: N/A");
            }

            GUILayout.Space(15);
            EditorGUI.DrawRect(new Rect(0, 290, 4000, 1.5f), Color.gray);
            GUILayout.Space(15);
            GUI.skin.label.fontSize = 12;
            GUILayout.Label("Hovered Cell:");
            GUI.skin.label.fontSize = 20;
            GUILayout.Label($"{activeCell.x} , {activeCell.y}");


            if (activeCell != new Vector2(-1, -1))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cell Value:");

                if (layer.grid != null && layer.grid.cellValues != null)
                    layer.grid.cellValues[activeCell.x, activeCell.y] = EditorGUILayout.FloatField(layer.grid.cellValues[activeCell.x, activeCell.y]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cell Cost:");
                if (layer.grid != null && layer.grid.pf_CellCost != null)
                    layer.grid.pf_CellCost[activeCell.x, activeCell.y] = EditorGUILayout.IntField(layer.grid.pf_CellCost[activeCell.x, activeCell.y]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Occupied:");
                if (layer.grid != null && layer.grid.occupied != null)
                    layer.grid.occupied[activeCell.x, activeCell.y] = SKUtils.IntToBool(EditorGUILayout.IntField(SKUtils.BoolToInt(layer.grid.occupied[activeCell.x, activeCell.y])));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Cell Value: N/A");
                EditorGUILayout.LabelField("Cell Cost: N/A");
                EditorGUILayout.LabelField("Occupied: N/A");
            }

            EditorGUILayout.Space(30);
            GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
            if (GUILayout.Button("<Finish Edit>"))
            {
                FinishEdit(layer);
            }
            GUI.contentColor = Color.white;
        }

        private void OnDestroy()
        {
            EraseGridView();
            layer.isEditing = false;
            SKUtils.RefreshSelection(layer.gameObject);
            SceneView.duringSceneGui -= OnSceneView;

            layer.SaveGridToAssets(layer.saveAsset);
        }

        public static void FinishEdit(SKGridLayer gridLayer)
        {
            layer = gridLayer;
            layer.isEditing = false;
            SceneView.duringSceneGui -= OnSceneView;

            EraseGridView();
            layer.SaveGridToAssets(layer.saveAsset);
        }

        public static void OnSceneView(SceneView sv)
        {
            Selection.activeGameObject = layer.gameObject;
            if (Selection.activeGameObject == layer.gameObject && isMouseInSceneView)
            {
                if (view == SKGridEditorView.None)
                {

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit);
                    Vector2Int curCell = layer.grid.PositionToCell(hit.point);
                    if (activeCell != curCell && activeCell != selectedCell)
                    {
                        layer.SetCellColor(activeCell.x, activeCell.y, layer.defaultColor);
                    }
                    activeCell = curCell;

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        if (selectedCell != new Vector2Int(-1, -1))
                        {
                            layer.EraseCellColor(selectedCell.x, selectedCell.y);
                            selectedCell = new Vector2Int(-1, -1);
                        }

                        selectedCell = activeCell;
                        layer.SetCellColor(activeCell.x, activeCell.y, new Color(1, 1, 1, 0.8f));

                    }
                    else if (activeCell != selectedCell)
                    {
                        layer.SetCellColor(activeCell.x, activeCell.y, selectColor);
                    }
                }
            }
            else
            {
                activeCell = new Vector2Int(-1, -1);
            }


        }
    }


    public enum SKGridEditorView
    {
        None,
        CellValue,
        CellCost,
        Occupancy
    }
}

#endif

