using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SKCell
{
    [InitializeOnLoad]
    public class SKHierarchy
    {
        public static Color backgroundColor = new Color(1, 1, 1, .04f);
        public static Color selectionColor = new Color(.6f, .6f, 1, 1f);
        public static Color highlightColor = new Color(.2f, .2f, 1f, .15f);
        public static Color highlightColorSolid = new Color(.215f, .215f, .337f);
        public static Color selectionColorDefault = new Color(.17f, .35f, .6f);
        public static Color normalFontColor = new Color(1, 1, 1, .8f);
        public static Color lineColor = new Color(1, 1, 1, .1f);
        public static Color lineColor2 = new Color(1, 1, 1, .2f);
        public static Color pickLineColor = new Color(1, .3f, .3f, .5f);

        private static GUIStyle rename_Style;
        private static readonly Vector2 OFFSET = Vector2.right * 18;
        static SKHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
            rename_Style = new GUIStyle();
            rename_Style.fixedHeight = 10;
            rename_Style.fixedWidth = 10;

            highlightColorSolid.a = 1;
        }
        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            FontStyle styleFont = FontStyle.Normal;
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            //separators
            bool isSeparator = obj != null && obj.name[obj.name.Length - 1] == '-';
            bool isPrefab = false;
            if (obj != null)
            {
                PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(obj);
                isPrefab = prefabType == PrefabAssetType.Regular;
            }

            if (isSeparator)
            {
                EditorGUI.DrawRect(selectionRect, highlightColor);
                Rect offsetRect = new Rect(selectionRect.position + OFFSET, selectionRect.size);

                Color bgColor = highlightColorSolid;
                if (Selection.activeGameObject?.GetInstanceID() == instanceID)
                {
                    bgColor = selectionColorDefault;
                }

                EditorGUI.DrawRect(offsetRect, bgColor);

                Rect separatorRect = new Rect(selectionRect.position + Vector2.up * 15 + Vector2.left * 17, new Vector2(selectionRect.size.x * 1.5f, 1));
                EditorGUI.DrawRect(separatorRect, lineColor2);
                EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white * (obj.activeSelf ? .9f : .5f) },
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                });
            }
            if (obj != null && !isSeparator)
            {
                //active toggle
                Rect activeRect = new Rect(selectionRect);
                if(!isPrefab)
                activeRect.xMax += 16;
                activeRect.xMin = activeRect.xMax - 16;
                activeRect.width = 16;

                bool prevActive = obj.activeSelf;
                bool isActive = GUI.Toggle(activeRect, prevActive, "");
                if (prevActive != isActive)
                {
                    obj.SetActive(isActive);
                    if (EditorApplication.isPlaying == false)
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(obj.scene);
                        EditorUtility.SetDirty(obj);
                    }
                }

                //activeRect.xMax -= 20;
                //activeRect.xMin -= 20;
                //activeRect.yMax += 3;
                //activeRect.yMin += 3;
                //if(GUI.Button(activeRect, SKAssetLibrary.Texture_A, rename_Style))
                //{
                //    Selection.activeGameObject = obj;
                //    CommonUtils.InvokeActionEditor(0.05f, () =>
                //    {
                //        EditorWindow.focusedWindow.SendEvent(Events.Rename);
                //    });
                //}

            }

            Rect lineRect = new Rect(selectionRect);
            lineRect.xMin -= 20;
            lineRect.xMax = lineRect.xMin + 3;
            Color col = lineColor;
            EditorGUI.DrawRect(lineRect, col);

            if (!isSeparator && (selectionRect.yMin / 16) % 2 == 0)
            {
                selectionRect.width *= 2;
                EditorGUI.DrawRect(selectionRect, backgroundColor);
            }
        }


        public static class Events
        {
            public static Event Rename = new Event() { keyCode = KeyCode.F2, type = EventType.KeyDown };
        }
    }
}