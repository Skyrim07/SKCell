using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [ExecuteInEditMode]
    public sealed class SKPostProcessMask_Saturation : MonoBehaviour
    {
        public bool keepUpdate = true;

        public PPMask mask;
        [Range(0f, 1f)]
        public float saturation = 0.5f;

        private void Update()
        {
            if (keepUpdate)
            {
                SendToManager();
            }
        }

        private void SendToManager()
        {
            CommonUtils.InsertToList(SKPostProcessManager.instance.sat_masks, this, false);
        }
    }
}
