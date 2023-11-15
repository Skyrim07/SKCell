using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace SKCell
{
    [AddComponentMenu("SKCell/Physics & GO/SKObjectScaler")]
    public class SKObjectScaler : MonoBehaviour
    {
        public string uid = string.Empty;
        [Header("Scaling")]
        public float sensitivity = 0.2f;

        [Header("Scaling Constraints")]
        public bool xScaling = true;
        public bool yScaling = true;
        public bool zScaling = true;
        public float minScale = 0.2f, maxScale = 2f;

        [Header("Events")]
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag, onEndDrag;

        private Camera cam;

        private Vector3 lastPos, offset;
        private void Start()
        {
            cam = Camera.main;
        }
        private void OnMouseDown()
        {
            onBeginDrag.Invoke();
            lastPos = Input.mousePosition;
        }
        private void OnMouseDrag()
        {
            onDrag.Invoke();

            Vector3 v = cam.ScreenToWorldPoint(Input.mousePosition - offset);
            float scalingFactor = Input.mousePosition.SimpleDistanceSigned(lastPos)* sensitivity*Time.deltaTime;
            transform.localScale += new Vector3( (xScaling ? scalingFactor : 0),
                (yScaling ? scalingFactor : 0),
               (zScaling ? scalingFactor : 0));
            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, minScale, maxScale),
                Mathf.Clamp(transform.localScale.y, minScale, maxScale),
                Mathf.Clamp(transform.localScale.z, minScale, maxScale));
            lastPos = Input.mousePosition;
        }
        private void OnMouseUp()
        {
            onEndDrag.Invoke();
        }
    }
}
