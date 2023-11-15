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
    [CustomEditor(typeof(SKSlider))]
    [CanEditMultipleObjects]
    public class SKSliderEditor : Editor
    {
        static SKSlider skSlider;
        public override void OnInspectorGUI()
        {
            skSlider = (SKSlider)target;

            if (GUILayout.Button("<Generate structure>", GUILayout.Height(30)))
            {
                skSlider.GenerateStructure();
            }
            if (skSlider.initialized)
            {
                base.OnInspectorGUI();
                if(!Application.isPlaying)
                skSlider.DrawEditorPreview();
            }
        }
    }
#endif
}
