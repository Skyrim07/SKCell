using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SKCell
{
    /// <summary>
    /// Add drag behavior to game objects.
    /// </summary>
    [AddComponentMenu("SKCell/UI/SKDragger")]
    public class SKDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int draggerID = 0;

        [Tooltip("Enable to activate extended features such as SKDragSticker.")]
        public bool enablePhysicsModule = true;

        [Tooltip("Centers the gameobject on the pointer position on drag.")]
        [SerializeField] bool centerOnDrag = false;

        [SerializeField] bool isCameraCanvas = false;

        [Tooltip("The draggable area of this object.")]
        public RectTransform constraint;
        [Tooltip("The self area of this object. Use this rect transform as default.")]
        public RectTransform selfBound;

        [Header("Events")]
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag, onEndDrag, onEnterConstraint, onLeaveConstraint, onSpawn, onDispose;

        [HideInInspector]
        public SKDragSticker.SKDragStickerPullEvent onEndDragSticker;

        [HideInInspector] public bool isDragging = false, followsPointer = true, occupied = false;

        private Vector2 pointerOffset;
        private Rect constraintWorldRect;
        private bool isInConstraintLastFrame = false;
        private float constraintXMin, constraintXMax, constraintYMin, constraintYMax;
        private bool disposed = false;

        private RectTransform selfTF;
        private BoxCollider2D cld;
        private Rigidbody2D rb;
        [HideInInspector] public SKDragSticker sticker = null;
        private void Start()
        {
            if (selfBound == null)
                selfBound = GetComponent<RectTransform>();
            selfTF = GetComponent<RectTransform>();
            if (constraint != null)
            {
                constraintWorldRect = constraint.WorldRect();
                constraintXMin = constraintWorldRect.xMin + selfBound.rect.width / 2;
                constraintXMax = constraintWorldRect.xMax - selfBound.rect.width / 2;
                constraintYMin = constraintWorldRect.yMin + selfBound.rect.height / 2;
                constraintYMax = constraintWorldRect.yMax - selfBound.rect.height / 2;
            }
            if (enablePhysicsModule)
            {
                cld = gameObject.AddComponent<BoxCollider2D>();
                cld.size = new Vector2(selfBound.rect.width, selfBound.rect.height);
                cld.isTrigger = true;
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        #region Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (disposed)
                return;
            onBeginDrag.Invoke();
            if (!isCameraCanvas)
            {
                pointerOffset = centerOnDrag ? Vector2.zero : eventData.position - (Vector2)transform.position;
            }
            else
            {
                pointerOffset = centerOnDrag ? Vector2.zero : (Vector2)Camera.main.ScreenToWorldPoint(eventData.position) - (Vector2)transform.position;
            }

            isDragging = true;
            followsPointer = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (disposed)
                return;
            onDrag.Invoke();
            if(followsPointer)
                FollowPointer(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (disposed)
                return;
            onEndDrag.Invoke();
            onEndDragSticker.Invoke(this);
            isDragging = false;
            followsPointer = false;
        }

        public void OnDispose()
        {
            disposed = true;
            onDispose.Invoke();
            Destroy(gameObject, 3f);
        }

        #endregion
        #region Methods
        private void FollowPointer(PointerEventData eventData)
        {
            if(!isCameraCanvas)
                transform.position = eventData.position - pointerOffset;
            else
            {
                selfTF.position = (Vector2)Camera.main.ScreenToWorldPoint(eventData.position) - pointerOffset;
            }

            if (constraint != null)
            {
                ApplyConstraint();
            }
        }
        private void ApplyConstraint()
        {
            bool isTouchingConstraint = transform.position.x < constraintXMin || transform.position.x > constraintXMax
                || transform.position.y < constraintYMin || transform.position.y > constraintYMax;
            if (isTouchingConstraint&&!isInConstraintLastFrame)
            {
                onEnterConstraint.Invoke();
            }
            if (!isTouchingConstraint && isInConstraintLastFrame)
            {
                onLeaveConstraint.Invoke();
            }
            isInConstraintLastFrame = isTouchingConstraint;
            float rangeX = Mathf.Clamp(transform.position.x, constraintXMin, constraintXMax);
            float rangeY = Mathf.Clamp(transform.position.y, constraintYMin, constraintYMax);
            transform.position = new Vector3(rangeX, rangeY);
        }
        #endregion
    }
}
