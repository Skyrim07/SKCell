using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace SKCell
{
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("SKCell/UI/SKToggle")]
    public class SKToggle : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IFadable
    {
        #region Fields
        [SerializeField] bool interactable = true;
        public bool Interactable { get { return interactable; } set { SetInteractability(value); } }
        public bool isOn = true;
        public bool canBeToggledOff = true;

        [SerializeField] SKImage background, selector;
        [SerializeField] SKText text;

        [Header("Toggle Group")]
        public SKToggleGroup toggleGroup;

        [Header("Transition")]
        public SKToggleTransitionMode transitionMode = SKToggleTransitionMode.BackgroundAndSelector;
        public float transitionTime = 0.1f;
        [SerializeField] Color backgroundToggledOnColor = Color.white;
        [SerializeField] Color backgroundToggledOffColor = new Color(0.9f, 0.3f, 0.4f);
        [SerializeField] Color backgroundDisabledColor = Color.gray;
        [SerializeField] Color backgroundPressedColor = Color.gray;
        [SerializeField] float backgroundMouseOverTint = -0.1f;

        [SerializeField] Color selectorToggledOnColor = new Color(0.2f, 0.2f, 0.2f);
        [SerializeField] Color selectorDisabledOnColor = new Color(0.4f, 0.4f, 0.4f);

        public bool enableTextTransition = false;
        [SerializeField] Color textToggledOnColor = Color.black;
        [SerializeField] Color textToggledOffColor = Color.white;
        [SerializeField] Color textDisabledColor = new Color(0.6f, 0.6f, 0.6f);

        [Header("Events")]
        [SerializeField] ToggleEvent onToggled;
        [SerializeField] UnityEvent onToggledOn,onToggledOff;
        [SerializeField] UnityEvent onPress, onPointerEnter, onPointerExit, onPointerUp, onPointerDown, onStart;
        #endregion
        #region Private Fields
        [HideInInspector] public bool initialized = false;
        [HideInInspector] public bool hasAnimator = false;
        private Animator anim;
        private CanvasGroup canvasGroup;
        #endregion
        private void Start()
        {
            if (!initialized)
            {
                SKUtils.EditorLogError("SKToggle not initialized!");
                return;
            }

            if (toggleGroup != null)
                toggleGroup.AddMember(this);

            if (background != null && selector != null && text != null)
            {
                if (transitionMode != SKToggleTransitionMode.Animation)
                {
                    if (transitionMode != SKToggleTransitionMode.BackgroundOnly)
                        selector.color = isOn ? (interactable ? selectorToggledOnColor : selectorDisabledOnColor) : Color.clear;
                    if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                        background.color = interactable ? (isOn ? backgroundToggledOnColor : backgroundToggledOffColor) : backgroundDisabledColor;
                    if (enableTextTransition)
                        text.color = interactable ? (isOn ? textToggledOnColor : textToggledOffColor) : textDisabledColor;
                }
            }

            if (transitionMode == SKToggleTransitionMode.Animation)
            {
                anim = SKUtils.GetComponentNonAlloc<Animator>(gameObject);
                anim.SetBool("isOn", isOn);
                if (!interactable)
                    anim.SetTrigger("Disabled");
            }

            canvasGroup = SKUtils.GetComponentNonAlloc<CanvasGroup>(gameObject);

            onStart.Invoke();
        }
        #region Editor
#if UNITY_EDITOR
        public void GenerateStructure()
        {
            string pathSuffix = "/CommonToggle.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKToggle Resource Error: Toggle prefab lost.");
                initialized = false;
                return;
            }
            GameObject toggle = Instantiate(prefab);
            toggle.name = $"SKToggle-{GetHashCode()}";
            toggle.transform.SetParent(transform.parent);
            toggle.transform.CopyFrom(transform);
            toggle.transform.SetSiblingIndex(transform.GetSiblingIndex());
            toggle.GetComponent<SKToggle>().initialized = true;
            Selection.activeGameObject = toggle;
            DestroyImmediate(this.gameObject);
        }
        public void DrawEditorPreview()
        {
            if (background != null && selector != null && text != null)
            {
                if (transitionMode != SKToggleTransitionMode.Animation)
                {
                    if (transitionMode != SKToggleTransitionMode.BackgroundOnly)
                        selector.color = isOn ? (interactable ? selectorToggledOnColor : selectorDisabledOnColor) : Color.clear;
                    if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                        background.color = interactable ? (isOn ? backgroundToggledOnColor : backgroundToggledOffColor) : backgroundDisabledColor;
                    if (enableTextTransition)
                        text.color = interactable ? (isOn ? textToggledOnColor : textToggledOffColor) : textDisabledColor;
                }
            }
        }
        public void AttachController(AnimatorController controller)
        {
            Animator anim = gameObject.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller as RuntimeAnimatorController;
            this.anim = SKUtils.GetComponentNonAlloc<Animator>(gameObject);
            hasAnimator = true;
        }
        public void DetachController()
        {
            if (GetComponent<Animator>() != null)
            {
                DestroyImmediate(GetComponent<Animator>());
                this.anim = null;
                hasAnimator = false;
            }
        }
