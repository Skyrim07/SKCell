using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SKCell
{
    #region Localization Editor Window

#if UNITY_EDITOR
    /// <summary>
    /// Editor window for SK Localization
    /// </summary>
    [ExecuteInEditMode]
    public class SKLocalizationControlWindow : EditorWindow
    {
        public static string[] textSubpageTitles = { "General" };

        static bool initialized = false;

        static CurrentPageSelection page;
        static string supportedLangDescrip;
        static SKLocalizationAsset asset;
        static Vector2 scrollPos = Vector2.zero;
        static int subPage = 0;
        static List<LocalizedTextConfig> builtTextList = new List<LocalizedTextConfig>();
     

        [MenuItem("Tools/SKCell/Localization Center", priority = 3)]
        public static void Initialize()
        {
            SKLocalizationControlWindow window = GetWindow<SKLocalizationControlWindow>("Localization Center");
            window.minSize = window.maxSize = new Vector2(1400, 600);

            //SKAssetLibrary.LocalizationAsset = null;
            LoadLocalAsset();
            LoadJsonAsset();
            UpdateSupportedLangDescrip();
            page = CurrentPageSelection.MainPage;

            initialized = true;
        }

        private static void LoadLocalAsset()
        {
            asset = SKAssetLibrary.LocalizationAsset;
        }

        private void OnGUI()
        {
            if (!initialized)
                return;
            DrawTopbar();
            DrawCenterContent();
            DrawBottomBar();
        }
        private void OnDestroy()
        {
            initialized = false;
        }
        private static void DrawTopbar()
        {
            if (page != CurrentPageSelection.MainPage && GUI.Button(new Rect(new Vector2(10, 5), new Vector2(80, 20)), "< Back"))
            {
                if (page == CurrentPageSelection.Text)
                    page = CurrentPageSelection.TextSubpage;
                else
                    page = CurrentPageSelection.MainPage;
                scrollPos = Vector2.zero;
            }
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.font = SKAssetLibrary.DefaultFont;
            if (page == CurrentPageSelection.MainPage)
            {
                GUILayout.Label("Localization Control Center");
            }
            if (page == CurrentPageSelection.Language)
            {
                GUILayout.Label("Localization Control Center - Language Support");
            }
            if (page == CurrentPageSelection.Text)
            {
                GUILayout.Label("Localization Control Center - Text Localization");
            }
            if (page == CurrentPageSelection.Image)
            {
                GUILayout.Label("Localization Control Center - Image Localization");
            }
            EditorGUI.DrawRect(new Rect(new Vector2(0, 32), new Vector2(1400, 4)), Color.gray);
            EditorGUI.LabelField(new Rect(new Vector2(0, 45), new Vector2(1400, 14)), $"Current supported languages:  {supportedLangDescrip}");
        }

        private static void DrawCenterContent()
        {
            #region MainPage_Center
            if (page == CurrentPageSelection.MainPage)
            {
                if (asset == null)
                {
                    EditorGUILayout.Space(200);
                    GUI.skin.label.fontSize = 14;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label("No localization asset found. Press 'Create localization asset' to begin.");
                    GUILayout.BeginArea(new Rect(new Vector2(600, 300), new Vector2(1400, 470)));
                    if (GUILayout.Button("Create localization asset", GUILayout.Width(200)))
                    {
                        CreateLocalizationAsset();
                    }
                    GUILayout.EndArea();
                    return;
                }

                EditorGUILayout.Space(150);
                GUILayout.BeginArea(new Rect(new Vector2(400, 200), new Vector2(1400, 470)));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Text Localization", GUILayout.Width(200), GUILayout.Height(70)))
                {
                    page = CurrentPageSelection.TextSubpage;
                }
                if (GUILayout.Button("Image Localization", GUILayout.Width(200), GUILayout.Height(70)))
                {
                    page = CurrentPageSelection.Image;
                }
                if (GUILayout.Button("Language Support", GUILayout.Width(200), GUILayout.Height(70)))
                {
                    page = CurrentPageSelection.Language;
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Font Chart", GUILayout.Width(200), GUILayout.Height(70)))
                {
                    SKFontWindow.Initialize();
                }
                GUILayout.EndArea();
            }

            #endregion

            #region Text_Subpage
            if (page == CurrentPageSelection.TextSubpage)
            {
                EditorGUILayout.Space(0);
                GUILayout.BeginArea(new Rect(new Vector2(400, 200), new Vector2(1800, 470)));
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 18; i++)
                {
                    string s = (i >= textSubpageTitles.Length || textSubpageTitles[i].Length == 0) ? $"Group {i} \r\n {i*1000}-{(i+1)*1000}" : $"{textSubpageTitles[i]}\r\n {i * 1000}-{(i + 1) * 1000}";
                    if (GUILayout.Button(s, GUILayout.Width(200), GUILayout.Height(50)))
                    {
                        subPage = i;
                        BuildTextList();
                        page = CurrentPageSelection.Text;
                    }
                    if (i % 3 == 2)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            #endregion 
            #region Language_Center
            if (page == CurrentPageSelection.Language)
            {
                EditorGUI.DrawRect(new Rect(new Vector2(0, 80), new Vector2(1400, 470)), new Color(0.1f, 0.1f, 0.1f));
                EditorGUI.DrawRect(new Rect(new Vector2(560, 80), new Vector2(3, 470)), new Color(0.2f, 0.2f, 0.2f));
                EditorGUI.LabelField(new Rect(new Vector2(0, 60), new Vector2(1400, 20)), $"Language supports");
                EditorGUILayout.Space(60);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(470));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < Enum.GetNames(typeof(LanguageSupport)).Length; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(-50));
                    if (!asset.languageSupports.Contains((LanguageSupport)i))
                    {
                        EditorGUILayout.LabelField($"Language: {(LanguageSupport)i}", GUILayout.Width(300));
                        if (GUILayout.Button("Add", GUILayout.Width(200)))
                        {
                            AddNewLanguageSupport(i);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Language: {(LanguageSupport)i} (ADDED)", GUILayout.Width(300));
                        EditorGUILayout.LabelField("", GUILayout.Width(300));
                        if (GUILayout.Button("Remove", GUILayout.Width(200)))
                        {
                            DeleteLanguageSupport(i);
                        }
                    }
                    EditorGUILayout.LabelField("", GUILayout.Width(300));
                    EditorGUILayout.LabelField("Font: ");
                    asset.languageFonts[i] = EditorGUILayout.IntField(asset.languageFonts[i]);

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(30);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            #endregion
            #region Text_Center
            if (page == CurrentPageSelection.Text)
            {
                EditorGUI.DrawRect(new Rect(new Vector2(0, 90), new Vector2(1400, 470)), new Color(0.1f, 0.1f, 0.1f));
                EditorGUI.LabelField(new Rect(new Vector2(0, 60), new Vector2(1400, 20)), $"Text localizations: {subPage * 1000}-{(subPage + 1) * 1000}");
                EditorGUI.LabelField(new Rect(new Vector2(1080, 60), new Vector2(1400, 20)), $"Leave any local field blank to apply default values.");
                EditorGUILayout.Space(60);
                if (builtTextList.Count < 1)
                {
                    EditorGUILayout.Space(200);
                    GUI.skin.label.fontSize = 14;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label("No text localization created. Press 'Add new text entry' to begin.");
                    return;
                }
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(470));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < builtTextList.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(-50));
                    EditorGUILayout.LabelField($"Local Text ID:");
                    builtTextList[i].id = EditorGUILayout.IntField(builtTextList[i].id, GUILayout.Width(50));
                    builtTextList[i].descrip = EditorGUILayout.TextField(builtTextList[i].descrip, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();
                    for (int j = 0; j < Enum.GetNames(typeof(LanguageSupport)).Length; j++)
                    {
                        if (asset.languageSupports.Contains((LanguageSupport)j))
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(-150));
                            EditorGUILayout.LabelField($"{(LanguageSupport)j}: ");
                            builtTextList[i].localTexts[j] = EditorGUILayout.TextField(builtTextList[i].localTexts[j], GUILayout.Width(500), GUILayout.Height(20));
                            EditorGUILayout.LabelField($"   Font override: ");
                            builtTextList[i].fontOverrides[j] = EditorGUILayout.IntField(builtTextList[i].fontOverrides[j], GUILayout.Width(50));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(-50));
                    if (GUILayout.Button("Delete", GUILayout.Width(200)))
                    {
                        DeleteTextEntry(builtTextList[i]);
                    }
                    if (GUILayout.Button("Copy", GUILayout.Width(200)))
                    {
                        CopyTextEntry(builtTextList[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(30);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            #endregion
            #region Image_Center
            if (page == CurrentPageSelection.Image)
            {
                EditorGUI.DrawRect(new Rect(new Vector2(0, 90), new Vector2(1400, 470)), new Color(0.1f, 0.1f, 0.1f));
                EditorGUI.LabelField(new Rect(new Vector2(0, 60), new Vector2(1400, 20)), $"Image localizations");
                EditorGUI.LabelField(new Rect(new Vector2(1080, 60), new Vector2(1400, 20)), $"Leave any local field blank to apply default values.");
                EditorGUILayout.Space(60);
                if (asset.imageConfigs.Count < 1)
                {
                    EditorGUILayout.Space(200);
                    GUI.skin.label.fontSize = 14;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label("No image localization created. Press 'Add new image entry' to begin.");
                    return;
                }
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(470));
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < asset.imageConfigs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(-50));
                    EditorGUILayout.LabelField($"Local Image ID:");
                    asset.imageConfigs[i].id = EditorGUILayout.IntField(asset.imageConfigs[i].id, GUILayout.Width(50));
                    asset.imageConfigs[i].descrip = EditorGUILayout.TextField(asset.imageConfigs[i].descrip, GUILayout.Width(150));
                    EditorGUILayout.EndHorizontal();
                    for (int j = 0; j < Enum.GetNames(typeof(LanguageSupport)).Length; j++)
                    {
                        if (asset.languageSupports.Contains((LanguageSupport)j))
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(-150));
                            EditorGUILayout.LabelField($"{(LanguageSupport)j}: ");
                            asset.imageConfigs[i].localImages[j] = (Texture2D)EditorGUILayout.ObjectField(asset.imageConfigs[i].localImages[j], typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(-50));
                    if (GUILayout.Button("Delete", GUILayout.Width(200)))
                    {
                        DeleteImageEntry(i);
                    }
                    if (GUILayout.Button("Copy", GUILayout.Width(200)))
                    {
                        CopyImageEntry(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(30);
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            #endregion
        }

        private static void DrawBottomBar()
        {
            GUILayout.BeginArea(new Rect(new Vector2(20, 570), new Vector2(1400, 530)));
            EditorGUILayout.BeginHorizontal();
            if (page == CurrentPageSelection.Text)
            {
                if (GUILayout.Button("Add New Text Entry", GUILayout.Width(200)))
                {
                    AddNewTextEntry();
                }
            }
            if (page == CurrentPageSelection.Image)
            {
                if (GUILayout.Button("Add New Image Entry", GUILayout.Width(200)))
                {
                    AddNewImageEntry();
                }
            }
            if (page == CurrentPageSelection.Language)
            {

            }
            if (asset != null && GUILayout.Button("Save asset file", GUILayout.Width(200)))
            {
                SaveJsonAsset();
            }
            if (GUILayout.Button("Load asset file", GUILayout.Width(200)))
            {
                LoadJsonAsset();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUI.skin.label.fontSize = 15;
            GUI.Label(new Rect(new Vector2(1290, 575), new Vector2(150, 20)), "SKCell");
        }

        private static void SaveJsonAsset()
        {
            SKUtils.SKSaveObjectToJson(new SKLocalizationAssetJson(asset), "SKLocalizationAsset.json");
        }

        private static void LoadJsonAsset()
        {
            asset.UpdateInfo(SKUtils.SKLoadObjectFromJson<SKLocalizationAssetJson>("SKLocalizationAsset.json"));
            UpdateSupportedLangDescrip();
        }
        #region Minor Utilities
        private static void AddNewTextEntry()
        {
            asset.textConfigs.Add(new LocalizedTextConfig());
            asset.textConfigs[asset.textConfigs.Count - 1].id = builtTextList.Count == 0 ? subPage * 1000 : builtTextList[builtTextList.Count - 1].id + 1;
            BuildTextList();
            LoadLocalAsset();
        }
        private static void DeleteTextEntry(LocalizedTextConfig text)
        {
            asset.textConfigs.Remove(text);
            BuildTextList();
            LoadLocalAsset();
        }
        private static void DeleteTextEntry(int index)
        {
            asset.textConfigs.RemoveAt(index);

            LoadLocalAsset();
        }
        private static void CopyTextEntry(LocalizedTextConfig text)
        {
            asset.textConfigs.Add(new LocalizedTextConfig(text));
            BuildTextList();
            LoadLocalAsset();
        }
        private static void CopyTextEntry(int index)
        {
            asset.textConfigs.Add(new LocalizedTextConfig(asset.textConfigs[index]));

            LoadLocalAsset();
        }
        private static void AddNewImageEntry()
        {
            asset.imageConfigs.Add(new LocalizedImageConfig());

            LoadLocalAsset();
        }

        private static void DeleteImageEntry(int index)
        {
            asset.imageConfigs.RemoveAt(index);

            LoadLocalAsset();
        }

        private static void CopyImageEntry(int index)
        {
            asset.imageConfigs.Add(new LocalizedImageConfig(asset.imageConfigs[index]));

            LoadLocalAsset();
        }

        private static void AddNewLanguageSupport(int index)
        {
            asset.languageSupports.Add((LanguageSupport)index);
            UpdateSupportedLangDescrip();
            LoadLocalAsset();
        }
        private static void DeleteLanguageSupport(int index)
        {
            asset.languageSupports.Remove((LanguageSupport)index);
            UpdateSupportedLangDescrip();
            LoadLocalAsset();
        }
        #endregion
        private static void UpdateSupportedLangDescrip()
        {
            if (asset == null)
            {
                SKUtils.EditorLogWarning("No localization asset found!");
                return;
            }
            string s = "";
            if (asset.languageSupports.Count == 0)
            {
                supportedLangDescrip = "None";
                return;
            }
            int count = 0;
            foreach (var item in asset.languageSupports)
            {
                s += $"    {count++}: {item}";
            }
            supportedLangDescrip = s;
        }

        private static void CreateLocalizationAsset()
        {
            AssetDatabase.CreateAsset(new SKLocalizationAsset(), SKAssetLibrary.LOCAL_ASSET_PATH);
            LoadLocalAsset();
            asset.Initialize();
            UpdateSupportedLangDescrip();
        }

        private static void BuildTextList()
        {
            builtTextList.Clear();
            for (int i = 0; i < asset.textConfigs.Count; i++)
            {
                if (asset.textConfigs[i].id >= (subPage + 1) * 1000 || asset.textConfigs[i].id < (subPage) * 1000)
                    continue;
                builtTextList.Add(asset.textConfigs[i]);
            }
        }
        private enum CurrentPageSelection
        {
            MainPage,
            Text,
            Image,
            Language,
            TextSubpage
        }
    }
#endif
    /// <summary>
    /// Class used for json serialization
    /// </summary>
    [Serializable]
    public class SKLocalizationAssetJson
    {
        public List<LocalizedTextConfig> textConfigs;
        public List<LocalizedImageConfig> imageConfigs;
        public List<LanguageSupport> languageSupports;
        public int[] languageFonts;
        public SKLocalizationAssetJson(SKLocalizationAsset data)
        {
            this.textConfigs = data.textConfigs;
            this.imageConfigs = data.imageConfigs;
            this.languageSupports = data.languageSupports;
            this.languageFonts = data.languageFonts;
        }
    }
    public enum LocalizationMethod
    {
        Responsive, //Refresh each time ApplyLocalization is called
        None, //No localization applied
    }
    public enum LocalizationType
    {
        Text,
        Image,
        Other
    }
    public enum LanguageSupport
    {
        English,
        SimplifiedChinese,
        TraditionalChinese,
        Japanese,
        Korean,
        French,
        Spanish,
        Italian,
        Russian,
        German,
        Dutch,
        Thai,
        Swedish,
        Custom
    }

    public enum FontLibrary
    {
        None = -1,
        Default = 0,
    }

    [Serializable]
    public class LocalizedTextConfig
    {
        public int id;
        public string descrip;
        public List<string> localTexts = new List<string>().PopulateList(string.Empty, 15);
        public List<int> fontOverrides = new List<int>().PopulateList(-1, 15);

        public LocalizedTextConfig()
        {
            id = -1;
            descrip = "/";
            localTexts = new List<string>().PopulateList(string.Empty, 15);
            fontOverrides = new List<int>().PopulateList(-1, 15);
        }
        public LocalizedTextConfig(LocalizedTextConfig data)
        {
            id = data.id;
            descrip = data.descrip;
            localTexts = new List<string>(data.localTexts);
            fontOverrides = new List<int>(data.fontOverrides);
        }
    }
    [Serializable]
    public class LocalizedImageConfig
    {
        public int id;
        public string descrip;
        public List<Texture2D> localImages = new List<Texture2D>().PopulateList(null, 15);

        public LocalizedImageConfig()
        {
            id = -1;
            descrip = "/";
            localImages = new List<Texture2D>().PopulateList(null, 15);
        }

        public LocalizedImageConfig(LocalizedImageConfig data)
        {
            id = data.id;
            descrip = data.descrip;
            localImages = new List<Texture2D>(data.localImages);
        }
    }
    [Serializable]
    public class LocalizedInfo
    {
        public int id;
        public LanguageSupport curLanguage;
        public LocalizationMethod localizationMethod;

        public string localText;
        public int fontOverride;

        public Texture2D localImage;

        public LocalizedInfo()
        {

        }

        public LocalizedInfo(LanguageSupport language, LocalizationMethod method, LocalizationType type, LocalizedTextConfig textConfig = null, LocalizedImageConfig imageConfig = null)
        {
            if (textConfig == null && imageConfig == null)
            {
                SKUtils.EditorLogError("LocalizedInfo Initialization Error: All configs are null.");
                return;
            }
            curLanguage = language;
            localizationMethod = method;
            switch (type)
            {
                case LocalizationType.Text:
                    id = textConfig.id;
                    localText = textConfig.localTexts[(int)language];
                    fontOverride = textConfig.fontOverrides[(int)language];
                    break;
                case LocalizationType.Image:
                    id = imageConfig.id;
                    localImage = imageConfig.localImages[(int)language];
                    break;
            }
        }
        public void UpdateInfo(LanguageSupport language, LocalizationMethod method, LocalizationType type, LocalizedTextConfig textConfig = null, LocalizedImageConfig imageConfig = null)
        {
            if (textConfig == null && imageConfig == null)
            {
                SKUtils.EditorLogError("LocalizedInfo Initialization Error: All configs are null.");
                return;
            }
            curLanguage = language;
            localizationMethod = method;
            switch (type)
            {
                case LocalizationType.Text:
                    id = textConfig.id;
                    localText = textConfig.localTexts[(int)language].Replace(@"\r\n", "\r\n");
                    fontOverride = textConfig.fontOverrides[(int)language];
                    break;
                case LocalizationType.Image:
                    id = imageConfig.id;
                    localImage = imageConfig.localImages[(int)language];
                    break;
            }
        }
    }
}
#endregion
