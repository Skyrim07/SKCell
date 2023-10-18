using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace SKCell
{
    [InitializeOnLoad]
    public class SKFolderIconEditor
    {
        private static Dictionary<string, string> folderTypeCache = new Dictionary<string, string>();
        private static Dictionary<string, int> fileTypeCounts = new Dictionary<string, int>();

        private static Dictionary<string, Color> folderColors = new Dictionary<string, Color>()
        {
            {"SKCell", new Color(1.0f, 0.9f,0.7f, 1f)},
            {"Editor", new Color(0.3f, 0.65f,1.0f, 1f)},
            {"Prefabs", new Color(0.6f, 0.95f,1.0f, 1f)},
            {"Prefab", new Color(0.6f, 0.95f,1.0f, 1f)},
            {"Resources", new Color(0.3f, 0.95f,0.4f, 1f)},
            {"Scenes", new Color(0.83f, 0.72f,0.61f, 1f)},
            {"TextMesh Pro", new Color(0.93f, 0.42f,0.41f, 1f)},
            {"StreamingAssets", new Color(0.92f, 0.63f,0.96f, 1f)},
        };
        static SKFolderIconEditor()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
            UpdateFolderCache();
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if(string.IsNullOrEmpty(path))
                return;
            if (!AssetDatabase.IsValidFolder(path))
                return;
            bool isSelected = Selection.assetGUIDs.Contains(guid);
            Rect folderRect = new Rect(selectionRect);

            int fileCount = 0;
            if (!folderTypeCache.TryGetValue(path, out string dominantType))
            {
                dominantType = DetermineDominantFileType(path, out fileCount);
                folderTypeCache[path] = dominantType;
            }

            string iconPath = "FolderIcons/folder";
            float aspect = selectionRect.width / selectionRect.height;
            if (aspect<=1.0f)
            {
                folderRect.y -= 2;
                folderRect.height -= 10;
                foreach (var item in folderColors.Keys)
                {
                    int index = Mathf.Max(0, path.LastIndexOf('/') + 1);
                    if (path.Substring(index) == item)
                    {
                        GUI.DrawTexture(folderRect, SKAssetLibrary.LoadTexture(iconPath), ScaleMode.ScaleToFit, true, 0, folderColors[item], 0, 0);
                    }
                }
                folderRect = new Rect(selectionRect);
                Vector2 ocenter = folderRect.center;
                folderRect.width *= 1.2f;
                folderRect.height *= 1.0f;
                folderRect.center = ocenter;
                folderRect.y += folderRect.width * .08f;

                Color col = isSelected ? new Color(1, 1, 1, .06f) : new Color(0, 0, 0, .00f);
                GUI.DrawTexture(folderRect, SKAssetLibrary.LoadTexture("sq"), ScaleMode.ScaleToFit, true, 0, col, 0, 5);
            }

            Texture iconTexture = dominantType == "other" ? null : SKAssetLibrary.LoadTexture($"FolderIcons/{dominantType}");
            if (iconTexture)
            {
                float size = selectionRect.width / 2.5f;
                Rect iconRect = (aspect<=1.0f)
                    ? new Rect(selectionRect.xMax - selectionRect.width / 2.5f, selectionRect.y + selectionRect.width / 1.6f, size, size)
                    : new Rect(selectionRect.xMax - 20, selectionRect.y, 15, 15);

                GUI.DrawTexture(iconRect, iconTexture);
            }


        }

        private static string DetermineDominantFileType(string folderPath, out int fileCount)
        {
            fileTypeCounts.Clear();
            fileCount = 0;
            if (folderPath.EndsWith("Editor"))
            {
                return "settings";
            }
            if (folderPath.EndsWith("Assets"))
            {
                return "other";
            }
            foreach (var file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                fileCount++;
                string ext = Path.GetExtension(file).ToLower();
                switch (ext)
                {
                    case ".cs":
                    case ".js":
                        IncrementFileType("script");
                        break;
                    case ".prefab":
                        IncrementFileType("prefab");
                        break;
                    case ".unity":
                        IncrementFileType("scene");
                        break;
                    case ".mat":
                        IncrementFileType("mat");
                        break;
                    case ".txt":
                    case ".json":
                    case ".doc":
                    case ".docx":
                    case ".csv":
                        IncrementFileType("txt");
                        break;
                    case ".playable":
                        IncrementFileType("timeline");
                        break;
                    case ".png":
                    case ".jpg":
                    case ".psd":
                    case ".psb":
                    case ".jpeg":
                    case ".tga":
                        IncrementFileType("sprite");
                        break;
                    case ".ttf":
                    case ".otf":
                        IncrementFileType("font");
                        break;
                    case ".shader":
                    case ".cginc":
                        IncrementFileType("shader");
                        break;
                    case ".renderTexture":
                        IncrementFileType("rt");
                        break;
                    case ".mp3":
                    case ".wav":
                    case ".ogg":
                        IncrementFileType("audio");
                        break;
                    case ".anim":
                    case ".controller":
                        IncrementFileType("anim");
                        break;
                    case ".asset":
                        IncrementFileType("so");
                        break;
                    default:
                        break;
                }
            }
            var dominantEntry = fileTypeCounts.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            return dominantEntry.Key ?? "other";
        }

        private static void IncrementFileType(string fileType)
        {
            if (!fileTypeCounts.ContainsKey(fileType))
            {
                fileTypeCounts[fileType] = 0;
            }
            fileTypeCounts[fileType]++;
        }

        [MenuItem("SKCell/Tools/Update Folder Icons Cache")]
        public static void UpdateFolderCache() 
        { 
            int count=0;
            folderTypeCache.Clear();
            foreach (var folder in Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories))
            {
                folderTypeCache[folder] = DetermineDominantFileType(folder,out count );
            }
        }
    }

}