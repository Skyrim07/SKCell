using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SKCell {
    public class SKTranslatorPreview
    {
        public static Color TIME_OUTLINE_COLOR = new Color(1f, 0.97f, 0.43f, 0.4f);
        public static List<GameObject> preview = new List<GameObject>();
        public static GameObject timePreview;

        public static SKPathDesigner translator;

        static SKTranslatorPreview()
        {
            Selection.selectionChanged += SelectionChanged;
        }

        private static void SelectionChanged()
        {
            if (preview != null && !preview.Contains(Selection.activeGameObject))
            {
               // DestroyPreview();
            }
        }
        public static void NewPreview(SKPathDesigner origin)
        {
            if (preview != null)
            {
                DestroyPreview();
            }

            if (origin.waypoints.Count == 0)
                return;

            translator = origin;

            for (int i=0;i<translator.waypoints.Count;++i) {

                SKTranslatorWaypoint point = translator.waypoints[i];
                GameObject go = Object.Instantiate(origin.gameObject);
                go.transform.position = point.localPosition+origin.transform.position;
                preview.Add(go);
                go.hideFlags = HideFlags.HideAndDontSave; 
                SKPathDesigner trans = go.GetComponentInChildren<SKPathDesigner>();
                Object.DestroyImmediate(trans);


                Color c = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                SpriteRenderer[] rends = go.GetComponentsInChildren<SpriteRenderer>();
                for (int j = 0; j < rends.Length; ++j)
                    rends[j].color = c;
            }

            timePreview = Object.Instantiate(origin.gameObject); 
            timePreview.transform.position = origin.transform.position;
            timePreview.hideFlags = HideFlags.HideAndDontSave;
            Object.DestroyImmediate(timePreview.GetComponentInChildren<SKPathDesigner>());
            Color c2 = new Color(0.8f, 1, 0.8f, 0.1f);
            SpriteRenderer[] rendt = timePreview.GetComponentsInChildren<SpriteRenderer>();
            for (int j = 0; j < rendt.Length; ++j)
            {
                rendt[j].color = c2;
                //SKSpriteProcessing ssp = rendt[j].gameObject.AddComponent<SKSpriteProcessing>();
                //ssp.rimColor = TIME_OUTLINE_COLOR;
                //ssp.active = true;
                //ssp.baseAlphaThreshold = 0.3f;
            }
            
        }

        public static void DestroyPreview()
        {
            if (preview == null)
                return;
            foreach (var item in preview)
            {
                Object.DestroyImmediate(item);
            }
            preview.Clear();
            if (timePreview != null)
            {
                Object.DestroyImmediate(timePreview);
            }
        }
    }
}
