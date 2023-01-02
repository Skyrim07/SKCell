using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public class SKScrollEventEmitter : MonoBehaviour
    {
        public SKScrollController controller;

        private void OnEnable()
        {
            controller.OnScrollEnable();
        }
    }
}