using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    public class SKInventoryItemFrame : MonoBehaviour, ISKInventoryItemFrame
    {
        SKInventoryLayer layer;
        public int ItemID { get; set; }

        [SerializeField] Animator selectorAnim;
        [SerializeField] RawImage iconImage;
        [SerializeField] SKText countText;
        public void OnDeselect()
        {
            if(selectorAnim)
                selectorAnim.Disappear();
        }

        public void OnSelect()
        {
            if (selectorAnim)
                selectorAnim.Appear();
            if(layer)
                layer.OnSelectItemFrame(this);
        }

        public void UpdateItemInfo(SKInventoryLayer layer, int itemid)
        {
            ItemID = itemid;
            SKInventoryItemData item = SKInventory.GetItemData(itemid);
            iconImage.texture =  item.icon;
            countText.UpdateTextDirectly(layer.Inventory.GetItem(itemid).count.ToString());
            this.layer = layer;
        }
    }
}
