using UnityEditor;

namespace SKCell
{
    public class SKHierarchyEditor : EditorWindow
    {
        [MenuItem("Tools/SKCell/Tools/Hierarchy Style")]
        public static void ShowWindow()
        {
            GetWindow<SKHierarchyEditor>("Hierarchy Style");
        }
        private void OnGUI()
        {
            SKHierarchy.backgroundColor = EditorGUILayout.ColorField("Background Color", SKHierarchy.backgroundColor);
            //SKHierarchy.highlightColor = EditorGUILayout.ColorField("Separator Color", SKHierarchy.highlightColor);

            EditorGUILayout.Space();    
            EditorGUILayout.LabelField("To make a separator, end the game object with a '-'.");
        }
    }
}
