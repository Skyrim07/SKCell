using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mono singleton class
/// </summary>
namespace SKCell {
    public abstract class SKMonoSingleton<T> : MonoBehaviour where T : SKMonoSingleton<T>
    {
        private static T _inst = null;

        public static T instance
        {
            get
            {
                return _inst;
            }
        }

        protected virtual void Awake()
        {
            _inst = (T)this;
        }
    }
}
