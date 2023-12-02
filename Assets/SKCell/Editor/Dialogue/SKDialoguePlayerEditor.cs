using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SKCell
{
    [CustomEditor(typeof(SKDialoguePlayer))]
    public class SKDialoguePlayerEditor : Editor
    {
        SKDialoguePlayer player;
        SKDialogueEditor editor;
        private void OnEnable()
        {
            player = (SKDialoguePlayer)target;
        }

        public override void OnInspectorGUI()
        {
            GUI.color = new Color(.9f, .8f, .7f);
            if(GUILayout.Button("<Open Dialogue Editor>"))
            {
                editor = (SKDialogueEditor)EditorWindow.GetWindow<SKDialogueEditor>("SK Dialogue Editor", typeof(UnityEditor.SceneView));
                editor.LoadAsset(player.asset);
            }
            GUI.color = Color.white;
            if (GUILayout.Button("<New Dialogue Asset>"))
            {
                string path = EditorUtility.SaveFilePanel("Create new dialogue asset", "Assets/", "New Dialogue Asset", "asset");
                if (path.IndexOf("Assets") > 0)
                {
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SKDialogueAsset>(), path.Substring(path.IndexOf("Assets")));
                    player.asset = AssetDatabase.LoadAssetAtPath<SKDialogueAsset>(path.Substring(path.IndexOf("Assets")));
                    editor = (SKDialogueEditor)EditorWindow.GetWindow(typeof(SKDialogueEditor));
                    editor.LoadAsset(player.asset);
                }
            }
            if (GUILayout.Button("<Generate Structure>"))
            {
                string pathSuffix = "/SKDialogue.prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
                if (prefab == null)
                {
                    SKUtils.EditorLogError("SKButton Resource Error: Button prefab lost.");
                    return;
                }
                GameObject button = Instantiate(prefab);
                button.name = $"SKDialogue-{GetHashCode()}";
                button.transform.SetParent(player.transform.parent);
                button.transform.CopyFrom(player.transform);
                button.transform.SetSiblingIndex(player.transform.GetSiblingIndex());
                Selection.activeGameObject = button;
                DestroyImmediate(player.gameObject);
            }
            else
            base.OnInspectorGUI();
        }
    }
}
