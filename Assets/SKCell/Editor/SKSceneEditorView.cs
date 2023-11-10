using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SKCell 
{
    [InitializeOnLoad]
    public class SKSceneEditorView
    {
        static string objCountStr = string.Empty;
        static bool objCountDisplayOn = false;
        static SKSceneEditorView() 
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorSceneManager.activeSceneChangedInEditMode += UpdateObjectCount;
        }
        static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.FlexibleSpace();
            GUI.skin.label.fontSize = 10;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(objCountStr);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Object Count", EditorStyles.miniButtonRight, GUILayout.Width(85)))
            {
                ToggleObjectCount();
            }

            EditorGUILayout.EndHorizontal();
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            Handles.EndGUI();
        }

        private static void ToggleObjectCount()
        {
            objCountStr = (objCountDisplayOn = !objCountDisplayOn) ? $"{GameObject.FindObjectsOfType<Transform>().Length} objects\n" +
                $"{GameObject.FindObjectsOfType<SpriteRenderer>().Length} sprites\n"+
                $"{GameObject.FindObjectsOfType<MaskableGraphic>().Length} UI graphics\n"+
                $"{GameObject.FindObjectsOfType<Collider>().Length+ GameObject.FindObjectsOfType<Collider2D>().Length} colliders" : string.Empty;
        }

        private static void UpdateObjectCount(UnityEngine.SceneManagement.Scene s1, UnityEngine.SceneManagement.Scene s2)
        {
            if (objCountDisplayOn)
            {
                ToggleObjectCount();
                ToggleObjectCount();
            }
        }
    }
}