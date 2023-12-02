using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Provides public localization functions
    /// </summary>
    public class SKLocalizationManager
    {
        private static List<SKText> texts = new List<SKText>();
        private static List<SKImage> images = new List<SKImage>();

        public SKLocalizationManager() { }

        /// <summary>
        /// Localize all objects in the scene
        /// </summary>
        /// <param name="language"></param>
        public static void LocalizeAll()
        {
            LanguageSupport language = SKEnvironment.curLanguage;
            UpdateAllLocalizationComponents();
            foreach (var item in texts)
            {
                item.ApplyLocalization(language);
            }
            foreach (var item in images)
            {
                item.ApplyLocalization(language);
            }
        }
        /// <summary>
        /// Localize all objects in the scene with language
        /// </summary>
        /// <param name="language"></param>
        public static void LocalizeAll(LanguageSupport language)
        {
            UpdateAllLocalizationComponents();
            foreach (var item in texts)
            {
                item.ApplyLocalization(language);
            }
            foreach (var item in images)
            {
                item.ApplyLocalization(language);
            }
        }
        /// <summary>
        /// Localize the given objects
        /// </summary>
        /// <param name="language"></param>
        /// <param name="type"></param>
        /// <param name="texts"></param>
        /// <param name="images"></param>
        public static void LocalizeObjects(LocalizationType type, List<SKText> texts = null, List<SKImage> images = null)
        {
            LanguageSupport language = SKEnvironment.curLanguage;
            if (texts != null)
                foreach (var item in texts)
                {
                    item.ApplyLocalization(language);
                }
            if (images != null)
                foreach (var item in images)
                {
                    item.ApplyLocalization(language);
                }
        }
        /// <summary>
        /// Localize a single object
        /// </summary>
        /// <param name="language"></param>
        /// <param name="type"></param>
        /// <param name="go"></param>
        public static void Localize(GameObject go)
        {
            LanguageSupport language = SKEnvironment.curLanguage;
            SKText text = SKUtils.GetComponentNonAlloc<SKText>(go);
            if (text != null)
            {
                text.ApplyLocalization(language);
            }
            SKImage image = SKUtils.GetComponentNonAlloc<SKImage>(go);
            if (image != null)
            {
                image.ApplyLocalization(language);
            }
            if (text == null && image == null)
            {
                SKUtils.EditorLogWarning($"Localization Error: Not a localizable object. Gameobject: {go.name}");
            }
        }

        public static void LocalizeAllChildren(GameObject go)
        {
            List<Transform> children = go.transform.GetAllChildren();
            List<SKText> texts = new List<SKText>();
            foreach (Transform child in children)
            {
                SKText t;
                if (child.TryGetComponent<SKText>(out t))
                {
                    texts.Add(t);
                }
            }
            LocalizeObjects(LocalizationType.Text, texts);
        }


        /// <summary>
        /// Find all objects that need to be localized in the scene
        /// </summary>
        private static void UpdateAllLocalizationComponents()
        {
            texts = new List<SKText>(GameObject.FindObjectsOfType<SKText>(true));
            images = new List<SKImage>(GameObject.FindObjectsOfType<SKImage>());
        }
    }
}
