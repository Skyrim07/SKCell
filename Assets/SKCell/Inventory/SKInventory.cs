using System.Collections.Generic;

namespace SKCell
{
    /// <summary>
    /// An inventory object that holds items.
    /// </summary>
    public class SKInventory
    { 
        private static SKInventoryAsset asset;
        public static SKInventoryAsset Asset
        {
            get
            {
                if (asset == null)
                    asset = SKAssetLibrary.InventoryAsset;
                return asset;
            }
        }
        //private static bool initialized = false;
        private static Dictionary<int, SKInventoryItemData> itemDataDict = new Dictionary<int, SKInventoryItemData>();

        /// <summary>
        /// Currently acquired items
        /// </summary>
        public Dictionary<int, SKInventoryItem> itemDict = new Dictionary<int, SKInventoryItem>();

        public static void Initialize()
        {
            BuildItemDataDict();
        }
        private static void BuildItemDataDict()
        {
            for (int i = 0; i < Asset.itemData.Count; i++)
            {
                SKInventoryItemData data = Asset.itemData[i];
                if (itemDataDict.ContainsKey(data.id))
                {
                    SKUtils.EditorLogWarning($"SKInventory: Duplicate item ID at id = {data.id}");
                    continue;
                }
                itemDataDict.Add(data.id, data);
            }
        }


        #region Static Methods
        /// <summary>
        /// Get item data by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SKInventoryItemData GetItemData(int id)
        {
            if (itemDataDict.ContainsKey(id))
                return itemDataDict[id];
            return null;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Get an item from this inventory.
        /// </summary>
        /// <param name="id"></param>
        public SKInventoryItem GetItem(int id)
        {
            if (id <= 0)
            {
                SKUtils.EditorLogWarning($"SKInventory.GetItem: Item id must be positive. (item id = {id})");
                return null;
            }
            if (!itemDict.ContainsKey(id))
            {
                SKUtils.EditorLogWarning($"SKInventory.GetItem: Item not present. (item id = {id})");
                return null;
            }
            return itemDict[id];
        }

        /// <summary>
        /// Remove all items from this inventory.
        /// </summary>
        public void Clear()
        {
            itemDict.Clear();
        }

        /// <summary>
        /// Add an item to this inventory. (id as specified in the SK Inventory Center)
        /// </summary>
        /// <param name="id">Item id as specified in the SK Inventory Center.</param>
        /// <param name="count">Item count.</param>
        /// <param name="stacking">If enabled, items with the same id will stack.</param>
        public void AddItem(int id, int count, bool stacking = true)
        {
            if (id <= 0 || count <= 0)
            {
                SKUtils.EditorLogWarning($"SKInventory.RemoveItem: Item id and count must be positive. (item id = {id}, count = {count})");
                return;
            }
            if (stacking && itemDict.ContainsKey(id))
            {
                itemDict[id].count += count;
            }
            else
            {
                itemDict[id] = new SKInventoryItem()
                {
                    id = id,
                    count = count,
                };
            }
        }


        /// <summary>
        /// Remove a certain count of an item from this inventory. (id as specified in the SK Inventory Center)
        /// </summary>
        /// <param name="id">Item id as specified in the SK Inventory Center.</param>
        /// <param name="count">Item count to be removed.</param>
        public void RemoveItem(int id, int count)
        {
            if (id <= 0 || count <= 0)
            {
                SKUtils.EditorLogWarning($"SKInventory.RemoveItem: Item id and count must be positive. (item id = {id}, count = {count})");
                return;
            }

            if (itemDict.ContainsKey(id))
            {
                if (itemDict[id].count > count)
                {
                    itemDict[id].count -= count;
                }
                else if (itemDict[id].count == count)
                {
                    itemDict.Remove(id);
                }
                else
                {
                    SKUtils.EditorLogWarning($"SKInventory.RemoveItem: Trying to remove more items than available. (item id = {id}, available count = {itemDict[id].count}, requested count = {count})");
                }
            }
            else
            {
                SKUtils.EditorLogWarning($"SKInventory.RemoveItem: Item not found in inventory. (item id = {id})");
            }
        }

        /// <summary>
        /// Remove an item from this inventory, regardless of its count.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveItemDirectly(int id)
        {
            if (id <= 0 )
            {
                SKUtils.EditorLogWarning($"SKInventory.RemoveItemDirectly: Item id must be positive. (item id = {id})");
                return;
            }
            if (!itemDict.ContainsKey(id))
            {
                SKUtils.EditorLogWarning($"SKInventory.RemoveItemDirectly: Item not present. (item id = {id})");
                return;
            }

            itemDict.Remove(id);
        }

        /// <summary>
        /// Does this inventory contain the given item?
        /// </summary>
        /// <param name="id"></param>
        public bool ContainsItem(int id)
        {
            return itemDict.ContainsKey(id);
        }

        #endregion
    }
}