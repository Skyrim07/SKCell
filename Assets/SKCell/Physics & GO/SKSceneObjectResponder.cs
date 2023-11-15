using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SKCell
{
    [AddComponentMenu("SKCell/Physics & GO/SKSceneObjectResponder")]
    [RequireComponent(typeof(Collider))]
    
    public class SKSceneObjectResponder : MonoBehaviour
    {
        [HideInInspector] public bool isMouseOver = false;

        public UnityEvent onMouseEnter, onMouseOver, onMouseExit, onMouseDown, onMouseUp, onMouseDrag;


        private void OnMouseOver()
        {
            onMouseOver.Invoke();
        }
        private void OnMouseExit()
        {
            isMouseOver = false;
            onMouseExit.Invoke();
        }

        private void OnMouseDown()
        {
            onMouseDown.Invoke();
        }

        private void OnMouseUp()
        {
            onMouseUp.Invoke();
        }

        private void OnMouseEnter()
        {
            isMouseOver = true;
            onMouseEnter.Invoke();
        }

        private void OnMouseDrag()
        {
            onMouseDrag.Invoke();
        }
    }
}
