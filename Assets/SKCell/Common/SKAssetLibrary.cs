using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
    /// <summary>
    /// Store references to various SK assets
    /// </summary>
    public static class SKAssetLibrary
    {
        public const string INVENTORY_ASSET_PATH = "Assets/SKCell/Resources/SKCell/SKInventoryAsset.asset";
        public const string LOCAL_ASSET_PATH = "Assets/SKCell/Resources/SKCell/SKLocalizationConfigAsset.asset";
        public const string FONT_ASSET_PATH = "Assets/SKCell/Resources/SKCell/SKFontAsset.asset";
        public const string TEXTURE_ASSET_PATH = "Assets/SKCell/Sprites/";
        public const string DEFAULT_FONT_PATH = "Assets/SKCell/Font/SK_Default_Font.ttf";
        public const string ENV_ASSET_PATH = "Assets/SKCell/Resources/SKCell/SKEnvironmentAsset.asset";
        public const string UI_ANIM_DIR_PATH = "Assets/SKCell/Resources/SKCell/Animations";
        public const string UI_ANIM_PRESET_DIR_PATH = "SKCell/Animations/Presets";
        public const string MATERIAL_PATH = "Assets/SKCell/Resources/SKCell/Materials";

        public const string PREFAB_PATH = "Assets/SKCell/Resources/SKCell/Prefabs";
        public const string RESOURCES_PREFAB_PATH = "SKCell/Prefabs";
        public const string RESOURCES_JSON_PATH_SUFFIX = "/SKCell//Resources/SKCell/Json/";
        public const string PANEL_PREFAB_PATH = "/Resources/SKCell/UI/Panels";

        public const string JSON_PATH_SUFFIX = "/Json/";
        public const string RES_JSON_PATH_SUFFIX = "Assets/SKCell/Resources/SKCell/Json/";

        public const string RES_SPRITE_PATH = "SKCell/Sprites/";

        private static Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

        public static void ClearTextureCache()
        {
            textureCache.Clear();
        }

        public static void SaveJsonFile(object obj, string fileName)
        {
            string js = JsonUtility.ToJson(obj);
            string path = RES_JSON_PATH_SUFFIX + fileName +".txt";
            File.WriteAllText(path, js);
        }

        public static T LoadJsonFile<T>(string fileName)
        {
            return JsonUtility.FromJson<T>(RES_JSON_PATH_SUFFIX + fileName + ".txt");
        }

        public static Sprite LoadSprite(string fileName)
        {
            return Resources.Load<Sprite>(RES_SPRITE_PATH + fileName);
        }
        public static Texture LoadTexture(string fileName)
        {
            if(textureCache.ContainsKey(fileName))
                return textureCache[fileName];
            textureCache.Add(fileName, Resources.Load<Texture>(RES_SPRITE_PATH + fileName));
            return textureCache[fileName];
        }

        private static SKLocalizationAsset localizationAsset;
        public static SKLocalizationAsset LocalizationAsset
        {
            get
            {
#if UNITY_EDITOR
                if (localizationAsset == null)
                    localizationAsset = AssetDatabase.LoadAssetAtPath<SKLocalizationAsset>(LOCAL_ASSET_PATH);
#endif
                if (localizationAsset == null)
                    localizationAsset = new SKLocalizationAsset(SKUtils.SKLoadObjectFromJson<SKLocalizationAssetJson>("SKLocalizationAsset.json"));

                return localizationAsset;
            }
            set
            {
                localizationAsset = value;
            }
        }

        private static SKInventoryAsset inventoryAsset;
        public static SKInventoryAsset InventoryAsset
        {
            get
            {
                if (inventoryAsset == null)
                {
                    inventoryAsset = Resources.Load<SKInventoryAsset>("SKCell/SKInventoryAsset");
                    if (inventoryAsset == null)
                    {
                        inventoryAsset = ScriptableObject.CreateInstance<SKInventoryAsset>();

#if UNITY_EDITOR
                        string assetPath = "Assets/SKCell/Resources/SKCell/SKInventoryAsset.asset";
                        AssetDatabase.CreateAsset(inventoryAsset, assetPath);
                        AssetDatabase.SaveAssets();
#endif
                    }
                }
                return inventoryAsset;
            }
        }

        private static SKFontAsset fontAsset;
        public static SKFontAsset FontAsset
        {
            get
            {
                //if (fontAsset == null)
                fontAsset = Resources.Load<SKFontAsset>("SKCell/SKFontAsset");
                if(fontAsset!=null)
                    return fontAsset;
#if UNITY_EDITOR
                if (fontAsset == null)
                    fontAsset = AssetDatabase.LoadAssetAtPath<SKFontAsset>(FONT_ASSET_PATH);
                return fontAsset;
#endif
                return null;
            }
        }
        private static Font defaultFont;
        public static Font DefaultFont
        {
            get
            {
#if UNITY_EDITOR
                if (defaultFont == null)
                    defaultFont = AssetDatabase.LoadAssetAtPath<Font>(DEFAULT_FONT_PATH);
#endif
                return defaultFont;
            }
        }
        private static Texture texture_A;
        public static Texture Texture_A
        {
            get 
            { 
#if UNITY_EDITOR
                if (texture_A == null)
                    texture_A = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH+"/A.png");
#endif
                return texture_A;
            }
        }
        private static Texture texture_Transparent;
        public static Texture Texture_Transparent
        {
            get
            {
#if UNITY_EDITOR
                if (texture_Transparent == null)
                    texture_Transparent = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/Transparent.png");
#endif
                return texture_Transparent;
            }
        }
        private static Texture texture_Logo;
        public static Texture Texture_Logo
        {
            get
            {
#if UNITY_EDITOR
                if (texture_Logo == null)
                    texture_Logo = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/SKCell_Logo.png");
#endif
                return texture_Logo;
            }
        }
        private static Texture texture_solid_circle;
        public static Texture Texture_Solid_Circle
        {
            get
            {
#if UNITY_EDITOR
                if (texture_solid_circle == null)
                    texture_solid_circle = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/solid.png");
#endif
                return texture_solid_circle;
            }
        }
        private static Texture texture_smooth_circle;
        public static Texture Texture_Smooth_Circle
        {
            get
            {
#if UNITY_EDITOR
                if (texture_smooth_circle == null)
                    texture_smooth_circle = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/smooth.png");
#endif
                return texture_smooth_circle;
            }
        }
        private static Texture texture_alpha_circle;
        public static Texture Texture_Alpha_Circle
        {
            get
            {
#if UNITY_EDITOR
                if (texture_alpha_circle == null)
                    texture_alpha_circle = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/alpha_circle.png");
#endif
                return texture_alpha_circle;
            }
        }
        private static Texture texture_Conv_Bubble;
        public static Texture Texture_Conv_Bubble
        {
            get
            {
#if UNITY_EDITOR
                if (texture_Conv_Bubble == null)
                    texture_Conv_Bubble = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/convbubble.png");
#endif
                return texture_Conv_Bubble;
            }
        }
        private static Texture texture_Random;
        public static Texture Texture_Random
        {
            get
            {
#if UNITY_EDITOR
                if (texture_Random == null)
                    texture_Random = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/rand.png");
#endif
                return texture_Random;
            }
        }
        private static Texture texture_ArrowDown;
        public static Texture Texture_ArrowDown
        {
            get
            {
#if UNITY_EDITOR
                if (texture_ArrowDown == null)
                    texture_ArrowDown = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/ArrowDown.png");
#endif
                return texture_ArrowDown;
            }
        }
        private static Texture texture_CircArrow;
        public static Texture Texture_CircArrow
        {
            get
            {
#if UNITY_EDITOR
                if (texture_CircArrow == null)
                    texture_CircArrow = AssetDatabase.LoadAssetAtPath<Texture>(TEXTURE_ASSET_PATH + "/circarrow.png");
#endif
                return texture_CircArrow;
            }
        }
        private static SKEnvironmentAsset envAsset;
        public static SKEnvironmentAsset EnvAsset
        {
            get
            {
#if UNITY_EDITOR
                if (envAsset == null)
                    envAsset = AssetDatabase.LoadAssetAtPath<SKEnvironmentAsset>(ENV_ASSET_PATH);
#endif
                if (envAsset == null)
                    envAsset = Resources.Load<SKEnvironmentAsset>(ENV_ASSET_PATH.Substring(ENV_ASSET_PATH.IndexOf("SKCell")));
                return envAsset;
            }
        }
        private static Material spriteEditor_mat;
        public static Material SpriteEditorMat
        {
            get
            {
#if UNITY_EDITOR
                if (spriteEditor_mat == null)
                    spriteEditor_mat = AssetDatabase.LoadAssetAtPath<Material>(TEXTURE_ASSET_PATH + "/SpriteEditor.mat");
#endif
                return spriteEditor_mat;
            }
        }
        private static Material gridCellMat;
        public static Material GridCellMat
        {
            get
            {
#if UNITY_EDITOR
                if (gridCellMat == null)
                    gridCellMat = AssetDatabase.LoadAssetAtPath<Material>(MATERIAL_PATH+"/GridCellMat.mat");
#endif
                if (gridCellMat == null)
                    gridCellMat = Resources.Load<Material>(MATERIAL_PATH.Substring(MATERIAL_PATH.IndexOf("SKCell")) + "/GridCellMat");
                return gridCellMat;
            }
        }

        public static void Initialize()
        {
           // localizationAsset = null;
          //  fontAsset=null;
           // envAsset = null;
        }
    }
}
