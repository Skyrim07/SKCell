using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SKCell 
{
    [CustomEditor(typeof(SKMeasurer))]
    public class SKMeasurerEditor : Editor
    {
        SKMeasurer measurer;
        public static GUIStyle normalguiStyle = new GUIStyle();
        public static Color NORMAL_TEXT_COLOR = new Color(1f, 1f, 1f, 0.7f);
        private void OnEnable()
        {
            measurer = (SKMeasurer)target;

            normalguiStyle.font = SKAssetLibrary.DefaultFont;
            normalguiStyle.fontSize = 12;
            normalguiStyle.normal.textColor = NORMAL_TEXT_COLOR;
            normalguiStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnSceneGUI()
        {
            List<Transform> list = new List<Transform>();
            foreach(Transform t in measurer.measuredObjects)
                if(t!=null)
                    list.Add(t);
            Handles.color = new Color(1, 1, 1, 0.5f);
            if(list==null || list.Count == 0)
            {
                Handles.Label(measurer.transform.position, "Add another object to begin measure.", normalguiStyle);
            } 
            else
            {
                if (measurer.mode == SKMeasurerMode.EveryObject)
                {
                    list.Insert(0, measurer.transform);
                    for (int i = 0; i < list.Count; i++)
                    {
                        for(int j = list.Count-1; j >i; j--)
                        {
                            Handles.DrawLine(list[i].position, list[j].position);
                            Handles.Label((list[i].position + list[j].position) / 2, (list[i].position - list[j].position).magnitude.ToString("f2") + " m", normalguiStyle);
                        }
                    }
                }
                else if (measurer.mode == SKMeasurerMode.NextObject)
                {
                    Handles.DrawLine(measurer.transform.position, list[0].position);
                    Handles.Label((measurer.transform.position + list[0].position) / 2, (measurer.transform.position - list[0].position).magnitude.ToString("f2") + " m", normalguiStyle);
                    for (int i = 1; i < list.Count; i++)
                    {
                        Handles.DrawLine(list[i].position, list[i-1].position);
                        Handles.Label((list[i].position + list[i-1].position) / 2, (list[i].position - list[i-1].position).magnitude.ToString("f2") + " m", normalguiStyle);
                    }
                    
                }
            }
        }
    }
}