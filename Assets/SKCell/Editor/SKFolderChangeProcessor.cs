using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SKCell
{
    public class SKFolderChangeProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
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
