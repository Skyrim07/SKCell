using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SKCell
{
    public class SKFolderChangeProcessor : AssetPostprocessor
    {
        private static double lastChangeTime = 0;
        private static readonly double debounceTime = 0.5; 

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (EditorApplication.timeSinceStartup - lastChangeTime < debounceTime)
                return;

            lastChangeTime = EditorApplication.timeSinceStartup;

            var allChangedAssets = importedAssets.Concat(deletedAssets).Concat(movedAssets);
            HashSet<string> changedDirectories = new HashSet<string>();

            foreach (string asset in allChangedAssets)
            {
                changedDirectories.Add(Path.GetDirectoryName(asset));
            }

            SKFolderIconEditor.UpdateFolderCache();
            EditorApplication.RepaintProjectWindow();
        }



    }
}
