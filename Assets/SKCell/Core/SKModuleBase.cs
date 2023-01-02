using System.Collections.Generic;

namespace SKCell
{
    /// <summary>
    /// Abstract class for SK module managers.
    /// </summary>
    public abstract class SKModuleBase
    {

        /// <summary>
        /// Module priority. Determines update order.
        /// </summary>
        internal int Priority
        {
            get;
        }

        /// <summary>
        /// Initialize function.
        /// </summary>
        internal abstract void Initialize();

        /// <summary>
        /// Start function.
        /// </summary>
        internal abstract void Start();

        /// <summary>
        /// Update function.
        /// </summary>
        internal abstract void Tick();

        /// <summary>
        /// Fixed update function.
        /// </summary>
        internal abstract void FixedTick();

        /// <summary>
        /// Clear and shutdown module.
        /// </summary>
        internal abstract void Dispose();
    }

    internal class SKModuleComparer : IComparer<SKModuleBase>
    {
        int IComparer<SKModuleBase>.Compare(SKModuleBase x, SKModuleBase y)
        {
            return x.Priority > y.Priority ? 1 : 0;
        }
    }
}
