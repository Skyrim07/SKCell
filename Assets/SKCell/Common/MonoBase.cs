using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for mono objects. Controlled by MonoCore.
/// Priority-------------------
/// 500: Common
/// </summary>
namespace SKCell
{
    public class MonoBase : MonoBehaviour
    {
        /// <summary>
        /// Execution priority of this mono object. Cannot be changed after initialization.
        /// </summary>
        public MonoPriority priority = MonoPriority.MP_500_COMMON;
        protected virtual void InitPriority() { }
        public virtual void Start()
        {
            InitPriority();
            MonoCore.RegisterMonoObject(this);
        }
        private void OnDestroy()
        {
            Dispose();
        }

        public virtual void Tick(float unscaledDeltaTime, float deltaTime)
        {

        }

        public virtual void FixedTick(float unscaledDeltaTime, float deltaTime)
        {

        }

        public virtual void LateTick(float unscaledDeltaTime, float deltaTime)
        {

        }

        protected virtual void Dispose()
        {
            if(MonoCore.instance && this)
            MonoCore.DisposeMonoObject(this);
        }
    }

    /// <summary>
    /// Execution priority for mono objects. 0(fastest) - 900(slowest).
    /// </summary>
    public enum MonoPriority
    {
        MP_0,
        MP_100,
        MP_200,
        MP_300,
        MP_400,
        MP_500_COMMON,
        MP_600,
        MP_700,
        MP_800,
        MP_900
    }
}
