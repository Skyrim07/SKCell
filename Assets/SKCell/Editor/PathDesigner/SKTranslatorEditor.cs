using UnityEngine;
using UnityEditor;

namespace SKCell
{
    [CustomEditor(typeof(SKPathDesigner))]
    public class SKTranslatorEditor : Editor
    {
        private static Color HANDLE_COLOR = new Color(0.6f, 0.9f, 0.6f, 0.7f);
        public static Color TIME_OUTLINE_COLOR = new Color(1f, 0.97f, 0.43f, 0.8f);
        public static Color NORMAL_TEXT_COLOR = new Color(1f, 1f, 1f, 0.9f);
        public static Color BEZIER_CONTROL_COLOR = new Color(0.6f, 1f, 7f, 0.6f);

        public static float BUTTON_DROPDOWN_DIST = 2;
        static SKPathDesigner translator;

        public static GUIStyle timeguiStyle = new GUIStyle();
        public static GUIStyle normalguiStyle = new GUIStyle();
        public static GUIStyle minorguiStyle = new GUIStyle();
        public static GUIStyle headerguiStyle = new GUIStyle();

        private static int delete = -1;
        public static bool hideBezierEdit = false;
        public static bool hideTimePreview = false;

        private static bool isPlaying = false;

        private void OnEnable()
        {
            translator = (SKPathDesigner)target;
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                SKTranslatorPreview.NewPreview(translator);
            translator.UpdateDistances();
            translator.UpdateBezier();
            InitGUIStyles();
        }

        private static void InitGUIStyles()
        {
            timeguiStyle.font = SKAssetLibrary.DefaultFont;
            timeguiStyle.fontStyle = FontStyle.Bold;
            timeguiStyle.normal.textColor = TIME_OUTLINE_COLOR;
            timeguiStyle.alignment = TextAnchor.MiddleCenter;

            normalguiStyle.font = SKAssetLibrary.DefaultFont;
            normalguiStyle.fontSize = 12;
            normalguiStyle.normal.textColor = NORMAL_TEXT_COLOR;
            normalguiStyle.alignment = TextAnchor.MiddleCenter;

            headerguiStyle.fontStyle = FontStyle.Bold;
            headerguiStyle.normal.textColor = NORMAL_TEXT_COLOR;
            headerguiStyle.alignment = TextAnchor.UpperLeft;

            minorguiStyle.fontSize = 12;
            minorguiStyle.normal.textColor = HANDLE_COLOR;
            minorguiStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnDisable()
        {
            SKTranslatorPreview.DestroyPreview();
        }
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Path Player", headerguiStyle);
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
            if (!Application.isPlaying && GUILayout.Button("<Play>", GUILayout.Height(20)))
            {
                playDir = 1;
                translator.normalizedTime = 0;
                isPlaying = true;
            }
            if (!Application.isPlaying && GUILayout.Button("<Stop>", GUILayout.Height(20)))
            {
                translator.normalizedTime = 0;
                isPlaying = false;
            }
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white; 

            GUILayout.Label("General Settings", headerguiStyle);
            base.OnInspectorGUI();
            GUILayout.Space(10);
          
            GUILayout.Label("Waypoint Editor", headerguiStyle);
            GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
            if ( GUILayout.Button("<Add Waypoint>"))
            {
                SKTranslatorWaypoint wp = new SKTranslatorWaypoint()
                {
                    localPosition = translator.GetLastWayPoint() == null ? (Vector3.right * 5) : (translator.GetLastWayPoint().localPosition + Vector3.right * 5),
                    bezier = new SKBezier(),
                };
                wp.curve = SKCurve.LinearIn; 
                wp.bezier.p1 = wp.localPosition + Vector3.right;
                wp.bezier.p2 = wp.localPosition + Vector3.right*2;
                translator.AddWaypoint(wp);
                EditorUtility.SetDirty(translator);
                SKTranslatorPreview.NewPreview(translator);
            }
            GUI.contentColor = Color.white;
            hideBezierEdit=EditorGUILayout.Toggle("Hide Bezier Points", hideBezierEdit);
            hideTimePreview = EditorGUILayout.Toggle("Hide Time Preview", hideTimePreview);

            GUILayout.Space(10);
            int size = 64;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Node Self", headerguiStyle, GUILayout.Width(size));
            translator.selfWaitTime = EditorGUILayout.FloatField("Wait Time", translator.selfWaitTime);
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < translator.waypoints.Count; i++)
            {
                SKTranslatorWaypoint point = translator.waypoints[i];

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical(GUILayout.Width(size));
                EditorGUILayout.LabelField("Node " + i, headerguiStyle, GUILayout.Width(size));
                GUI.contentColor = new Color(0.9f, 0.8f, 0.7f);
                if (GUILayout.Button("Delete", GUILayout.Width(size)))
                {
                    delete = i;
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                point.type = (SKTranslatorWaypointType)EditorGUILayout.EnumPopup(point.type, GUILayout.Width(size));
                point.curve.curveType = (CurveType)EditorGUILayout.EnumPopup(point.curve.curveType, GUILayout.Width(size));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                point.localPosition = EditorGUILayout.Vector3Field("Position", point.localPosition);

                EditorGUILayout.BeginHorizontal();
                point.stayTime = EditorGUILayout.FloatField("Wait Time", point.stayTime);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
            }
            if (delete >= 0)
            {
                translator.DeleteWaypoint(delete);
                EditorUtility.SetDirty(translator);
                SKTranslatorPreview.NewPreview(translator);
                delete = -1;
            }
        }

