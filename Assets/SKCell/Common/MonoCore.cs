using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Control mono lifecycle flow.
    /// </summary>
    public sealed class MonoCore : SKMonoSingleton<MonoCore>
    {
        private static Dictionary<MonoPriority, List<MonoBase>> monoBases = new Dictionary<MonoPriority, List<MonoBase>>();

        /// <summary>
        /// Late ticks will only be called if this is false.
        /// </summary>
        public bool lateTickLock { get; private set; } = true;

        /// <summary>
        /// Register a mono object to the list. Called by MonoBase.Awake.
        /// </summary>
        /// <param name="mb"></param>
        public static void RegisterMonoObject(MonoBase mb)
        {
            if (mb != null)
                SKUtils.InsertToList(monoBases[mb.priority], mb, true);
        }
        /// <summary>
        /// Remove a mono object from list. Called by MonoBase.Dispose.
        /// </summary>
        /// <param name="mb"></param>
        public static void DisposeMonoObject(MonoBase mb)
        {
            if (mb != null)
                SKUtils.RemoveFromList(monoBases[mb.priority], mb);
        }

        /// <summary>
        /// Set the late tick lock. If false, late ticks will be called per frame.
        /// </summary>
        /// <param name="isLock"></param>
        public void SetLateTickLock(bool isLock)
        {
            lateTickLock = isLock;
        }

        #region Lifecycle
        protected override void Awake()
        {
            monoBases.Clear();
            for (int i = 0; i < 10; i++)
            {
                monoBases.Add((MonoPriority)i, new List<MonoBase>());
            }
        }

        /// <summary>
        /// Loop through all mono object and execute their lifecycle functions according to their priority.
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (var item in monoBases[(MonoPriority)i])
                {
                    item.Tick(Time.unscaledDeltaTime, Time.deltaTime);
                }
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (var item in monoBases[(MonoPriority)i])
                {
                    item.FixedTick(Time.unscaledDeltaTime, Time.deltaTime);
                }
            }
        }
        private void LateUpdate()
        {
            if (lateTickLock)
                return;

            for (int i = 0; i < 10; i++)
            {
                foreach (var item in monoBases[(MonoPriority)i])
                {
                    item.LateTick(Time.unscaledDeltaTime, Time.deltaTime);
                }
            }
        }
        #endregion
    }
}
