using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public interface ISKInventoryItemFrame
    {
        public int ItemID { get; set; }
        /// <summary>
        /// Called when this frame is selected.
        /// </summary>
        public void OnSelect();

        /// <summary>
        /// Called when this frame loses selection.
        /// </summary>
        public void OnDeselect();

        /// <summary>
        /// Set this frame to display item <itemid> in SKInventoryLayer <layer>.
        /// </summary>
        /// <param name="itemid"></param>
        public void UpdateItemInfo(SKInventoryLayer layer, int itemid);
    }
}
