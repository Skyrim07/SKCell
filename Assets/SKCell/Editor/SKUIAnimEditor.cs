using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SKUIAnimation))]
    [CanEditMultipleObjects]
    public class SKUIAnimEditor : Editor
    {
        static SKUIAnimation skAnim;
        public override void OnInspectorGUI()
        {
            skAnim = (SKUIAnimation)target;

            GUILayout.Label("This is a two-state animator.");
            GUILayout.Label("Use bool 'Appear' as condition.");
            GUILayout.Space(20);
            if (Application.isPlaying)
            {
                if (GUILayout.Button("<Preview State: Enter>", GUILayout.Height(20)))
                {
                    skAnim.SetState(true);
                }
                if (GUILayout.Button("<Preview State: Exit>", GUILayout.Height(20)))
                {
                    skAnim.SetState(false);
                }
            }
            base.OnInspectorGUI();
        }
    }
#endif
}
