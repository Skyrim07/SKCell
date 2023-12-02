using UnityEngine;
using UnityEngine.EventSystems;

namespace SKCell
{
    [AddComponentMenu("SKCell/UI/SKTooltip")]
    public class SKTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string tooltipText;
        public Vector2 mouseOffset = new Vector2(150, 50);
        public Color backgroundColor = new Color(1, 1, 1, .9f);
        public Color textColor = new Color(.1f,.1f,.1f);
        public float fontSize = 36;

        private bool isActive;

        static SKTooltipObject tooltipObject;
        void Start()
        {
            if (tooltipObject == null)
            {
                GenerateTooltipObject();
            }
        }

        void FixedUpdate()
        {
            if (!isActive) return;
            tooltipObject.SetPosition((Vector3)mouseOffset+Input.mousePosition);
        }
        public void GenerateTooltipObject()
        {
            string pathSuffix = "/Tooltip";
            GameObject prefab = Resources.Load<GameObject>(SKAssetLibrary.RESOURCES_PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKTooltip Resource Error: prefab lost.");
                return;
            }
            GameObject tip = Instantiate(prefab);
            tip.name = $"SKTooltip";
            tooltipObject = tip.GetComponent< SKTooltipObject>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isActive = true;
            tooltipObject.SetProperties(backgroundColor, textColor, fontSize);
            tooltipObject.SetText(tooltipText);
            tooltipObject.SetState(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isActive = false;
            tooltipObject.SetState(false);
        }
    }
}