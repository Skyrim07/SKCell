using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class SKFader : MonoBehaviour,IFadable
    {
        [Range(0.005f,1f)]
        public float alphaDelta = 0.02f;
        [Range(0f, 1f)]
        public float initialAlpha = 1f;
        private CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = initialAlpha;
        }
        public void FadeIn()
        {
            SKCore.FixedTick000 += FadeInCR;
            SKCore.FixedTick000 -= FadeOutCR;
        }

        public void FadeOut()
        {
            SKCore.FixedTick000 += FadeOutCR;
            SKCore.FixedTick000 -= FadeInCR;
        }

        void FadeInCR()
        {
            if (!canvasGroup)
                return;
            canvasGroup.alpha += alphaDelta;
            if (Mathf.Abs(canvasGroup.alpha - 1) < 0.01f)
            {
                canvasGroup.alpha = 1;
                SKCore.FixedTick000 -= FadeInCR;
            }
        }
        void FadeOutCR()
        {
            if (!canvasGroup)
                return;
            canvasGroup.alpha -= alphaDelta;
            if (Mathf.Abs(canvasGroup.alpha) < 0.01f)
            {
                canvasGroup.alpha = 0;
                SKCore.FixedTick000 -= FadeOutCR;
            }
        }
    }
}