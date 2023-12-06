using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace SKCell
{
    public class SKAssetReferencer : EditorWindow
    {
        private Object assetToFind;
        private Dictionary<string, List<string>> scenesWithReferences = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> prefabsWithReferences = new Dictionary<string, List<string>>();
        private Vector2 scrollPosition;

        [MenuItem("Tools/SKCell/Tools/SK Asset Referencer")]
        public static void ShowWindow()
        {
            GetWindow<SKAssetReferencer>("SK Asset Referencer");
        }

        [MenuItem("Assets/SKAssetReferencer", false, 200)]
        private static void FindReferencesToAsset()
        {
            var selectedObject = Selection.activeObject;
            if (selectedObject != null)
            {
                var window = GetWindow<SKAssetReferencer>("SK Asset Referencer");
                window.SetAssetToFind(selectedObject);
            }
        }

        void OnGUI()
        {
            GUILayout.Label("Select Asset to Find", EditorStyles.boldLabel);
            assetToFind = EditorGUILayout.ObjectField("Asset: ", assetToFind, typeof(Object), false);

            if (GUILayout.Button("Find References"))
            {
                FindReferences();
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            int osize = GUI.skin.label.fontSize;
            if (scenesWithReferences.Count > 0)
            {
                GUILayout.Label("Scenes with References:");
                foreach (var sceneEntry in scenesWithReferences)
                {
                    if (GUILayout.Button(sceneEntry.Key, EditorStyles.linkLabel))
                    {
                        SelectAndPingAsset(sceneEntry.Key);
                    }
                    GUI.skin.label.fontSize = 10;
                    foreach (var objName in sceneEntry.Value)
                    {
                        GUILayout.Label(" - " + objName);
                    }
                }
            }

            if (prefabsWithReferences.Count > 0)
            {
                GUI.skin.label.fontSize = osize;
                GUILayout.Label("\nPrefabs with References:");
                foreach (var prefabEntry in prefabsWithReferences)
                {
                    if (GUILayout.Button(prefabEntry.Key, EditorStyles.linkLabel))
                    {
                        SelectAndPingAsset(prefabEntry.Key);
                    }
                    GUI.skin.label.fontSize = 10;
                    foreach (var objName in prefabEntry.Value)
                    {
                        GUILayout.Label(" - " + objName);
                    }
                }
            }
            GUI.skin.label.fontSize = osize;
            GUILayout.EndScrollView();
        }

        private void FindReferences()
        {
            if (assetToFind == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an asset to find.", "OK");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(assetToFind);
            scenesWithReferences.Clear();
            prefabsWithReferences.Clear();
            string originalScene = EditorSceneManager.GetActiveScene().path;

            foreach (string scenePath in EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes))
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var foundObjects = FindReferencesInScene(assetPath);
                if (foundObjects.Count > 0)
                {
                    scenesWithReferences.Add(scenePath, foundObjects);
                }
            }

            FindReferencesInPrefabs(assetPath);

            if (originalScene != null)
            {
                EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
            }

            Repaint();
        }

        private List<string> FindReferencesInScene(string assetPath)
        {
            List<string> foundObjects = new List<string>();
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                Component[] components = obj.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component == null)
                        continue;
                    SerializedObject serializedObject = new SerializedObject(component);
                    SerializedProperty serializedProperty = serializedObject.GetIterator();

                    while (serializedProperty.NextVisible(true))
                    {
                        if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (serializedProperty.objectReferenceValue != null)
                            {
                                string path = AssetDatabase.GetAssetPath(serializedProperty.objectReferenceValue);
                                if (path == assetPath)
                                {
                                    foundObjects.Add(obj.name);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return foundObjects;
        }

        private void FindReferencesInPrefabs(string assetPath)
        {
            string[] allPrefabs = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in allPrefabs)
            {
                string relativePath = "Assets" + prefabPath.Replace(Application.dataPath, "").Replace('\\', '/');
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                if (prefab != null)
                {
                    Component[] components = prefab.GetComponentsInChildren<Component>(true);
                    List<string> foundObjectNames = new List<string>();
                    foreach (Component component in components)
                    {
                        if (component == null)
                            continue;

                        SerializedObject serializedObject = new SerializedObject(component);
                        SerializedProperty serializedProperty = serializedObject.GetIterator();

                        while (serializedProperty.NextVisible(true))
                        {
                            if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                if (serializedProperty.objectReferenceValue != null)
                                {
                                    string path = AssetDatabase.GetAssetPath(serializedProperty.objectReferenceValue);
                                    if (path == assetPath)
                                    {
                                        foundObjectNames.Add(component.gameObject.name);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (foundObjectNames.Count > 0)
                    {
                        prefabsWithReferences.Add(relativePath, foundObjectNames);
                    }
                }
            }
        }

        private void SelectAndPingAsset(string assetPath)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
        }
        public void SetAssetToFind(Object asset)
        {
            assetToFind = asset;
        }
    }
}