        private void OnSceneGUI()
        {
            if (Application.isPlaying)
                return;

            UpdatePlay();
            UpdateWaypointPreview();
        }

        int playDir = 1;
        private void UpdatePlay()
        {
            if (!isPlaying)
                return;
            translator.normalizedTime += 0.02f * playDir;
            if (translator.translateMode == TranslateMode.OneTime)
            {
                if (translator.normalizedTime >= 1.0f)
                {
                    translator.normalizedTime = 1.0f;
                    isPlaying = false;
                }
            }
            else if (translator.translateMode== TranslateMode.PingPong)
            {
                if (translator.normalizedTime >= 1.0f)
                {
                    playDir = -1;
                }
                if (translator.normalizedTime <= 0.0f)
                {
                    playDir = 1;
                }
            }
            else if (translator.translateMode == TranslateMode.Repeat)
            {
                if (translator.normalizedTime >= 1.0f)
                {
                    translator.normalizedTime = 0.0f;
                }
            }
        }
        private void UpdateWaypointPreview()
        {
            for (int i = 0; i < translator.waypoints.Count; i++)
            {
                Vector3 wPos = translator.waypoints[i].localPosition+translator.transform.position;
                Vector3 newWPos = Handles.PositionHandle(wPos, Quaternion.identity);
                Handles.color = NORMAL_TEXT_COLOR;
                if (translator.waypoints[i].type == SKTranslatorWaypointType.Line)
                {
                    if (i == 0)
                    {
                        Handles.DrawDottedLine(wPos, translator.transform.position, 10);
                        Handles.Label((wPos+ translator.transform.position)/2, translator.waypoints[i].curve.curveType.ToString(), minorguiStyle);
                    }
                    else
                    {
                        Handles.DrawDottedLine(wPos, translator.waypoints[i - 1].localPosition + translator.transform.position, 10);
                        Handles.Label((wPos + translator.waypoints[i - 1].localPosition + translator.transform.position) / 2, translator.waypoints[i].curve.curveType.ToString(), minorguiStyle);
                    }
                }
                else if (translator.waypoints[i].type == SKTranslatorWaypointType.Bezier)
                {
                    SKBezier b = translator.waypoints[i].bezier;
                    Vector3 o = translator.transform.position;
                    Vector3 w = translator.waypoints[i].localPosition + o;
                    if (!hideBezierEdit)
                    {
                        Vector3 wp1 = Handles.PositionHandle(b.p1 + w, Quaternion.identity);
                        Vector3 wp2 = Handles.PositionHandle(b.p2 + w, Quaternion.identity);
                        if (wp1 != b.p1 + w)
                            b.p1 = wp1 - w;
                        if (wp2 != b.p2 + w)
                            b.p2 = wp2 - w;

                        Handles.color = BEZIER_CONTROL_COLOR;
                        Handles.DrawLine(b.p0 + w, b.p1 + w);
                        Handles.DrawLine(b.p3 + w, b.p2 + w);
                    }
                    Vector3[] path = b.Path(translator.bezierSegments, w);
                    Handles.DrawDottedLines(path, b.SegmentIndices(translator.bezierSegments), 10);
                    Handles.Label(path[path.Length/2], translator.waypoints[i].curve.curveType.ToString(), minorguiStyle) ;

                    Handles.color = NORMAL_TEXT_COLOR;
                }

                if (wPos != newWPos)
                {
                    translator.waypoints[i].localPosition = newWPos-translator.transform.position;
                    translator.UpdateDistances();
                    translator.UpdateBezier();
                }
                SKTranslatorPreview.preview[i].transform.position = newWPos;
                Handles.Label(newWPos, $"{i}, wait {translator.waypoints[i].stayTime.ToString("f1")} s", normalguiStyle);
            }
            EditorUtility.SetDirty(translator);
            Handles.Label(translator.transform.position+Vector3.down, $"wait {translator.selfWaitTime.ToString("f1")} s", normalguiStyle);
            Vector3 normPos = translator.GetNormalizedWPosition();
            if(SKTranslatorPreview.timePreview)
                SKUtils.SetActiveEfficiently(SKTranslatorPreview.timePreview, !hideTimePreview);
            if (SKTranslatorPreview.timePreview)
            {
                SKTranslatorPreview.timePreview.transform.position = normPos;
                if(!hideTimePreview)
                Handles.Label(normPos, $"{translator.normalizedTime.ToString("p0")}", timeguiStyle);
            }
        }
    }
}
