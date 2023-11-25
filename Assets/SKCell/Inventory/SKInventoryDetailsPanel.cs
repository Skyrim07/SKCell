using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SKCell {
    public class SKInventoryDetailsPanel : MonoBehaviour
    {
        SKInventoryLayer layer;

        [SerializeField] GameObject content;
        [SerializeField] RawImage iconImage;
        [SerializeField] SKText nameText, descriptionText;
        [SerializeField] Button useButton;


        /// <summary>
        /// Set this details panel to display item <itemid> in SKInventoryLayer <layer>. 
        /// Override this method to customize.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="itemid"></param>
        public virtual void UpdateItemInfo(SKInventoryLayer layer, int itemid)
        {
            content.SetActive(itemid > 0);
            if (itemid <= 0) return;
            this.layer = layer;
            SKInventoryItemData item = SKInventory.GetItemData(itemid);
            iconImage.texture = item.icon;

            if(item.name_LocalID>=0)
                nameText.UpdateLocalID(item.name_LocalID);
            else
                nameText.UpdateTextDirectly(item.name);

            if (item.descrip_LocalID >= 0)
                descriptionText.UpdateLocalID(item.descrip_LocalID);
            else
                descriptionText.UpdateTextDirectly(item.description);

            useButton.gameObject.SetActive(item.canUse);
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => item.onUse());
        }
    }
}
