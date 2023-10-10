using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SKCell
{
    [InitializeOnLoad]
    public class SKFolderIconEditor
    {
        private static Dictionary<string, string> folderTypeCache = new Dictionary<string, string>();
        private static Dictionary<string, int> fileTypeCounts = new Dictionary<string, int>();

        static SKFolderIconEditor()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
            UpdateFolderCache();
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path))
                return;

            if (!folderTypeCache.TryGetValue(path, out string dominantType))
            {
                dominantType = DetermineDominantFileType(path);
                folderTypeCache[path] = dominantType;
            }

            Texture iconTexture = dominantType == "other" ? null : SKAssetLibrary.LoadTexture($"FolderIcons/{dominantType}");
            if (iconTexture)
            {
                float size = selectionRect.width / 2.5f;
                Rect iconRect = (selectionRect.width < 100)
                    ? new Rect(selectionRect.xMax - selectionRect.width / 2.5f, selectionRect.y + selectionRect.width / 1.6f, size, size)
                    : new Rect(selectionRect.xMax - 20, selectionRect.y, 15, 15);

                GUI.DrawTexture(iconRect, iconTexture);
            }
        }

        private static string DetermineDominantFileType(string folderPath)
        {
            fileTypeCounts.Clear();
            int count = 0;
            foreach (var file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                count++;
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
            // int halfCount = count / 4 - 1;
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
            folderTypeCache.Clear();
            foreach (var folder in Directory.GetDirectories("Assets", "*", SearchOption.AllDirectories))
            {
                folderTypeCache[folder] = DetermineDominantFileType(folder);
            }
        }
    }

}