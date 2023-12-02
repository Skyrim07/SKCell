using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SKCell {
    public class SKUIPanelEditor
    {
        [MenuItem("Tools/SKCell/UI/BuildPanelHierarchy")]
        public static void BuildPanelHierarchy()
        {
            Transform rootPanel = (new GameObject()).transform;
            rootPanel.name = "UIRoot";

            for (int i = 1; i <= 7; i++)
            {
                Transform panel =(new GameObject()).AddComponent(typeof(Canvas)).transform;
                panel.gameObject.AddComponent(typeof(CanvasScaler));
                panel.gameObject.AddComponent(typeof(GraphicRaycaster));
                panel.name = SKUIPanelManager.panelHierarcyName[i];
                Canvas canvas = panel.GetComponent<Canvas>();
                canvas.sortingOrder = SKUIPanelManager.panelHierarcyValue[i];
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                panel.SetParent(rootPanel);
            }
        }
    }
}
