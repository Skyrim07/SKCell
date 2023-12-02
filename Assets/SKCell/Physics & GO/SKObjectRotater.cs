using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SKCell
{
    [AddComponentMenu("SKCell/Physics & GO/SKObjectRotater")]
    /// <summary>
    /// Add rotation behavior to scene objects
    /// </summary>
    public class SKObjectRotater : MonoBehaviour
    {
        public string uid = string.Empty;
        [Header("Rotation")]
        public float sensitivity = 0.1f;

        [Header("Rotation Constraints")]
        public bool xRotation = true;
        public bool yRotation = true;

        [Header("Inertia")]
        public bool applyInertia = true;
        public float damping = 1f;

        [Header("Events")]
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag, onEndDrag, onStop;

        private Camera cam;
        private Vector3 lastMousePos, rotAngle;

        private void Start()
        {
            cam = Camera.main;
        }
        private void OnMouseDown()
        {
            onBeginDrag.Invoke();

            if(applyInertia)
                 StopCoroutine(nameof(InertiaCR));
            lastMousePos = Input.mousePosition;
        }
        private void OnMouseDrag()
        {
            onDrag.Invoke();

            Vector3 mouseDelta = Input.mousePosition - lastMousePos;
            rotAngle = new Vector3(yRotation ? mouseDelta.y * sensitivity : 0, xRotation ? -mouseDelta.x * sensitivity : 0, 0);
            transform.Rotate(rotAngle ,Space.World);
            lastMousePos = Input.mousePosition;
        }
        private void OnMouseUp()
        {
            onEndDrag.Invoke();

            if (applyInertia)
                ApplyInertia(rotAngle);
        }

        private void ApplyInertia(Vector3 formerRotAngle)
        {
            SKUtils.StartCoroutine(InertiaCR(formerRotAngle));
        }
        IEnumerator InertiaCR(Vector3 formerRotAngle)
        {
            float force = 1f;
            while (force >= 0.001f)
            {
                force -= damping * Time.fixedDeltaTime;
                transform.Rotate(rotAngle*force, Space.World);
                yield return new WaitForFixedUpdate();
            }
            onStop.Invoke();
        }
    }
}
