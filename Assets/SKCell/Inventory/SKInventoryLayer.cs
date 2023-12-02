using UnityEditor;
using UnityEngine;

namespace SKCell
{
    public class SKInventoryLayer : MonoBehaviour
    {
        [SKInspectorText("Please click on < Generate Structure > \nto start using this SKInventoryLayer for the first time.")]
        [SerializeField] int descrip_text;

        private SKInventory inventory;
        /// <summary>
        /// SKInventory object that this layer is displaying.
        /// </summary>
        public SKInventory Inventory
        {
            get
            {
                if (inventory == null) inventory = new SKInventory();
                return inventory;
            }
            set
            {
                inventory = value;
            }
        }

        [SKFolder("References")]
        [SerializeField] SKUIPanel panel;
        [SerializeField] SKInventoryDetailsPanel detailsPanel;
        [SerializeField] Transform itemFrameContainer, categoryTabContainer;
        [SerializeField] SKInventoryCategoryTab allCategoryTab;

        [SKFolder("Prefabs")]
        [SerializeField] GameObject itemFrameTemplate;
        [SerializeField] GameObject categoryTabTemplate;

        public ISKInventoryItemFrame selectedItemFrame;
        [HideInInspector]
        public SKInventoryCategoryTab selectedCategoryTab;
        [HideInInspector]
        public int selectedCategory = -1; // -1: ALL

        private void Awake()
        {
            UpdateDetailsPanel(-1); //close the details panel on awake
        }


        public void OpenInventoryPanel()
        {
            panel.SetState(true);
            LoadCategoryTabs();
            allCategoryTab.UpdateCategoryInfo(this, -1);
            allCategoryTab.OnSelect(); //display all items
        }

        public void CloseInventoryPanel()
        {
            panel.SetState(false);
        }
        public void ToggleInventoryPanel()
        {
            if (panel.active)
                CloseInventoryPanel();
            else
                OpenInventoryPanel();
        }

        public void OnSelectItemFrame(ISKInventoryItemFrame itemFrame)
        {
            if (selectedItemFrame != null && selectedItemFrame != itemFrame)
                selectedItemFrame.OnDeselect();
            selectedItemFrame = itemFrame;

            UpdateDetailsPanel(selectedItemFrame.ItemID);
        }
        public void OnSelectCategoryTab(SKInventoryCategoryTab tab)
        {
            if (selectedCategoryTab != null && selectedCategoryTab != tab)
                selectedCategoryTab.OnDeselect();
            selectedCategoryTab = tab;

            LoadItemFrames(tab.categoryID);
        }

        public void UpdateDetailsPanel(int itemID)
        {
            detailsPanel.UpdateItemInfo(this, itemID);
        }

        private void LoadItemFrames(int category)
        {
            selectedCategory = category;
            itemFrameContainer.ClearChildren();

            bool isFirst = true;
            foreach (var item in inventory.itemDict.Values)
            {
                if (selectedCategory != -1 && SKInventory.GetItemData(item.id).category != selectedCategory) continue;

                ISKInventoryItemFrame itemFrame = Instantiate(itemFrameTemplate, itemFrameContainer).GetComponent<ISKInventoryItemFrame>();
                itemFrame.UpdateItemInfo(this, item.id);
                if (isFirst)
                {
                    itemFrame.OnSelect();
                    isFirst = false;
                }
            }
            if (isFirst)
                UpdateDetailsPanel(-1);
        }
        private void LoadCategoryTabs()
        {
            categoryTabContainer.ClearChildren();

            for (int i = 0; i < SKInventory.Asset.categoryData.categoryNames.Length; i++)
            {
                if (!SKInventory.Asset.categoryData.categoryIsActive[i]) continue;
                SKInventoryCategoryTab tab = Instantiate(categoryTabTemplate, categoryTabContainer).GetComponent<SKInventoryCategoryTab>();
                tab.categoryID = i;
                tab.UpdateCategoryInfo(this, i);
            }
        }

#if UNITY_EDITOR
        [SKInspectorButton("Open Inventory Center")]
        public void OpenInventoryCenter()
        {
            SKInventoryControlCenter.Initialize();
        }

        [SKInspectorButton("Generate Structure")]
        public void GenerateStructure()
        {
            string pathSuffix = "/Inventory.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKInventory Resource Error: prefab lost.");
                return;
            }
            GameObject inv = Instantiate(prefab);
            inv.name = $"SKInventory - {inv.GetHashCode()}";
            inv.transform.SetParent(transform.parent);
            inv.transform.CopyFrom(transform);
            inv.transform.SetSiblingIndex(transform.GetSiblingIndex());
            Selection.activeGameObject = inv;
            DestroyImmediate(this.gameObject);
        }
#endif
    }
}