#endif
        #endregion
        #region Transition
        private void TransitOn()
        {
            if (transitionMode != SKToggleTransitionMode.Animation)
            {
                StopAllCoroutines();
                if (transitionMode != SKToggleTransitionMode.BackgroundOnly)
                    StartCoroutine(TransitImageCR(selector, selectorToggledOnColor));
                if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                    StartCoroutine(TransitImageCR(background, backgroundToggledOnColor));
                if (enableTextTransition)
                    StartCoroutine(TransitTextCR(text, textToggledOnColor));
            }
            else
            {
                anim.SetTrigger("anyToOn");
                anim.SetBool("isOn", true);
            }
        }
        private void TransitOff()
        {
            if (transitionMode != SKToggleTransitionMode.Animation)
            {
                StopAllCoroutines();
                if (transitionMode != SKToggleTransitionMode.BackgroundOnly)
                    StartCoroutine(TransitImageCR(selector, Color.clear));
                if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                    StartCoroutine(TransitImageCR(background, backgroundToggledOffColor));
                if (enableTextTransition)
                    StartCoroutine(TransitTextCR(text, textToggledOffColor));
            }
            else
            {
                anim.SetTrigger("anyToOff");
                anim.SetBool("isOn", false);
            }
        }
        private void TransitDisabled()
        {
            if (transitionMode != SKToggleTransitionMode.Animation)
            {
                StopAllCoroutines();
                if (transitionMode != SKToggleTransitionMode.BackgroundOnly)
                {
                    if (isOn)
                        StartCoroutine(TransitImageCR(selector, selectorDisabledOnColor));
                    else
                        StartCoroutine(TransitImageCR(selector, Color.clear));
                }
                if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                    StartCoroutine(TransitImageCR(background, backgroundDisabledColor));
                if (enableTextTransition)
                    StartCoroutine(TransitTextCR(text, textDisabledColor));
            }
            else
            {
                anim.SetTrigger("Disabled");
            }
        }
        private void TransitPress()
        {
            if (transitionMode != SKToggleTransitionMode.Animation)
            {
                if (toggleGroup != null)
                {
                    if (toggleGroup.mode == SKToggleGroup.SKToggleGroupMode.ActiveOneOnly)
                    {
                        if (isOn)
                            return;
                    }
                }
                StopAllCoroutines();
                if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                    StartCoroutine(TransitImageCR(background, backgroundPressedColor));
            }
            else
            {
                anim.SetTrigger("Pressed");
            }
        }
        private void TransitOver()
        {
            if (transitionMode != SKToggleTransitionMode.Animation)
            {
                StopAllCoroutines();
                if (transitionMode != SKToggleTransitionMode.SelectorOnly)
                    StartCoroutine(TransitImageCR(background, new Color(background.color.r - backgroundMouseOverTint, background.color.g - backgroundMouseOverTint, background.color.b - backgroundMouseOverTint)));
            }
            else
            {
                if(isOn)
                anim.SetTrigger("OverOn");
                else
                    anim.SetTrigger("OverOff");
            }
        }

        private IEnumerator TransitImageCR(SKImage image, Color targetColor)
        {
            float divider = 1f / transitionTime * Time.fixedDeltaTime;
            float deltaR = (targetColor.r - image.color.r) * divider;
            float deltaG = (targetColor.g - image.color.g) * divider;
            float deltaB = (targetColor.b - image.color.b) * divider;
            float deltaA = (targetColor.a - image.color.a) * divider;
            float stepCount = transitionTime / Time.fixedDeltaTime;
            for (int i = 0; i < stepCount; i++)
            {
                image.color = new Color(image.color.r + deltaR, image.color.g + deltaG, image.color.b + deltaB, image.color.a + deltaA);
                yield return new WaitForFixedUpdate();
            }
            image.color = targetColor;
            yield return null;
        }
        private IEnumerator TransitTextCR(SKText text, Color targetColor)
        {
            float divider = 1f / transitionTime * Time.fixedDeltaTime;
            float deltaR = (targetColor.r - text.color.r) * divider;
            float deltaG = (targetColor.g - text.color.g) * divider;
            float deltaB = (targetColor.b - text.color.b) * divider;
            float deltaA = (targetColor.a - text.color.a) * divider;
            float stepCount = transitionTime / Time.fixedDeltaTime;
            for (int i = 0; i < stepCount; i++)
            {
                text.color = new Color(text.color.r + deltaR, text.color.g + deltaG, text.color.b + deltaB, text.color.a + deltaA);
                yield return new WaitForFixedUpdate();
            }
            text.color = targetColor;
            yield return null;
        }

        #endregion
        #region Events

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
                return;
            onPress.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable)
                return;
            onPointerEnter.Invoke();
            TransitOver();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable)
                return;
            onPointerExit.Invoke();
            if (isOn)
            {
                TransitOn();
            }
            else
            {
                TransitOff();
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;
            onPointerDown.Invoke();
            TransitPress();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
                return;
            onPointerUp.Invoke();
        }
        private void OnInteractabilityChanged()
        {
            if (interactable)
            {
                if (isOn)
                    TransitOn();
                else
                    TransitOff();
            }
            else
                TransitDisabled();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Toggle the value of this SKToggle.
        /// </summary>
        public void Toggle()
        {
            if (isOn && !canBeToggledOff)
                return;
            isOn = !isOn;
            onToggled.Invoke(isOn);
            if (toggleGroup != null)
            {
                toggleGroup.OnMemberStateChanged(this);
            }
            if (isOn)
            {
                TransitOn();
                onToggledOn.Invoke();
            }
            else if(canBeToggledOff)
            {
                TransitOff();
                onToggledOff.Invoke();
            }
        }
        public void ToggleOn()
        {
            if (!isOn)
            {
                TransitOn();
                onToggledOn.Invoke();
            }
            else
                return;
            isOn = true;
            if (toggleGroup != null)
            {
                toggleGroup.OnMemberStateChanged(this);
            }
        }
        public void ToggleOff()
        {
            if (isOn)
            {
                TransitOff();
                onToggledOff.Invoke();
            }
            else
                return;
            isOn = false;
            if (toggleGroup != null)
            {
                toggleGroup.OnMemberStateChanged(this);
            }
        }
        /// <summary>
        /// Remove this toggle from the toggle group.
        /// </summary>
        public void RemoveFromGroup()
        {
            if(toggleGroup!=null)
            toggleGroup.RemoveMember(this);
        }
        /// <summary>
        /// Add an action to an SKToggle event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void AddListener(SKToggleEventType type, UnityAction action)
        {
            switch (type)
            {
                case SKToggleEventType.OnToggledOn:
                    onToggledOn.AddListener(action);
                    break;
                case SKToggleEventType.OnToggledOff:
                    onToggledOff.AddListener(action);
                    break;
                case SKToggleEventType.OnPress:
                    onPress.AddListener(action);
                    break;
                case SKToggleEventType.OnPointerEnter:
                    onPointerEnter.AddListener(action);
                    break;
                case SKToggleEventType.OnPointerExit:
                    onPointerExit.AddListener(action);
                    break;
                case SKToggleEventType.OnPointerUp:
                    onPointerUp.AddListener(action);
                    break;
                case SKToggleEventType.OnPointerDown:
                    onPointerDown.AddListener(action);
                    break;
                default:
                    break;
            }
        }
        public void RemoveListener(SKToggleEventType type, UnityAction action)
        {
            switch (type)
            {
                case SKToggleEventType.OnToggledOn:
                    onToggledOn.RemoveListener(action);
                    break;
                case SKToggleEventType.OnToggledOff:
                    onToggledOff.RemoveListener(action);
                    break;
                case SKToggleEventType.OnPress:
                    onPress.RemoveListener(action);
                    break;
                case SKToggleEventType.OnPointerEnter:
                    onPointerEnter.RemoveListener(action);
                    break;
                case SKToggleEventType.OnPointerExit:
                    onPointerExit.RemoveListener(action);
                    break;
                case SKToggleEventType.OnPointerUp:
                    onPointerUp.RemoveListener(action);
                    break;
                case SKToggleEventType.OnPointerDown:
                    onPointerDown.RemoveListener(action);
                    break;
                default:
                    break;
            }
        }
        private void SetInteractability(bool interactable)
        {
            this.interactable = interactable;
            OnInteractabilityChanged();
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
            float delta = 0.2f;
            canvasGroup.alpha += delta;
            if (Mathf.Abs(canvasGroup.alpha - 1) < 0.01f)
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
        #endregion

        public enum SKToggleTransitionMode
        {
            Animation,
            BackgroundOnly,
            SelectorOnly,
            BackgroundAndSelector
        }
        public enum SKToggleEventType
        {
            OnToggledOn,
            OnToggledOff,
            OnPress,
            OnPointerEnter,
            OnPointerExit,
            OnPointerUp,
            OnPointerDown
        }
        [Serializable]
        public class ToggleEvent : UnityEvent<bool> { }
    }
}
