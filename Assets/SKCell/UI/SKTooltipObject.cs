using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    public class SKTooltipObject : MonoBehaviour
    {
        [SerializeField] Image bgComp;
        [SerializeField] SKText textComp;
        [SerializeField] SKUIAnimation anim;
        [SerializeField] Transform content;
        public void SetText(string text)
        {
            textComp.UpdateTextDirectly(text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(textComp.rectTransform);
        }

        public void SetProperties(Color bg, Color text, float fontSize)
        {
            bgComp.color = bg;
            textComp.color = text;
            textComp.fontSize = fontSize;
        }

        public void SetState(bool appear)
        {
            anim.SetState(appear);
        }

        public void SetPosition(Vector3 position)
        {
            content.position = position;
        }
    }
}