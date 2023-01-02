using UnityEditor;

namespace SKCell
{
    public class SKHierarchyEditor : EditorWindow
    {
        [MenuItem("SKCell/Hierarchy Editor")]
        public static void ShowWindow()
        {
            GetWindow<SKHierarchyEditor>("HierarchyEditor");
        }
        private void OnGUI()
        {
            SKHierarchy.backgroundColor = EditorGUILayout.ColorField("Background Color", SKHierarchy.backgroundColor);
            SKHierarchy.selectionColor = EditorGUILayout.ColorField("Selection Color", SKHierarchy.selectionColor);
            SKHierarchy.normalFontColor = EditorGUILayout.ColorField("Normal Text Color", SKHierarchy.normalFontColor);
        }
    }
}
