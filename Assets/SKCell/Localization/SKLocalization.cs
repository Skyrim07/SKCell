using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
    #region Mono class
    /// <summary>
    /// Mono class to label localized text components
    /// </summary>
    [AddComponentMenu("SKCell/Localization/SKLocalization")]
    [ExecuteInEditMode]
    public class SKLocalization : MonoBehaviour
    {
        public static SKLocalizationAsset asset;
        private static bool initialized;

        [SerializeField] int _localID = -1;
        public int localID
        {
            get
            {
                return _localID;
            }
            set
            {
                _localID = value;
            }
        }
        [SerializeField] LocalizationMethod method;

        [HideInInspector]
        public LocalizedInfo info;

        #region ApplyLocalization methods
        /// <summary>
        /// Apply a certain localization. Called by SK Components.
        /// </summary>
        /// <param name="lang"></param>
        public void ApplyLocalization(LanguageSupport lang, LocalizationType type)
        {
            if (method == LocalizationMethod.None)
            {
                return;
            }
            if (!UpdateLocalizedInfo(lang, type))
            {
                return;
            }
            switch (type)
            {
                case LocalizationType.Image:
                    SKImage image = SKUtils.GetComponentNonAlloc<SKImage>(gameObject);
                    if (image == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKImage> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (info.localImage != null)
                        image.sprite = Sprite.Create(info.localImage, new Rect(0, 0, info.localImage.width, info.localImage.height), new Vector2(0.5f, 0.5f));
                    break;
                case LocalizationType.Text:
                    SKText text = SKUtils.GetComponentNonAlloc<SKText>(gameObject);
                    if (text == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKText> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (!info.localText.Equals(string.Empty))
                        text.textAnimator.UpdateText(info.localText);

                    SKFontAsset asset = SKAssetLibrary.FontAsset;

                    if (info.fontOverride >= 0)
                    {
                        text.font = asset.fontList[info.fontOverride];
                    }
                    else
                    {
                        int font = SKLocalization.asset.languageFonts[(int)SKEnvironment.curLanguage];

                        if (font>=0 && font<asset.fontList.Count)
                            text.font =asset.fontList[font];
                    }
                    break;
            }
        }
        /// <summary>
        /// Apply localization with the given arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lang"></param>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public void ApplyLocalization<T>(LanguageSupport lang, LocalizationType type, T arg0)
        {
            if (method == LocalizationMethod.None)
            {
                return;
            }
            if (!UpdateLocalizedInfo(lang, type))
            {
                return;
            }
            switch (type)
            {
                case LocalizationType.Image:
                    SKImage image = SKUtils.GetComponentNonAlloc<SKImage>(gameObject);
                    if (image == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKImage> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (info.localImage != null)
                        image.sprite = Sprite.Create(info.localImage, new Rect(0, 0, info.localImage.width, info.localImage.height), new Vector2(0.5f, 0.5f));
                    break;
                case LocalizationType.Text:
                    SKText text = SKUtils.GetComponentNonAlloc<SKText>(gameObject);
                    if (text == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKText> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (!info.localText.Equals(string.Empty))
                    {
                        text.textAnimator.UpdateText(string.Format(info.localText,arg0));
                    }
                    SKFontAsset asset = SKAssetLibrary.FontAsset;
                    if (info.fontOverride >= 0)
                    {
                        text.font = asset.fontList[info.fontOverride];
                    }
                    else
                    {
                        int font = SKLocalization.asset.languageFonts[(int)SKEnvironment.curLanguage];
                       if(font>=0 && font<asset.fontList.Count)
                            text.font = asset.fontList[font];
                    }
                    break;
            }
        }
        public void ApplyLocalization<T>(LanguageSupport lang, LocalizationType type, T arg0, T arg1)
        {
            if (method == LocalizationMethod.None)
            {
                return;
            }
            if (!UpdateLocalizedInfo(lang, type))
            {
                return;
            }
            switch (type)
            {
                case LocalizationType.Image:
                    SKImage image = SKUtils.GetComponentNonAlloc<SKImage>(gameObject);
                    if (image == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKImage> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (info.localImage != null)
                        image.sprite = Sprite.Create(info.localImage, new Rect(0, 0, info.localImage.width, info.localImage.height), new Vector2(0.5f, 0.5f));
                    break;
                case LocalizationType.Text:
                    SKText text = SKUtils.GetComponentNonAlloc<SKText>(gameObject);
                    if (text == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKText> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (!info.localText.Equals(string.Empty))
                    {
                        text.textAnimator.UpdateText(string.Format(info.localText, arg0, arg1));
                    }
                    SKFontAsset asset = SKAssetLibrary.FontAsset;
                    if (info.fontOverride >= 0)
                    {
                        text.font = asset.fontList[info.fontOverride];
                    }
                    else
                    {
                        int font = SKLocalization.asset.languageFonts[(int)SKEnvironment.curLanguage];
                       if(font>=0 && font<asset.fontList.Count)
                            text.font = asset.fontList[font];
                    }
                    break;
            }
        }
        public void ApplyLocalization<T>(LanguageSupport lang, LocalizationType type, T arg0, T arg1, T arg2)
        {
            if (method == LocalizationMethod.None)
            {
                return;
            }
            if (!UpdateLocalizedInfo(lang, type))
            {
                return;
            }
            switch (type)
            {
                case LocalizationType.Image:
                    SKImage image = SKUtils.GetComponentNonAlloc<SKImage>(gameObject);
                    if (image == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKImage> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (info.localImage != null)
                        image.sprite = Sprite.Create(info.localImage, new Rect(0, 0, info.localImage.width, info.localImage.height), new Vector2(0.5f, 0.5f));
                    break;
                case LocalizationType.Text:
                    SKText text = SKUtils.GetComponentNonAlloc<SKText>(gameObject);
                    if (text == null)
                    {
                        SKUtils.EditorLogError($"Localization error: Component <SKText> does not exist. Gameobject: {name}");
                        return;
                    }
                    if (!info.localText.Equals(string.Empty))
                    {
                        text.textAnimator.UpdateText(string.Format(info.localText, arg0, arg1, arg2));
                    }
                    SKFontAsset asset = SKAssetLibrary.FontAsset;
                    if (info.fontOverride >= 0)
                    {
                        text.font = asset.fontList[info.fontOverride];
                    }
                    else
                    {
                        int font = SKLocalization.asset.languageFonts[(int)SKEnvironment.curLanguage];
                       if(font>=0 && font<asset.fontList.Count)
                            text.font = asset.fontList[font];
                    }
                    break;
            }
        }

        #endregion
        public bool UpdateLocalizedInfo(LanguageSupport lang, LocalizationType type)
        {
            if (!initialized)
            {
                LoadConfigAsset();
                InitializeAssetDictionary();
                initialized = true;
            }
            if (!CheckValidity(lang, type))
            {
                return false;
            }
            if (info == null) //Create an info only once
            {
                info = new LocalizedInfo();
            }
            if (type == LocalizationType.Text)
            {
                LocalizedTextConfig config = asset.textConfigDict[_localID];
                info.UpdateInfo(lang, method, type, config);
            }
            if (type == LocalizationType.Image)
            {
                if (!asset.imageConfigDict.ContainsKey(_localID))
                {
                    SKUtils.EditorLogError($"Localization Error: ID not configured. Gameobject: {name}, LocalID: {_localID}");
                }
                LocalizedImageConfig config = asset.imageConfigDict[_localID];
                info.UpdateInfo(lang, method, type, null, config);
            }
            return true;
        }

        private static void LoadConfigAsset()
        {
            if (asset == null)
            {
                asset = new SKLocalizationAsset(SKUtils.SKLoadObjectFromJson<SKLocalizationAssetJson>("SKLocalizationAsset.json"));
            }
        }

        /// <summary>
        /// Dictionaries are not serializable. So we need to build the dictionary in play mode. (Just once)
        /// </summary>
        public static void InitializeAssetDictionary()
        {
            foreach (var item in asset.textConfigs)
            {
                SKUtils.InsertOrUpdateKeyValueInDictionary(asset.textConfigDict, item.id, item);
            }
            foreach (var item in asset.imageConfigs)
            {
                SKUtils.InsertOrUpdateKeyValueInDictionary(asset.imageConfigDict, item.id, item);
            }
        }

        /// <summary>
        /// Get localized text according to the current language.
        /// </summary>
        /// <param name="localID"></param>
        /// <returns></returns>
        public static string GetLocalizationText(int localID)
        {
            return GetLocalizationText(localID, SKEnvironment.curLanguage);
        }

        /// <summary>
        /// Get localized text according to the specified language.
        /// </summary>
        /// <param name="localID"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetLocalizationText(int localID, LanguageSupport language)
        {
            if (!initialized)
            {
                LoadConfigAsset();
                InitializeAssetDictionary();
            }
            if (!asset.textConfigDict.ContainsKey(localID))
                return "";

            string s = asset.textConfigDict[localID].localTexts[(int)language];
            s = s.Replace(@"\r", "\r");
            s = s.Replace(@"\n", "\n");
            return s;
        }

        private bool CheckValidity(LanguageSupport lang, LocalizationType type)
        {
            if (!asset.languageSupports.Contains(lang))
            {
                SKUtils.EditorLogError($"Localization Error: Language not supported. Gameobject: {name}, LocalID: {_localID}, Language: {lang}");
                return false;
            }
            if (type == LocalizationType.Text)
            {
                if (!asset.textConfigDict.ContainsKey(_localID))
                {
                    SKUtils.EditorLogError($"Localization Error: ID not configured. Gameobject: {name}, LocalID: {_localID}");
                    return false;
                }
            }
            if (type == LocalizationType.Image)
            {
                if (!asset.imageConfigDict.ContainsKey(_localID))
                {
                    SKUtils.EditorLogError($"Localization Error: ID not configured. Gameobject: {name}, LocalID: {_localID}");
                    return false;
                }
            }
            return true;
        }
    }
    #endregion
   
}
