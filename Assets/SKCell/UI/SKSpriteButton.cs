using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/UI/SKSpriteButton")]
    public sealed class SKSpriteButton : MonoBehaviour
    {
        public bool allowPlayerActivation = true;

        public float size_MouseOver = 1.2f;
        public float size_MouseDown = 0.8f;

        public bool activeOneTime = true;

        public AudioClip clip;
        public SKButtonEvent onClick;

        private bool isActive = true;
        private Vector3 oScale;
        private float targetSize;
        private bool isTransition;
        private void Start()
        {
            oScale = transform.localScale;
            targetSize = 1;
        }

        private void FixedUpdate()
        {
            if (isTransition)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, oScale * targetSize, 0.2f);
            }
        }

        private void OnMouseEnter()
        {
            isTransition = true;
            targetSize = size_MouseOver;
        }

        private void OnMouseExit()
        {
            isTransition = true;
            targetSize = 1;
        }

        private void OnMouseUpAsButton()
        {
            if (!isActive)
                return;

            isTransition = true;
            targetSize = size_MouseDown;

            if (clip != null)
            {
                SKAudioManager.instance.PlaySound(clip.name, null, false, 1, 1, 0);
            }
            if (onClick != null)
            {
                onClick.Invoke();
                if (activeOneTime)
                {
                    isActive = false;
                }
            }
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.tag.Equals("Player"))
        //    {
        //        if (allowPlayerActivation)
        //        {
        //            OnMouseUpAsButton();
        //        }
        //    }
        //}
    }
}