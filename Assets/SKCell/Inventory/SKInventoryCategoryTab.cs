using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    public class SKInventoryCategoryTab : MonoBehaviour
    {
        SKInventoryLayer layer;
        public int categoryID { get; set; }

        [SerializeField] Animator selectorAnim;
        [SerializeField] SKText nameText;
        public void OnDeselect()
        {
            selectorAnim.Disappear();
        }

        public void OnSelect()
        {
            selectorAnim.Appear();
            if (layer)
                layer.OnSelectCategoryTab(this);
        }

        public void UpdateCategoryInfo(SKInventoryLayer layer, int categoryID)
        {
            this.categoryID = categoryID;
            this.layer = layer;
            if (categoryID < 0) return;

            if (SKInventory.Asset.categoryData.categoryLocalIDs[categoryID] >= 0)
            {
                nameText.UpdateLocalID(SKInventory.Asset.categoryData.categoryLocalIDs[categoryID]);
            }
            else
            {
                string name = SKInventory.Asset.categoryData.categoryNames[categoryID];
                nameText.UpdateTextDirectly(name.Length < 1 ? $"Category {categoryID + 1}" : name);
            }


        }
    }
}
