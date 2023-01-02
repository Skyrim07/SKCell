using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Non-mono singleton class
/// </summary>
namespace SKCell
{
    public class Singleton<T> where T : class, new()
    {
        protected Singleton() {}

        private static T _inst = null;

        public static T instance => _inst ?? (_inst = new T());

        public static void Clear()
        {
            _inst = null;
        }
    }
}