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
        public static GUIStyle transparentButtonStyle, blackButtonStyle;
        private static readonly Vector2 OFFSET = Vector2.right * 18;

        public static GUIContent srContent, cldContent;
        public static GUIContent crossContent, resetContent;
        public static Texture folderTexture;

        static SKHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;

            LoadResources();
            rename_Style = new GUIStyle();
            rename_Style.fixedHeight = 10;
            rename_Style.fixedWidth = 10;
            transparentButtonStyle = new GUIStyle();
            blackButtonStyle = new GUIStyle();

            Texture2D normalTexture = new Texture2D(1, 1);
            normalTexture.SetPixel(0, 0, Color.clear);
            normalTexture.Apply();

            Texture2D hoverTexture = new Texture2D(1, 1);
            hoverTexture.SetPixel(0, 0, new Color(1, 1, 1, 0.2f));
            hoverTexture.Apply();

            transparentButtonStyle.normal.background = normalTexture;
            transparentButtonStyle.hover.background = hoverTexture;

            transparentButtonStyle = new GUIStyle();

            normalTexture = new Texture2D(1, 1);
            normalTexture.SetPixel(0, 0, new Color(.1f, .1f, .1f, 0.9f));
            normalTexture.Apply();

            hoverTexture = new Texture2D(1, 1);
            hoverTexture.SetPixel(0, 0, new Color(.4f, .4f, .4f, 0.9f));
            hoverTexture.Apply();

            blackButtonStyle.normal.background = normalTexture;
            blackButtonStyle.hover.background = hoverTexture;

            highlightColorSolid.a = 1;
        }

        private static void LoadResources()
        {
            SKAssetLibrary.ClearTextureCache();
            srContent = new GUIContent(SKAssetLibrary.LoadTexture($"ObjectIcons/obj_icon_7"));
            cldContent = new GUIContent(SKAssetLibrary.LoadTexture($"ObjectIcons/obj_icon_11"));
            folderTexture = SKAssetLibrary.LoadTexture($"ObjectIcons/obj_icon_1");
            crossContent = new GUIContent(SKAssetLibrary.LoadTexture($"cross_1"));
            resetContent = new GUIContent(SKAssetLibrary.LoadTexture($"return"));
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (folderTexture==null)
            {
                LoadResources();
            }
            GameObject _object = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            GameObject obj = _object as GameObject;
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
                bgColor = new Color(.18f, .18f, .18f); //Updated color
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
                   // fontStyle = FontStyle.Bold,
                    font = SKAssetLibrary.DefaultFont,
                    fontSize = 12,
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
                
                /*
                Rect activeRect = new Rect(selectionRect);
                if (!isPrefab)
                    activeRect.xMax += 16;
                activeRect.xMin = activeRect.xMax - 16;
                activeRect.width = 16;

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    GUI.Button(activeRect, srContent, transparentButtonStyle);
                }
                */
            }

            Rect lineRect = new Rect(selectionRect);
            lineRect.xMin -= 20;
            lineRect.xMax = lineRect.xMin + 3;
            Color col = lineColor;
            EditorGUI.DrawRect(lineRect, col);

            if (obj!=null && !isSeparator && (selectionRect.yMin / 16) % 2 == 0)
            {
                selectionRect.width *= 2;
                EditorGUI.DrawRect(selectionRect, backgroundColor);
            }


            if (obj != null)
            {
                Rect iconRect = new Rect(selectionRect);
                iconRect.xMin -= 0;
                iconRect.xMax = iconRect.xMin + 16;
                //col = Selection.activeObject == obj?new Color(.17f,.35f,.6f): new Color(.25f,.25f,.25f);
                col = new Color(.25f, .25f, .25f);
                EditorGUI.DrawRect(iconRect, col);
                if (!isSeparator)
                {
                    Texture tex = GetHierarchyIcon(obj);
                    if (tex != null)
                    {
                        EditorGUI.DrawRect(iconRect, col);
                        GUI.DrawTexture(iconRect, tex);
                    }

                    if (GUI.Button(iconRect, "", transparentButtonStyle))
                    {
                        Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                        float differenceY = screenMousePosition.y - Event.current.mousePosition.y;
                        float iconScreenY = iconRect.y + differenceY;
                        Vector2 windowPosition = new Vector2(iconRect.x + 30, iconScreenY + 16);
                        ObjectIconPopup.ShowWindow(windowPosition, obj);

                    }
                }
                else
                {                
                    EditorGUI.DrawRect(iconRect, col);
                    GUI.DrawTexture(iconRect, folderTexture);
                }
            }
          
        }

        public static Texture GetHierarchyIcon(GameObject obj)
        {
                GUIContent content = EditorGUIUtility.ObjectContent(obj, typeof(GameObject));
                return content.image;
        }
        public static class Events
        {
            public static Event Rename = new Event() { keyCode = KeyCode.F2, type = EventType.KeyDown };
        }
    }

    public class ObjectIconPopup : EditorWindow
    {
        public float size = 20;

        public static GameObject go;
        public static GUIContent[] iconContents;
        public static GUIContent returnContent, crossContent;

        private static GUIStyle textStyle = new GUIStyle();
        private static Rect windowRect, titleRect, exitRect;

        private static Color bgColor = new Color(.1f, .1f, .1f);
        private static bool openAnim;
        private static Vector2 oPos;

        private static ObjectIconPopup window;
        public static void ShowWindow(Vector2 position, GameObject obj)
        {
            if (window != null)
                window.Close();

            go = obj;
            iconContents = new GUIContent[50];
            for (int i = 0; i < iconContents.Length; i++)
            {
                Texture texture = SKAssetLibrary.LoadTexture($"ObjectIcons/obj_icon_{i}");
                if (texture == null)
                    break;
                iconContents[i] = new GUIContent(texture);
            }
            returnContent = new GUIContent(SKAssetLibrary.LoadTexture($"return"));
            crossContent = new GUIContent(SKAssetLibrary.LoadTexture($"cross"));
            textStyle.fontSize = 12;
            textStyle.normal.textColor = Color.white;

            window = CreateInstance<ObjectIconPopup>();
            window.position = new Rect(position, new Vector2(250, 120));
            window.ShowPopup();

            windowRect = new Rect(0, 0, 250, 120);
            titleRect = new Rect(0, 0, 250, 16);
            exitRect = new Rect(250-10-3, 3, 10 , 10);
            bgColor = new Color(.1f, .1f, .1f);

            oPos = position;
            openAnim = true;
            t = 0;
        }
        private static float t;
        void OnGUI()
        {
            if (go == null)
            {
                window.Close();
                return;
            }
            if (openAnim)
            {
                t += EditorDeltaTime.DeltaTime *2;
                bgColor =Color.Lerp(new Color(.2f,.2f,.2f), new Color(.1f, .1f, .1f),t);
                if (t >= .8f)
                {
                    t = 1;
                    bgColor = new Color(.1f, .1f, .1f);
                    openAnim = false;
                }
            }


            EditorGUI.DrawRect(windowRect, bgColor);
            EditorGUI.DrawRect(titleRect, new Color(.35f, .35f, .35f));
            textStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label($"Select Icon for {go.name}", textStyle);
            textStyle.fontStyle = FontStyle.Normal;
            if (GUI.Button(exitRect, crossContent, SKHierarchy.blackButtonStyle))
            {
                Close();
            }
            GUILayout.Space(6);

            if (GUILayout.Button(returnContent, SKHierarchy.transparentButtonStyle, GUILayout.Width(size), GUILayout.Height(size)))
            {
                EditorGUIUtility.SetIconForObject(go, null);
                Close();
            }

            bool finished = false;
            int count = 0;
            while (!finished)
            {

                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 12; j++)
                {
                    int i = count * 12 + j;
                    if (i>=iconContents.Length || iconContents[i] == null)
                    {
                        finished = true;
                        break;
                    }
                    if (GUILayout.Button(iconContents[i], SKHierarchy.transparentButtonStyle, GUILayout.Width(size), GUILayout.Height(size)))
                    {
                        EditorGUIUtility.SetIconForObject(go, iconContents[i].image as Texture2D);
                        Close();
                    }
                }
                count++;
                EditorGUILayout.EndHorizontal();
            }
            window.Repaint();

        }
        void OnLostFocus()
        {
            Close();
        }
    }
}