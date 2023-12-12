using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SKCell
{
    /// <summary>
    /// A sticky field to drag into.
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("SKCell/UI/SKDragSticker")]
    public class SKDragSticker : MonoBehaviour
    {
        public int stickerID = 0;
        [Range(0.01f,1f)]
        public float attractForce = 0.2f;
        public bool instantStick = false;

        [Tooltip("The self area of this object. Use this rect transform as default.")]
        public RectTransform selfBound;
        [HideInInspector]
        public SKDragger drag;

        [Header("Events")]
        public SKDragStickerEvent onEnter=new SKDragStickerEvent();
        public SKDragStickerEvent onStick=new SKDragStickerEvent();
        public SKDragStickerEvent onExit=new SKDragStickerEvent();

        private BoxCollider2D cld;
        private UnityAction pullEvent;
        private void Awake()
        {
            if (selfBound == null)
                selfBound = GetComponent<RectTransform>();

            cld = gameObject.GetOrAddComponent<BoxCollider2D>();
            cld.size = new Vector2(selfBound.rect.width, selfBound.rect.height);
            cld.isTrigger = true;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            drag = collision.gameObject.GetComponent<SKDragger>();
            if (drag == null)
                return;
            onEnter.Invoke(drag,this);
            if (instantStick)
            {
                m_OnTriggerEnter(drag);
            }
            else
            {
                drag.onEndDragSticker.AddListener(m_OnTriggerEnter);
            }
        }

        private void m_OnTriggerEnter(SKDragger drag)
        {
           
            if (drag.occupied)
                return;
            drag.onEndDragSticker = new SKDragStickerPullEvent();
            drag.onEndDragSticker.AddListener(StartPulling);
            StartPulling(drag);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            SKDragger drag = collision.gameObject.GetComponent<SKDragger>();
            if (drag == null)
                return;
            drag.sticker = null;
            onExit.Invoke(drag, this);
            drag.onEndDragSticker.RemoveListener(m_OnTriggerEnter);
            drag.onEndDragSticker.RemoveListener(StartPulling);
        }

        private void StartPulling(SKDragger drag)
        {
            StartCoroutine(PullDraggable(drag));
        }

        IEnumerator PullDraggable(SKDragger drag)
        {
            drag.followsPointer = false;
            drag.occupied = true;
            drag.sticker = this;
            Transform tf = drag.transform;
            while(Vector3.SqrMagnitude(transform.position-tf.position)>1f)
            {
                tf.position = Vector3.Lerp(tf.position, transform.position, attractForce);
                yield return new WaitForFixedUpdate();
            }
            tf.position = transform.position;
            drag.occupied = false;
            onStick.Invoke(drag,this);
        }
        [Serializable]
        public class SKDragStickerEvent : UnityEvent<SKDragger,SKDragSticker> { }
        [Serializable]
        public class SKDragStickerPullEvent : UnityEvent<SKDragger> { }
    }
}
