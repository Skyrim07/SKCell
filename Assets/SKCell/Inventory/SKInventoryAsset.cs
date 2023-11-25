using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    #region Assets and helper classes

    [CreateAssetMenu(fileName = "SKInventoryAsset", menuName = "SKCell/Inventory/SKInventoryAsset", order = 0)]
    [Serializable]
    public class SKInventoryAsset : ScriptableObject
    {
        public List<SKInventoryItemData> itemData = new List<SKInventoryItemData>();
        public SKInventoryCategoryData categoryData = new SKInventoryCategoryData();
        public SKInventoryAsset() { }
        public SKInventoryAsset(SKInventoryAssetJson data)
        {
            if (data == null) return;
            this.itemData = new List<SKInventoryItemData>(data.itemData);
            this.categoryData = data.categoryData;
        }
        public void Initialize()
        {
            itemData = new List<SKInventoryItemData>();
            categoryData = new SKInventoryCategoryData();
        }
        public void UpdateInfo(SKInventoryAssetJson data)
        {
            if (data == null) return;
            this.itemData = new List<SKInventoryItemData>(data.itemData);
            this.categoryData = data.categoryData;
        }
    }

    
}
#endregion

