using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SKCell
{
    [AddComponentMenu("SKCell/Physics & GO/SKObjectDragger")]
    /// <summary>
    /// Add dragging behaviour to scene objects. (SKDragger is the version for UI elements.)
    /// </summary>
    public class SKObjectDragger : MonoBehaviour
    {
        public string uid=string.Empty;
        [Header("Translation Constraints")]
        public bool xTranslation = true;
        public bool yTranslation = true;

        [Header("Inertia")]
        public bool applyInertia = true;
        public float damping = 1;

        [Header("Events")]
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag, onEndDrag, onStop;

        private Camera cam;

        private Vector3 offset, translateVector;
        private void Start()
        {
            cam = Camera.main;
        }
        private void OnMouseDown()
        {
            offset = Input.mousePosition - cam.WorldToScreenPoint(transform.position);
            onBeginDrag.Invoke();
        }
        private void OnMouseDrag()
        {
            onDrag.Invoke();

            Vector3 v = cam.ScreenToWorldPoint(Input.mousePosition - offset);
            translateVector = new Vector3(xTranslation ? v.x : transform.position.x, yTranslation ? v.y : transform.position.y, transform.position.z)-transform.position;
            transform.Translate(translateVector);
        }
        private void OnMouseUp()
        {
            onEndDrag.Invoke();

            if (applyInertia)
                ApplyInertia(translateVector);
        }

        private void ApplyInertia(Vector3 translateVector)
        {
            SKUtils.StartCoroutine(InertiaCR(translateVector));
        }
        IEnumerator InertiaCR(Vector3 translateVector)
        {
            float force = 1f;
            while (force >= 0.001f)
            {
                force -= damping * Time.deltaTime;
                transform.Translate(translateVector * force);
                yield return new WaitForEndOfFrame();
            }
            onStop.Invoke();
        }
    }
}
