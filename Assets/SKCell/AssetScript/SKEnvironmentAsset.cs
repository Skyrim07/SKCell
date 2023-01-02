using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [CreateAssetMenu(fileName = "SKEnvironmentAsset", menuName = "SKCell/SKEnvironmentAsset", order = 0)]
    [Serializable]
    public class SKEnvironmentAsset : ScriptableObject
    {
        public LanguageSupport defaultLanguage;
    }
}
