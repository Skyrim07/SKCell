using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell.Test { 
    public class UIScene : MonoBehaviour
    {
        [SerializeField] SKTextAnimator[] typewriterTexts;
        [SerializeField] SKSlider[] sliders;
        public void ReplayTypewriter()
        {
            foreach (var typewriter in typewriterTexts)
            {
                typewriter.PlayTypeWriter();
            }
        }

        public void IncreaseSliderValue()
        {
            foreach(var slider in sliders)
            {
                slider.SetValue(slider.value + Time.deltaTime/5f);
            }
        }
    }
}
