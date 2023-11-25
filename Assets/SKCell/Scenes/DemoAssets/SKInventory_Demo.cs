using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell.Test
{
    public class SKInventory_Demo : MonoBehaviour
    {
        public SKInventoryLayer inventoryLayer; //This is the component that displays inventory data as a UI panel.
        void Start()
        {
            SKInventory inventory = inventoryLayer.Inventory; //Get the inventory data object of the layer.
            inventory.AddItem(1000, 1); //Add item (id = 1000, count = 1)
            inventory.AddItem(1001, 5);//Add item (id = 1001, count = 5)

            SKInventory.GetItemData(1000).canUse = true; //Set item 1000 to be usable.
            SKInventory.GetItemData(1000).onUse += () => { print("Use Item 1000"); }; // Add on use actions for item 1000

            inventoryLayer.OpenInventoryPanel(); //Open the inventory UI panel.
        }

    }
}
