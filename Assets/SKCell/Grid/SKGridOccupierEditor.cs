using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace SKCell
{
    [CustomEditor(typeof(SKGridOccupier))]
    [CanEditMultipleObjects]
    public class SKGridOccupierEditor : Editor
    {
        SKGridOccupier o;
        bool inDisplay;
        public override void OnInspectorGUI()
        {
            o = target as SKGridOccupier;
            if (!inDisplay)
            {
                if (GUILayout.Button("<Display Occupied Cells>"))
                {
                    inDisplay = true;
                    o.CalculateAndDisplayOccupiedCells();
                }
            }
            else
            {
                if (GUILayout.Button("<End Display>"))
                {
                    inDisplay = false;
                    o.StopEditorDisplay();
                }
            }
            base.OnInspectorGUI();
        }
    }
}
#endif
