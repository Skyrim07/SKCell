using System;
using System.Collections.Generic;
using UnityEngine;
namespace SKCell
{
    /// <summary>
    /// Stores global variables of game environment
    /// </summary>
    public static class SKEnvironment
    {
        private static SKEnvironmentAsset asset;

        public static LanguageSupport curLanguage;
        public static LanguageSupport defaultLanguage;


        public static void Initialize()
        {
            curLanguage = defaultLanguage;
        }

        public static void SetGameLanguage(LanguageSupport language)
        {
            curLanguage = language;
        }
    }
}