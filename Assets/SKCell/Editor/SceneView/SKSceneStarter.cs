using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor.SceneManagement;

namespace SKCell
{
    [InitializeOnLoad]
    public class SKSceneStarter
    {
        private static GenericMenu dropDownMenu;
        private static GUIStyle dropDownStyle;
        static SKSceneStarter()
        {
            SceneView.duringSceneGui += OnSceneGUI;

        }
        private static void OnSceneGUI(SceneView sceneView)
        {
            Vector2 buttonSize = new Vector2(150, 20);
            Rect buttonRect = new Rect(sceneView.position.width - buttonSize.x, sceneView.position.height - buttonSize.y - 25, buttonSize.x, buttonSize.y);

            InitGUIStyles();

            Handles.BeginGUI();
            int count = SceneManager.sceneCountInBuildSettings;


            if (dropDownMenu == null)
            {
                dropDownMenu = new GenericMenu();
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int i = 0; i < scenes.Length; i++)
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                    dropDownMenu.AddItem(new GUIContent(sceneName), false, OnMenuOptionSelected, sceneName);
                }
            }

            GUIContent content = new GUIContent();
            content.text = EditorSceneManager.GetActiveScene().name;
            if (content.text == null || content.text.Length < 1)
                content.text = "<Select scene...>";

            if (GUI.Button(buttonRect, content, dropDownStyle))
            {
                dropDownMenu.DropDown(buttonRect);
            }

            Handles.EndGUI();
        }

        private static void InitGUIStyles()
        {
            dropDownStyle = new GUIStyle(GUIStyle.none);
            dropDownStyle.alignment = TextAnchor.MiddleCenter;
            dropDownStyle.normal.textColor = new Color(1, 1, 1, .8f);
            dropDownStyle.normal.background = MakeTex(2, 2, new Color(0.25f, 0.25f, 0.25f, .6f));
            dropDownStyle.font = SKAssetLibrary.DefaultFont; dropDownStyle.fontSize = 12;
        }

        private static void OnMenuOptionSelected(object sceneName)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes.First(s => System.IO.Path.GetFileNameWithoutExtension(s.path) == (string)sceneName).path);
        }
        private static Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        static bool IsMouseInRect(Rect rect)
        {
            Event e = Event.current;
            return rect.Contains(e.mousePosition);
        }
    }
}