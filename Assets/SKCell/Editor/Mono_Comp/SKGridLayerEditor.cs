using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SKCell
{
    [CustomEditor(typeof(SKGridLayer))]
    public class SKGridLayerEditor : Editor
    {
        SKGridLayer gridLayer;
        public override void OnInspectorGUI()
        {
            gridLayer = target as SKGridLayer;

            GUI.contentColor = new Color(0.9f,0.8f, 0.7f);
            if (!gridLayer.isEditing)
            {
                if (GUILayout.Button("<Edit Grid>"))
                {
                    gridLayer.preview = true;
                    SKGridEditor.Initialize(gridLayer);
                    SKUtils.RefreshSelection(gridLayer.gameObject);
                }
            }
            else
            {
                if (GUILayout.Button("<Finish Edit>"))
                {
                    SKGridEditor.FinishEdit(gridLayer);
                    SKUtils.RefreshSelection(gridLayer.gameObject);
                }
            }
            GUI.contentColor = Color.white;
            if (!gridLayer.preview)
            {
                if (GUILayout.Button("<Preview>"))
                {
                    gridLayer.preview = true;
                    SKUtils.RefreshSelection(gridLayer.gameObject);
                }
            }
            else
            {
                if (GUILayout.Button("<End Preview>"))
                {
                    gridLayer.preview = false;
                    SKUtils.RefreshSelection(gridLayer.gameObject);
                }
            }
            GUILayout.Space(15);
                GUILayout.Label("Generate a new grid according to the parameters below.\r\nCannot preserve existing data.");
                if (GUILayout.Button("<Generate New Grid>"))
                {
                    gridLayer.GenerateStructure();
                }
                GUILayout.Label("Apply changes made in the inspector to the grid.");
                if (GUILayout.Button("<Apply Changes>"))
                {
                    gridLayer.ApplyChanges();
                    SKUtils.EditorLogNormal($"Grid Changes Applied");
                }
            
            GUILayout.Space(15);
            GUILayout.Label("Create a grid asset to save data in this grid.\r\nSave or load grid data by the asset below.");
            if (GUILayout.Button("<Save Grid>"))
            {
                gridLayer.SaveGridToAssets(gridLayer.saveAsset);
                SKUtils.EditorLogNormal($"Saved Grid: {gridLayer.saveAsset.name}");
            }
            if (GUILayout.Button("<Load Grid>"))
            {
                gridLayer.LoadGridFromAssets(gridLayer.saveAsset);
                SKUtils.EditorLogNormal($"Loaded Grid: {gridLayer.saveAsset.name}");
            }
            EditorGUILayout.BeginHorizontal();
            gridLayer.saveAsset = EditorGUILayout.ObjectField(gridLayer.saveAsset, typeof(SKGridAsset), false) as SKGridAsset;
            if (GUILayout.Button("New"))
            {
                string path = EditorUtility.SaveFilePanel("Create new grid file", "Assets/", "New Grid", "asset");
                if (path.IndexOf("Assets") > 0)
                {
                    AssetDatabase.CreateAsset(new SKGridAsset(), path.Substring(path.IndexOf("Assets")));
                    gridLayer.saveAsset = AssetDatabase.LoadAssetAtPath<SKGridAsset>(path.Substring(path.IndexOf("Assets")));
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(15);

            if(gridLayer.grid!=null )
                gridLayer.grid.bottomLeft = gridLayer.transform.position;
            base.OnInspectorGUI();
        }
       
    }
}
#endif
