using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    /// <summary>
    /// A slider using image translation instead of image fill.
    /// </summary>
    [AddComponentMenu("SKCell/UI/SKTranslationalSlider")]
    public class SKTranslationalSlider : MonoBehaviour
    {
        [SerializeField]
        Transform sliderContent;
        [SerializeField]
        Transform startPos, endPos;
        [SerializeField]
        SKText progressText;
        public float value = 0;
        private void Start()
        {
            SetValue(value);
        }
        public void SetValue(float value)
        {
            this.value = Mathf.Clamp01(value);
            sliderContent.position = startPos.position + (endPos.position - startPos.position) * value;
            if(progressText)
            progressText.text = value.ToString("p2");
        }
        public void SetRandomValue()
        {
            SetValue(Random.Range(0f, 1f));
        }
    }
}
