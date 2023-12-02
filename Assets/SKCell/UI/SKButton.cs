using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace SKCell
{
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("SKCell/UI/SKButton")]
    public class SKButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IFadable
    {
        #region Fields

        [SerializeField] bool interactable = true;
        public bool Interactable { get { return interactable; } set { SetInteractability(value); } }

        [SerializeField] SKImage buttonImage;
        [SerializeField] SKText buttonText;

        [Header("Transition")]
        public SKButtonTransitionMode transitionMode = SKButtonTransitionMode.ColorImageAndText;
        public float transitionTime = 0.1f;
        [SerializeField] Color imageNormalColor = Color.white;
        [SerializeField] Color imageMouseOverColor = new Color(0.7f, 0.7f, 0.7f);
        [SerializeField] Color imageMousePressColor = Color.grey;
        [SerializeField] Color imageDisabledColor = Color.grey;

        [SerializeField] Color textNormalColor = Color.black;
        [SerializeField] Color textMouseOverColor = Color.black;
        [SerializeField] Color textMousePressColor = new Color(0.7f, 0.7f, 0.7f);
        [SerializeField] Color textDisabledColor = new Color(0.6f, 0.6f, 0.6f);

        public bool useScaleTransition = true;
        public bool bouncyScaleTransition = false;
        [SerializeField] float normalScale = 1f, mouseOverScale = 0.95f, mousePressScale = 1.05f;

        [Header("Spam Control")]
        [Tooltip("When turned on, the button will not process events that are too close in time.")]
        [SerializeField] bool spamPrevention = false;
        [Range(0.01f, 3f)] public float spamCooldown = 0.5f;

        [Header("On Press Event")]
        [SerializeField] SKButtonEvent onPress;

        [Header("On Hold Stays Event")]
        [SerializeField] SKButtonEvent onHoldStays;
        [Tooltip("Invoke OnHoldStays event every fixedUpdateInterval while holding the button.")]

        [Header("On Hold Stays For Seconds Event")]
        [SerializeField] SKButtonEvent onHoldStaysForSeconds;
        [Tooltip("Invoke OnHoldStaysForSeconds event after this seconds while holding.")]
        [Range(0.01f, 10f)] [SerializeField] float holdForSecondsTime = 0.5f;

        [Header("On Hold Stays For Seconds2 Event")]
        [SerializeField] SKButtonEvent onHoldStaysForSeconds2;
        [Tooltip("Invoke OnHoldStaysForSeconds2 event after this seconds while holding.")]
        [Range(0.01f, 10f)] [SerializeField] float holdForSecondsTime2 = 0.8f;

        [Header("On Hold Up Event")]
        [SerializeField] SKButtonEvent onHoldUp;
        [Tooltip("Invoke OnHoldUp event after holding the button for this seconds.")]
        [Range(0.01f, 10f)] [SerializeField] float minHoldTime = 1f;

        [Header("On Hold Up Event 2")]
        [SerializeField] SKButtonEvent onHoldUp2;
        [Tooltip("Invoke OnHoldUp2 event after holding the button for this seconds.")]
        [Range(0.01f, 10f)] [SerializeField] float minHoldTime2 = 2f;

        [Header("On Pointer Enter Event")]
        [SerializeField] SKButtonEvent onPointerEnter;

        [Header("On Pointer Exit Event")]
        [SerializeField] SKButtonEvent onPointerExit;

        [Header("On Pointer Up Event")]
        [SerializeField] SKButtonEvent onPointerUp;

        [Header("On Pointer Down Event")]
        [SerializeField] SKButtonEvent onPointerDown;

        [Header("On Start Event")]
        [SerializeField] SKButtonEvent onStart;

        #endregion
        #region Private Fields
        [HideInInspector] public bool initialized = false;
        private bool isHolding = false, isHovering=false;
        private string hash;
        private bool canClick =true;
        [HideInInspector] public bool hasAnimator =false;
        private Animator anim;
        private CanvasGroup canvasGroup;
        private float initialScale = 1;
        #endregion
        private void Start()
        {
            hash = GetHashCode().ToString();
            if (!initialized)
            {
                //CommonUtils.EditorLogError("SKButton not initialized!");
                return;
            }
            initialScale = transform.localScale.x;

            buttonImage.color = interactable ? imageNormalColor : imageDisabledColor;
            buttonText.color = interactable ? textNormalColor : textDisabledColor;

            if (transitionMode == SKButtonTransitionMode.Animation)
                anim = SKUtils.GetComponentNonAlloc<Animator>(gameObject);

            canvasGroup = SKUtils.GetComponentNonAlloc<CanvasGroup>(gameObject);

            onStart.Invoke();
        }
        #region Editor
#if UNITY_EDITOR
        public void GenerateStructure()
        {
            string pathSuffix = "/CommonButton.prefab"; 
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKButton Resource Error: Button prefab lost.");
                initialized = false;
                return;
            }
            GameObject button = Instantiate(prefab);
            button.name = $"SKButton-{GetHashCode()}";
            button.transform.SetParent(transform.parent);
            button.transform.CopyFrom(transform);
            button.transform.SetSiblingIndex(transform.GetSiblingIndex());
            button.GetComponent<SKButton>().initialized = true;
            Selection.activeGameObject = button;
            DestroyImmediate(this.gameObject);
        }

        public void DrawEditorPreview()
        {
            if (buttonImage != null && buttonText != null)
            {
                if (transitionMode == SKButtonTransitionMode.ColorImageOnly)
                {
                    buttonImage.color = interactable ? imageNormalColor : imageDisabledColor;
                    buttonText.color = textNormalColor;
                }
                else if (transitionMode == SKButtonTransitionMode.ColorTextOnly)
                {
                    buttonText.color = interactable ? textNormalColor : textDisabledColor;
                    buttonImage.color = imageNormalColor;
                }
                else if (transitionMode == SKButtonTransitionMode.ColorImageAndText)
                {
                    buttonImage.color = interactable ? imageNormalColor : imageDisabledColor;
                    buttonText.color = interactable ? textNormalColor : textDisabledColor;
                }
            }
        }
        public void AttachController(AnimatorController controller)
        {
            Animator anim=gameObject.AddComponent<Animator>();
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
        public void TransitNormal()
        {
            if (transitionMode == SKButtonTransitionMode.None)
                return;
            if (transitionMode != SKButtonTransitionMode.Animation) //Color
            {
                StopAllCoroutines();
                if (transitionMode != SKButtonTransitionMode.ColorTextOnly)
                   StartCoroutine(TransitImageCR(imageNormalColor));
                if (transitionMode != SKButtonTransitionMode.ColorImageOnly)
                    StartCoroutine(TransitTextCR(textNormalColor));
                if (useScaleTransition)
                    StartCoroutine(TransitScaleCR(transitionTime, normalScale));
            }
            else //Animation
            {
                if (anim == null)
                {
                    SKUtils.EditorLogWarning($"SKButton: Animator not initialized! Gameobject: {name}");
                    return;
                }
                anim.ResetTrigger("Normal");
                anim.SetTrigger("Normal");
            }
        }
        private void TransitOver()
        {
            if (transitionMode == SKButtonTransitionMode.None)
                return;
            if (transitionMode != SKButtonTransitionMode.Animation) //Color
            {
                StopAllCoroutines();
                if (transitionMode != SKButtonTransitionMode.ColorTextOnly)
                    StartCoroutine(TransitImageCR(imageMouseOverColor));
                if (transitionMode != SKButtonTransitionMode.ColorImageOnly)
                    StartCoroutine(TransitTextCR(textMouseOverColor));
                if (useScaleTransition)
                    StartCoroutine(TransitScaleCR(transitionTime, mouseOverScale));
            }
            else //Animation
            {
                if (anim == null)
                {
                    SKUtils.EditorLogWarning($"SKButton: Animator not initialized! Gameobject: {name}");
                    return;
                }
                anim.ResetTrigger("Over");
                anim.SetTrigger("Over");
            }
        }
        private void TransitPress()
        {
            if (transitionMode == SKButtonTransitionMode.None)
                return;
            if (transitionMode != SKButtonTransitionMode.Animation) //Color
            {
                StopAllCoroutines();
                if (transitionMode != SKButtonTransitionMode.ColorTextOnly)
                    StartCoroutine(TransitImageCR(imageMousePressColor));
                if (transitionMode != SKButtonTransitionMode.ColorImageOnly)
                    StartCoroutine(TransitTextCR(textMousePressColor));
                if (useScaleTransition)
                    StartCoroutine(TransitScaleCR(0,mousePressScale));
            }
            else //Animation
            {
                if (anim == null)
                {
                    SKUtils.EditorLogWarning($"SKButton: Animator not initialized! Gameobject: {name}");
                    return;
                }
                anim.ResetTrigger("Pressed");
                anim.SetTrigger("Pressed");
            }
        }
        private void TransitInactive()
        {
            if (transitionMode == SKButtonTransitionMode.None)
                return;
            if (transitionMode != SKButtonTransitionMode.Animation) //Color
            {
                StopAllCoroutines();
                if (transitionMode != SKButtonTransitionMode.ColorTextOnly)
                    StartCoroutine(TransitImageCR(imageDisabledColor));
                if (transitionMode != SKButtonTransitionMode.ColorImageOnly)
                    StartCoroutine(TransitTextCR(textDisabledColor));
            }
            else //Animation
            {
                if (anim == null)
                {
                    SKUtils.EditorLogWarning($"SKButton: Animator not initialized! Gameobject: {name}");
                    return;
                }
                anim.ResetTrigger("Disabled");
                anim.SetTrigger("Disabled");
            }
        }
        private IEnumerator TransitImageCR(Color targetColor)
        {
            float divider = 1f / transitionTime * Time.fixedDeltaTime;
            float deltaR = (targetColor.r - buttonImage.color.r) * divider;
            float deltaG = (targetColor.g - buttonImage.color.g) * divider;
            float deltaB = (targetColor.b - buttonImage.color.b) * divider;
            float deltaA = (targetColor.a - buttonImage.color.a) * divider;
            float stepCount = transitionTime / Time.fixedDeltaTime;
            for (int i = 0; i < stepCount; i++)
            {
                buttonImage.color = new Color(buttonImage.color.r + deltaR, buttonImage.color.g + deltaG, buttonImage.color.b + deltaB, buttonImage.color.a + deltaA);
                yield return new WaitForFixedUpdate();
            }
            buttonImage.color = targetColor;
            yield return null;
        }
        private IEnumerator TransitTextCR(Color targetColor)
        {
            float divider = 1f / transitionTime * Time.fixedDeltaTime;
            float deltaR = (targetColor.r - buttonText.color.r) * divider;
            float deltaG = (targetColor.g - buttonText.color.g) * divider;
            float deltaB = (targetColor.b - buttonText.color.b) * divider;
            float deltaA = (targetColor.a - buttonText.color.a) * divider;
            float stepCount = transitionTime / Time.fixedDeltaTime;
            for (int i = 0; i < stepCount; i++)
            {
                buttonText.color = new Color(buttonText.color.r + deltaR, buttonText.color.g + deltaG, buttonText.color.b + deltaB, buttonText.color.a + deltaA);
                yield return new WaitForFixedUpdate();
            }
            buttonText.color = targetColor;
            yield return null;
        }
        private IEnumerator TransitScaleCR(float transitionTime, float targetScale)
        {
            float realTargetScale = initialScale * targetScale;
            float divider = 1f / transitionTime * Time.fixedDeltaTime;
            float delta =bouncyScaleTransition? ( transform.localScale.x- realTargetScale ) * divider : (realTargetScale-transform.localScale.x) * divider;
            float stepCount = transitionTime / Time.fixedDeltaTime;
            for (int i = 0; i < stepCount; i++)
            {
                transform.localScale = new Vector3(transform.localScale.x + delta, transform.localScale.y + delta, 0);
                yield return new WaitForFixedUpdate();
            }
            transform.localScale = new Vector3(realTargetScale, realTargetScale, realTargetScale);
            yield return null;
        }
        private void SetInteractability(bool interactable)
        {
            this.interactable = interactable;
            OnInteractabilityChange();
        }
        #endregion
        #region Events
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
                return;

            if (spamPrevention)
            {
                if (canClick)
                {
                    onPress.Invoke();
                    SKUtils.CancelInvoke("ResetClickCD");
                    SKUtils.InvokeAction(spamCooldown, ResetClickCD, 0, 0, "ResetClickCD");
                    canClick = false;
                }
            }
            else
            {
                onPress.Invoke();
            }
        }
        private void ResetClickCD()
        {
            canClick = true;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;
                TransitPress();
            //Start hold event
            if (onHoldStays.GetPersistentEventCount() > 0)
            {
                 SKCore.Tick000 += OnHoldStays;
            }
            SKCommonTimer.instance.CreateTimer(hash, 0, false);
            isHolding = true;

            //Invoke onPointerDown event
            if (onPointerDown.GetPersistentEventCount() > 0)
            {
                onPointerDown.Invoke();
            }

            Invoke("OnHoldStaysForSeconds1", holdForSecondsTime);
            Invoke("OnHoldStaysForSeconds2", holdForSecondsTime2);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
                return;
            //End hold event
            if (isHolding)
            {
                if (isHovering)
                    TransitOver();
                else
                    TransitNormal();
                float holdTime = GetCurrentHoldTime();
                if (holdTime >= minHoldTime)
                {
                    onHoldUp.Invoke();
                }
                if (holdTime >= minHoldTime2)
                {
                    onHoldUp2.Invoke();
                }
                if (onHoldStays.GetPersistentEventCount() > 0)
                    SKCore.Tick000 -= OnHoldStays;
                SKCommonTimer.instance.DisposeTimer(hash);
                isHolding = false;
            }
            //Invoke onPointerUp event
            if (onPointerUp.GetPersistentEventCount() > 0)
            {
                onPointerUp.Invoke();
            }

            CancelInvoke("OnHoldStaysForSeconds1");
            CancelInvoke("OnHoldStaysForSeconds2");
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable)
                return;
            if (onPointerEnter.GetPersistentEventCount() > 0)
            {
                onPointerEnter.Invoke();
            }
            isHovering = true;
            TransitOver();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable)
                return;
            if (onPointerExit.GetPersistentEventCount() > 0)
            {
                onPointerExit.Invoke();
            }
            isHovering = false;
            if(!isHolding)
            TransitNormal();
        }

        private void OnHoldStays()
        {
            if (!interactable)
                return;
            onHoldStays.Invoke();
        }
        private void OnHoldStaysForSeconds1()
        {
            if (!interactable)
                return;
            if (onHoldStaysForSeconds.GetPersistentEventCount() > 0)
                onHoldStaysForSeconds.Invoke();
            if (transitionMode == SKButtonTransitionMode.Animation)
                anim.SetTrigger("HoldForSeconds1");
        }
        private void OnHoldStaysForSeconds2()
        {
            if (!interactable)
                return;
            if (onHoldStaysForSeconds2.GetPersistentEventCount() > 0)
                onHoldStaysForSeconds2.Invoke();
            if (transitionMode == SKButtonTransitionMode.Animation)
                anim.SetTrigger("HoldForSeconds2");
        }

        private void OnInteractabilityChange()
        {
            if (interactable)
                TransitNormal();
            else
                TransitInactive();
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Get the current hold time of this SKButton. Return 0 if not being held. 
        /// </summary>
        /// <returns></returns>
        public float GetCurrentHoldTime()
        {
            return isHolding ? (float)SKCommonTimer.instance.GetTimerValue(hash) : 0;
        }
        /// <summary>
        /// Add an action to an SKButton event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void AddListener(SKButtonEventType type, UnityAction action)
        {
            switch (type)
            {
                case SKButtonEventType.OnPressed:
                    onPress.AddListener(action);
                    break;
                case SKButtonEventType.OnHoldStays:
                    onHoldStays.AddListener(action);
                    break;
                case SKButtonEventType.OnHoldStaysForSeconds:
                    onHoldStaysForSeconds.AddListener(action);
                    break;
                case SKButtonEventType.OnHoldStaysForSeconds2:
                    onHoldStaysForSeconds2.AddListener(action);
                    break;
                case SKButtonEventType.OnHoldUp:
                    onHoldUp.AddListener(action);
                    break;
                case SKButtonEventType.OnHoldUp2:
                    onHoldUp2.AddListener(action);
                    break;
                case SKButtonEventType.OnPointerEnter:
                    onPointerEnter.AddListener(action);
                    break;
                case SKButtonEventType.OnpointerExit:
                    onPointerExit.AddListener(action);
                    break;
                case SKButtonEventType.OnPointerUp:
                    onPointerUp.AddListener(action);
                    break;
                case SKButtonEventType.OnPointerDown:
                    onPointerDown.AddListener(action);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Remove an action from an SKButton event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RemoveListener(SKButtonEventType type, UnityAction action)
        {
            switch (type)
            {
                case SKButtonEventType.OnPressed:
                    onPress.RemoveListener(action);
                    break;
                case SKButtonEventType.OnHoldStays:
                    onHoldStays.RemoveListener(action);
                    break;
                case SKButtonEventType.OnHoldStaysForSeconds:
                    onHoldStaysForSeconds.RemoveListener(action);
                    break;
                case SKButtonEventType.OnHoldStaysForSeconds2:
                    onHoldStaysForSeconds2.RemoveListener(action);
                    break;
                case SKButtonEventType.OnHoldUp:
                    onHoldUp.RemoveListener(action);
                    break;
                case SKButtonEventType.OnHoldUp2:
                    onHoldUp2.RemoveListener(action);
                    break;
                case SKButtonEventType.OnPointerEnter:
                    onPointerEnter.RemoveListener(action);
                    break;
                case SKButtonEventType.OnpointerExit:
                    onPointerExit.RemoveListener(action);
                    break;
                case SKButtonEventType.OnPointerUp:
                    onPointerUp.RemoveListener(action);
                    break;
                case SKButtonEventType.OnPointerDown:
                    onPointerDown.RemoveListener(action);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Remove all actions from an SKButton event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        public void RemoveAllListeners(SKButtonEventType type)
        {
            switch (type)
            {
                case SKButtonEventType.OnPressed:
                    onPress.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnHoldStays:
                    onHoldStays.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnHoldStaysForSeconds:
                    onHoldStaysForSeconds.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnHoldStaysForSeconds2:
                    onHoldStaysForSeconds2.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnHoldUp:
                    onHoldUp.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnHoldUp2:
                    onHoldUp2.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnPointerEnter:
                    onPointerEnter.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnpointerExit:
                    onPointerExit.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnPointerUp:
                    onPointerUp.RemoveAllListeners();
                    break;
                case SKButtonEventType.OnPointerDown:
                    onPointerDown.RemoveAllListeners();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Set the content of the button text.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            buttonText.text = text;
        }

        public void UpdateText(int localID)
        {
            buttonText.UpdateLocalID(localID);
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

        public enum SKButtonTransitionMode
        {
            ColorImageOnly,
            ColorTextOnly,
            ColorImageAndText,
            Animation,
            None
        }
    }
    public enum SKButtonEventType
    {
        OnPressed,
        OnHoldStays,
        OnHoldStaysForSeconds,
        OnHoldStaysForSeconds2,
        OnHoldUp,
        OnHoldUp2,
        OnPointerEnter,
        OnpointerExit,
        OnPointerUp,
        OnPointerDown
    }
    [Serializable] public class SKButtonEvent : UnityEvent { }
}
