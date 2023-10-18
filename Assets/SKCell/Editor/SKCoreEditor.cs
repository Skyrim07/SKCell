using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SKCell
{
    [CustomEditor(typeof(SKCore))]
    public class SKCoreEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.skin.label.fontSize = 12;
            Rect entireRect = EditorGUILayout.GetControlRect();
            entireRect.x -= 20;
            entireRect.y -= 5;
            entireRect.width = 1000;
            entireRect.height = 70;
           // EditorGUI.DrawRect(entireRect, new Color(.15f, .15f, .15f));
            GUI.DrawTexture(new Rect(new Vector2(10, 0), new Vector2(450, 64)), SKAssetLibrary.Texture_Logo);
            GUILayout.Space(57);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(""+SKCore.SKCELL_VERSION);
            if (GUILayout.Button("Github"))
            {
                Application.OpenURL("https://github.com/Skyrim07/SKCell");
            }
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://skyrim07.github.io/SKCell/#/");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("by Alex Liu");
            GUILayout.Space(5);
            base.OnInspectorGUI();
        }
    }
}
