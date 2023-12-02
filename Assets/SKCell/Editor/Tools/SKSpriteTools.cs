using System.IO;
using UnityEngine;
using UnityEditor;

namespace SKCell
{
    public class SKSpriteTools : EditorWindow
    {
        private static Color color = new Color(1,1,1,0);
        private static Texture2D texture;
        private static string texturePath;

        [MenuItem("Tools/SKCell/Tools/Sprite Colorer")]
        public static void Initialize()
        {
            GetWindow<SKSpriteTools>("Sprite Colorer");
        }
        private void OnGUI()
        {
            color = EditorGUILayout.ColorField(color);
            color.a = 1;
            texture = EditorGUILayout.ObjectField(texture, typeof(Texture2D), false) as Texture2D;
            texturePath = AssetDatabase.GetAssetPath(texture);

            if(GUILayout.Button("Assign Color"))
            {
                AssignColor();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("1. Find and drag the sprite in.");
            EditorGUILayout.LabelField("2. Turn 'Read/Write' on.");
            EditorGUILayout.LabelField("3.Set format to RGBA 32 bit.");
            EditorGUILayout.LabelField("4.Choose a color to assign.");
        }
        public static void AssignColor()
        {
            Color c;
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                     c = texture.GetPixel(i, j);
                    if(c.a>0.1f)
                        texture.SetPixel(i, j, color);
                }
            }
            texture.Apply();

            byte[] itemBGBytes = texture.EncodeToPNG();
            File.WriteAllBytes(texturePath, itemBGBytes);
        }
    }
}
