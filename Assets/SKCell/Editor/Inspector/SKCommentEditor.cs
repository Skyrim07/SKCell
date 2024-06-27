using UnityEditor;
using UnityEngine;

namespace SKCell
{
    [CustomEditor(typeof(SKComment))]
    public class SKCommentEditor : Editor
    {
        Color color = new Color(1, .8f, .3f);
        public override void OnInspectorGUI()
        {
            SKComment comment = (SKComment)target;
            GUI.color = color;
            comment.comment = EditorGUILayout.TextArea(comment.comment);
            GUI.color = Color.white;
        }
    }
}