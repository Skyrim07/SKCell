using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SKCell
{
    [DisallowMultipleComponent]
    [AddComponentMenu("SKCell/Physics & GO/SKColliderResponder")]
    /// <summary>
    /// Collider responder with trigger/collision events to be registered.
    /// </summary>
    public class SKColliderResponder : MonoBehaviour
    {
        /// <summary>
        /// Specify a unique id to identify this responder.
        /// </summary>
        public string uid = string.Empty;

        [Tooltip("Disable this respoder after this count of events invoked. -1 stands for infinity.")]
        [Range(-1, 100)]
        public int maxEventCount = -1;
        private int curEventCount = 0;

        [Header("Responding Collision Type")]
        public bool trigger3D = true;
        public bool collision3D = true;
        public bool trigger2D = true;
        public bool collision2D = true;
        #region Events

        public UnityAction<Collider> onTriggerEnter = new UnityAction<Collider>(EmptyAction), 
            onTriggerStay = new UnityAction<Collider>(EmptyAction),
            onTriggerExit = new UnityAction<Collider>(EmptyAction);
        public UnityAction<Collision> onCollisionEnter = new UnityAction<Collision>(EmptyAction), 
            onCollisionStay = new UnityAction<Collision>(EmptyAction), 
            onCollisionExit = new UnityAction<Collision>(EmptyAction);
        public UnityAction<Collider2D> onTriggerEnter2D = new UnityAction<Collider2D>(EmptyAction), 
            onTriggerStay2D = new UnityAction<Collider2D>(EmptyAction), 
            onTriggerExit2D = new UnityAction<Collider2D>(EmptyAction);
        public UnityAction<Collision2D> onCollisionEnter2D = new UnityAction<Collision2D>(EmptyAction),
            onCollisionStay2D = new UnityAction<Collision2D>(EmptyAction), 
            onCollisionExit2D = new UnityAction<Collision2D>(EmptyAction);


        private void OnTriggerEnter(Collider cld)
        {
            if (trigger3D)
            {
                onTriggerEnter.Invoke(cld);
                OnInvokeEvent();
            }
        }
        private void OnTriggerStay(Collider cld)
        {
            if (trigger3D)
            {
                onTriggerStay.Invoke(cld);
                OnInvokeEvent();
            }
        }
        private void OnTriggerExit(Collider cld)
        {
            if (trigger3D)
            {
                onTriggerExit.Invoke(cld);
                OnInvokeEvent();
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision3D)
            {
                onCollisionEnter.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (collision3D)
            {
                onCollisionStay.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision3D)
            {
                onCollisionExit.Invoke(collision);
                OnInvokeEvent();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (trigger2D)
            {
                onTriggerEnter2D.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (trigger2D)
            {
                onTriggerStay2D.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (trigger2D)
            {
                onTriggerExit2D.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision2D)
            {
                onCollisionEnter2D.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision2D)
            {
                onCollisionStay2D.Invoke(collision);
                OnInvokeEvent();
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision2D)
            {
                onCollisionExit2D.Invoke(collision);
                OnInvokeEvent();
            }
        }
        #endregion

        private void OnInvokeEvent()
        {
            SKCldResponderManager.lastResponder = this;
            if(++curEventCount>=maxEventCount && maxEventCount > 0)
            {
                this.enabled = false;
            }
        }

        private void Awake()
        {
            if (!uid.Equals(string.Empty))
            {
                SKCldResponderManager.AddResponder(this);
            }
        }

        public void SetUID(string uid)
        {
            SKCldResponderManager.UpdateResponderUID(this, this.uid, uid);
        }

        private static void EmptyAction(Collider cld) { }
        private static void EmptyAction(Collider2D cld) { }
        private static void EmptyAction(Collision cld) { }
        private static void EmptyAction(Collision2D cld) { }

    }

}
