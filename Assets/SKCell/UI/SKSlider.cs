using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("SKCell/UI/SKSlider")]
    public class SKSlider : MonoBehaviour, IFadable
    {
        #region Fields
        [HideInInspector] public bool initialized = false;
        [Range(0f, 1f)]
        public float value = 1;
        [Range(0f, 1f)]
        public float initialValue = 0;
        
        [Header("General Settings")]
        [Tooltip("Press 'Generate structure' to apply any style changes.")]
        [SerializeField] SliderStyle style;
        [SerializeField] LinearSliderOrientation linearDirection = LinearSliderOrientation.LeftToRight;
        [SerializeField] CircularSliderOrientation circularPivot = CircularSliderOrientation.CircularFromTop;
        [SerializeField] CircularSliderFillMethod circularMethod = CircularSliderFillMethod.Radial360;
        [SerializeField] CircularSliderRotateDirection circularDirection = CircularSliderRotateDirection.Clockwise;

        [SerializeField] SKImage background, fill;

        [SerializeField] Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        [SerializeField] Color fillColor = new Color(0.8f, 0.8f, 0.8f);
        [Tooltip("Adjust the fill color based on slider value.")]
        public bool fillColorTransition = true;
        [Tooltip("If value<this, use default color; if value>= this, use color 2.")]
        public float fillColorTransitionThreshold = 0.9f;
        [SerializeField] Color fillColor2 = new Color(0.2f, 0.8f, 0.4f);
        public bool fillColorSmoothLerp = true;
        [Range(0.01f, 1f)]
        public float fillColorLerpSpeed = 0.1f;

        [Header("Progress Text")]
        public bool showProgressText = true;
        [SerializeField] SKText progressText;
        [SerializeField] ProgressTextType progressTextType = ProgressTextType.Percentage;
        [Tooltip("Float precision of the progress text.")]
        [Range(0, 5)]
        public int percentagePrecision = 2;
        [Tooltip("The total value if the progress text type is PartByWhole.")]
        public int totalValue = 10;
        [SerializeField] Color progressTextColor = new Color(0.9f, 0.9f, 0.9f);
        [Tooltip("Adjust the text color based on slider value.")]
        public bool textColorTransition = true;
        [Tooltip("If value<this, use default color; if value>= this, use color 2.")]
        public float textColorTransitionThreshold = 0.5f;
        [SerializeField] Color progressTextColor2 = new Color(0.1f, 0.1f, 0.1f);
        public bool textColorSmoothLerp = true;
        [Range(0.01f, 1f)]
        public float textColorLerpSpeed = 0.1f;

        [Tooltip("Smoothly transit to the next given value.")]
        [Header("Smooth Transition")]
        public bool enableSmoothTransition = true;
        [Range(0.01f, 1f)]
        public float lerpSpeed = 0.15f;
        public float threshold = 0.001f;

        [Header("Delayed Fill")]
        public bool enableDelayedFill = false;
        [SerializeField] SKImage delayedFill;
        [SerializeField] Color delayedFillColorUp = new Color(0.3f, 0.6f, 0.8f);
        [SerializeField] Color delayedFillColorDown = new Color(0.9f, 0.4f, 0.2f);
        [Range(0.01f, 1f)]
        public float delayedFillLerpSpeed = 0.2f;

        [Header("Events")]
        [SerializeField] UnityEvent onValueChanged;
        [SerializeField] UnityEvent onFull, onEmpty, onHalf, onStart;

        private GameObject slider, circularSlider;

        private CanvasGroup canvasGroup;

        private Coroutine textColorCR1, textColorCR2, fillColorCR1, fillColorCR2;
        #endregion

        #region Editor fields
        private float lastValue, valueDelta;
        private bool lerping;
        #endregion
        private void Start()
        {
            if (enableDelayedFill && delayedFillLerpSpeed <= lerpSpeed)
            {
                SKUtils.EditorLogWarning($"SKSlider: A latentLerpSpeed smaller than lerpSpeed is meaningless. Gameobject: {name}");
            }
            SetValueRaw(initialValue);

            canvasGroup = SKUtils.GetComponentNonAlloc<CanvasGroup>(gameObject);

            onStart.Invoke();
        }
        /// <summary>
        /// Set the value of this SKSlider
        /// </summary>
        /// <param name="value">Value 0-1</param>
        public void SetValue(float value)
        {
            if (!initialized)
            {
                SKUtils.EditorLogError("SKSlider not initialized!");
                return;
            }
            bool isUp = value > this.value;
            this.value = value;
            if (enableSmoothTransition)
            {
                StopAllCoroutines();
                if (enableDelayedFill)
                {
                    delayedFill.color = isUp ? delayedFillColorUp : delayedFillColorDown;
                    SKUtils.SetActiveEfficiently(delayedFill.gameObject, true);
                    StartCoroutine(LerpLatentFill(isUp ? delayedFillLerpSpeed : lerpSpeed, threshold));
                    StartCoroutine(LerpFill(isUp ? lerpSpeed : delayedFillLerpSpeed, threshold));
                }
                else
                {
                    SKUtils.SetActiveEfficiently(delayedFill.gameObject, false);
                    StartCoroutine(LerpFill(lerpSpeed, threshold));
                }
            }
            else
            {
                delayedFill.fillAmount=fill.fillAmount = value;
                if (fillColorTransition)
                    fill.color = value < fillColorTransitionThreshold ? fillColor : fillColor2;
                else
                    fill.color = fillColor;
                if (textColorTransition)
                    progressText.color = value < textColorTransitionThreshold ? progressTextColor : progressTextColor2;
                else
                    progressText.color = progressTextColor;
                SyncText(value);
            }
            onValueChanged.Invoke();
            if (value == 1f)
                onFull.Invoke();
            if (value == 0f)
                onEmpty.Invoke();
            if (value == 0.5f)
                onHalf.Invoke();
        }

        public void SetRandomValue()
        {
            SetValue(Random.Range(0f, 1f));
        }
        /// <summary>
        /// Set the numeric value only. No visual effects.
        /// </summary>
        /// <param name="value"></param>
        public void SetValueRaw(float value)
        {
            this.value = value;
            delayedFill.fillAmount = fill.fillAmount = value;
            SyncText(value);
            if (fillColorTransition)
                fill.color = value < fillColorTransitionThreshold ? fillColor : fillColor2;
            else
                fill.color = fillColor;
            if (textColorTransition)
                progressText.color = value < textColorTransitionThreshold ? progressTextColor : progressTextColor2;
            else
                progressText.color = progressTextColor;
        }
        private IEnumerator LerpFill(float lerpSpeed, float threshold)
        {
            while (Mathf.Abs(fill.fillAmount - value) > threshold)
            {
                fill.fillAmount = Mathf.Lerp(fill.fillAmount, value, lerpSpeed);
                if (showProgressText)
                {
                    if (progressTextType == ProgressTextType.Percentage)
                        progressText.UpdateTextDirectly(fill.fillAmount.ToString($"p{percentagePrecision}"));
                    else
                        progressText.UpdateTextDirectly($"{(fill.fillAmount * totalValue).ToString("f0")} / {totalValue}");
                    UpdateTextColor();

                }
                UpdateFillColor();
                yield return new WaitForFixedUpdate();
            }
            fill.fillAmount = value;
            if (showProgressText)
            {
                SyncText(value);
                UpdateTextColor();

            }
            UpdateFillColor();
        }
        private IEnumerator LerpLatentFill(float lerpSpeed, float threshold)
        {
            while (Mathf.Abs(delayedFill.fillAmount - value) > threshold)
            {
                delayedFill.fillAmount = Mathf.Lerp(delayedFill.fillAmount, value, lerpSpeed);
                yield return new WaitForFixedUpdate();
            }
            delayedFill.fillAmount = value;
        }
        private IEnumerator LerpProgressText(float lerpSpeed, float threshold, Color targetColor)
        {
            while (Mathf.Abs((progressText.color- targetColor).maxColorComponent) > threshold)
            {
                progressText.color = Color.Lerp(progressText.color, targetColor, lerpSpeed);
                yield return new WaitForFixedUpdate();
            }
            progressText.color = targetColor;
        }
        private IEnumerator LerpFillColor(float lerpSpeed, float threshold, Color targetColor)
        {
            while (Mathf.Abs((fill.color - targetColor).maxColorComponent) > threshold)
            {
                fill.color = Color.Lerp(fill.color, targetColor, lerpSpeed);
                yield return new WaitForFixedUpdate();
            }
            fill.color = targetColor;
        }
        private void UpdateTextColor()
        {
            if (textColorTransition)
            {
                if (textColorSmoothLerp)
                {
                    if (value < textColorTransitionThreshold && progressText.color.Distance(progressTextColor)>0.1f)
                    {
                        if (textColorCR1 != null)
                            StopCoroutine(textColorCR1);
                        textColorCR1= StartCoroutine(LerpProgressText(textColorLerpSpeed, 0.01f, progressTextColor));
                    }
                     if (value >= textColorTransitionThreshold && progressText.color.Distance(progressTextColor2) > 0.1f)
                    {
                        if (textColorCR2 != null)
                            StopCoroutine(textColorCR2);
                        textColorCR2 =StartCoroutine(LerpProgressText(textColorLerpSpeed, 0.01f, progressTextColor2));
                    }
                }
                else
                {
                    progressText.color = value <= textColorTransitionThreshold ? progressTextColor : progressTextColor2;
                }
            }
            else
            {
                progressText.color = progressTextColor;
            }
        }
        private void UpdateFillColor()
        {
            if (fillColorTransition)
            {
                if (fillColorSmoothLerp)
                {
                    if (value < fillColorTransitionThreshold && fill.color.Distance(fillColor)>0.1f)
                    {
                        if (fillColorCR1 != null)
                            StopCoroutine(fillColorCR1);
                        fillColorCR1 = StartCoroutine(LerpFillColor(fillColorLerpSpeed, 0.01f, fillColor));
                    }
                    if (value >= fillColorTransitionThreshold && fill.color.Distance(fillColor2) > 0.1f)
                    {
                        if (fillColorCR2 != null)
                            StopCoroutine(fillColorCR2);
                        fillColorCR2 = StartCoroutine(LerpFillColor(fillColorLerpSpeed, 0.01f, fillColor2));
                    }
                }
                else
                {
                    fill.color = value <= fillColorTransitionThreshold ? fillColor : fillColor2;
                }
            }
            else
            {
                fill.color = fillColor;
            }
        }
        private void SyncText(float value)
        {
            if (progressTextType == ProgressTextType.Percentage)
                progressText.UpdateTextDirectly(value.ToString($"p{percentagePrecision}"));
            else
                progressText.UpdateTextDirectly($"{(value * totalValue).ToString("f0")} / {totalValue}");
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
            float delta =0.2f;
            canvasGroup.alpha += delta;
            if(Mathf.Abs(canvasGroup.alpha - 1)<0.01f)
            {
                canvasGroup.alpha = 1;
                SKCore.FixedTick000 -= FadeInCR;
            }
        }
        void FadeOutCR()
        {
            float delta = -0.2f;
            canvasGroup.alpha += delta;
            if (Mathf.Abs(canvasGroup.alpha) < 0.01f)
            {
                canvasGroup.alpha = 0;
                SKCore.FixedTick000 -= FadeOutCR;
            }
        }
#if UNITY_EDITOR
        public void GenerateStructure()
        {
            initialized = true;
            ClearChildren(transform);
            string pathSuffix = string.Empty;
            if (style == SliderStyle.Linear)
            {
                pathSuffix = "/LinearSlider.prefab";
            }
            if (style == SliderStyle.Circular)
            {
                pathSuffix = "/CircularSlider.prefab";
            }
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKSlider Resource Error: Slider prefab lost.");
                initialized = false;
                return;
            }
            slider = Instantiate(prefab);
            slider.name = "SKSubSlider";
            name = $"SKSlider-{GetHashCode()}";
            transform.AttachChild(slider.transform, true);
            background = slider.transform.Find("Background").GetComponent<SKImage>();
            progressText = slider.transform.Find("ProgressText").GetComponent<SKText>();
            fill = slider.transform.Find("Fill Area/Fill").GetComponent<SKImage>();
            delayedFill = slider.transform.Find("Fill Area/LatentFill").GetComponent<SKImage>();
        }

        public void ClearChildren(Transform tf)
        {
            for (int i = tf.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(tf.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Draw previews in the editor, executes every frame in OnInspectorGUI
        /// </summary>
        public void DrawEditorPreview()
        {
            if (background != null)
            {
                background.color = backgroundColor;
            }
            if (fill != null)
            {
                UpdateFillColor();
                fill.fillAmount = value;
                if (showProgressText)
                {
                    SKUtils.SetActiveEfficiently(progressText.gameObject, true);
                    if (progressTextType == ProgressTextType.Percentage)
                        progressText.UpdateTextDirectly(value.ToString($"p{percentagePrecision}"));
                    else
                        progressText.UpdateTextDirectly($"{(value * totalValue).ToString("f0")} / {totalValue}");
                    UpdateTextColor();
                }
                else
                {
                    SKUtils.SetActiveEfficiently(progressText.gameObject, false);
                }
            }
            if (style == SliderStyle.Linear)
            {
                if (linearDirection == LinearSliderOrientation.LeftToRight || linearDirection == LinearSliderOrientation.RightToLeft)
                {
                    fill.fillMethod = Image.FillMethod.Horizontal;
                    fill.fillOrigin = (int)linearDirection;
                    delayedFill.fillMethod = Image.FillMethod.Horizontal;
                    delayedFill.fillOrigin = (int)linearDirection;
                }
                else
                {
                    fill.fillMethod = Image.FillMethod.Vertical;
                    fill.fillOrigin = (int)linearDirection - 2;
                    delayedFill.fillMethod = Image.FillMethod.Vertical;
                    delayedFill.fillOrigin = (int)linearDirection - 2;
                }
            }
            if (style == SliderStyle.Circular)
            {
                fill.fillMethod = (Image.FillMethod)circularMethod;
                fill.fillOrigin = (int)circularPivot;
                fill.fillClockwise = circularDirection == 0;
                delayedFill.fillMethod = (Image.FillMethod)circularMethod;
                delayedFill.fillOrigin = (int)circularPivot;
                delayedFill.fillClockwise = circularDirection == 0;
            }
        }


#endif
        public enum SliderStyle
        {
            Linear,
            Circular
        }
        public enum ProgressTextType
        {
            Percentage,
            PartByWhole
        }
        public enum LinearSliderOrientation
        {
            LeftToRight = 0,
            RightToLeft = 1,
            BottomToTop = 2,
            TopToBottom = 3,
        }
        public enum CircularSliderOrientation
        {
            CircularFromTop = 2,
            CircularFromBottom = 0,
            CircularFromRight = 1,
            CircularFromLeft = 3,
        }
        public enum CircularSliderFillMethod
        {
            Radial360 = 4,
            Radial180 = 3,
            Radial90 = 2
        }
        public enum CircularSliderRotateDirection
        {
            Clockwise = 0,
            CounterClockwise = 1
        }
    }

}
