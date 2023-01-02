using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SKCell
{
    [CreateAssetMenu(fileName = "SKFontAsset", menuName = "SKCell/Font/SKFontAsset", order = 0)]
    [Serializable]
    public class SKFontAsset : ScriptableObject
    {
        public List<TMP_FontAsset> fontList = new List<TMP_FontAsset>();
    }
}
