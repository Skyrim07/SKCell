using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace SKCell
{
#if UNITY_EDITOR
    public class SKInventoryControlCenter : EditorWindow
    {
        const int WINDOW_WIDTH = 1400;
        const int WINDOW_HEIGHT = 600;

        static SKInventoryAsset asset;

        static bool initialized = false;

        static RootPage rootPage;
        static int item_selectedCategory = 0;
        static SKInventoryItemData item_SelectedItem;
        static LanguageSupport previewLanguage = LanguageSupport.English;

        public static GUIStyle itemButtonStyle, blackButtonStyle;
        public static GUIStyle textFieldStyle;
        public static GUIContent crossContent;
        public static Texture2D normalTexture, hoverTexture,selectedTexture, itemFrameTexture;

        [MenuItem("Tools/SKCell/Inventory Center", priority = 1)]
        public static void Initialize()
        {
            SKInventoryControlCenter window = GetWindow<SKInventoryControlCenter>("Inventory Center");
            window.minSize = window.maxSize = new Vector2(1400, 600);

            LoadAsset();
            rootPage = RootPage.Item;

            InitializeStyles(); 

            item_selectedCategory = 0;
            item_SelectedItem = null;
            initialized = true;
        }

        private static void InitializeStyles()
        {
            crossContent = new GUIContent(SKAssetLibrary.LoadTexture($"cross_1"));

            textFieldStyle = new GUIStyle(EditorStyles.textField);
            textFieldStyle.wordWrap = true;
            itemButtonStyle = new GUIStyle();
            blackButtonStyle = new GUIStyle();

            normalTexture = new Texture2D(1, 1);
            normalTexture.SetPixel(0, 0, Color.clear);
            normalTexture.Apply();

            itemFrameTexture = new Texture2D(1, 1);
            itemFrameTexture.SetPixel(0, 0, new Color(1, 1, 1, 0.1f));
            itemFrameTexture.Apply();

            hoverTexture = new Texture2D(1, 1);
            hoverTexture.SetPixel(0, 0, new Color(1, 1, 1, 0.2f));
            hoverTexture.Apply();

            selectedTexture = new Texture2D(1, 1);
            selectedTexture.SetPixel(0, 0, new Color(1,1,1, 0.3f));
            selectedTexture.Apply();

            itemButtonStyle.normal.background = itemFrameTexture;
            itemButtonStyle.normal.textColor = Color.white;
            itemButtonStyle.alignment = TextAnchor.MiddleCenter;
            itemButtonStyle.hover.background = hoverTexture;
            itemButtonStyle.hover.textColor = Color.white;

            normalTexture = new Texture2D(1, 1);
            normalTexture.SetPixel(0, 0, new Color(.1f, .1f, .1f, 0.5f));
            normalTexture.Apply();

            hoverTexture = new Texture2D(1, 1);
            hoverTexture.SetPixel(0, 0, new Color(.25f, .25f, .25f, 0.9f));
            hoverTexture.Apply();

            blackButtonStyle.normal.background = normalTexture;
            blackButtonStyle.normal.textColor = Color.white;
            blackButtonStyle.hover.background = hoverTexture;
            blackButtonStyle.hover.textColor = new Color(.9f,.8f,.7f);
            blackButtonStyle.font = SKAssetLibrary.DefaultFont;
            blackButtonStyle.alignment = TextAnchor.MiddleCenter;
            blackButtonStyle.fontSize = 12;
        }
        private void OnGUI()
        {
            DrawTopbar();
            DrawRootMenu();
            DrawSecondaryMenu();
            DrawCenter();
            Repaint();
        }
        private void OnDestroy()
        {
            initialized = false;
        }

        #region Window Drawing
        const int ROOT_MENU_WIDTH = 150;
        const int TOP_BAR_HEIGHT = 34;
        private static void DrawTopbar()
        {
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.font = SKAssetLibrary.DefaultFont;
            GUILayout.Label("SKCell - Inventory Center");
            EditorGUI.DrawRect(new Rect(new Vector2(0, 32), new Vector2(1400, 2)), Color.grey);
            EditorGUI.DrawRect(new Rect(new Vector2(0, TOP_BAR_HEIGHT), new Vector2(1400, 1000)), new Color(.1f,.1f,.1f));

            if (GUI.Button(new Rect(WINDOW_WIDTH - 160, 5, 150, 20), "Save Asset"))
            {
                SaveAsset();
            }
        }

        static string[] rootMenuNames = new string[] { "Inventory Items", "Categories", "Settings" };
        private static void DrawRootMenu()
        {
            GUILayout.BeginArea(new Rect(new Vector2(0, TOP_BAR_HEIGHT), new Vector2(ROOT_MENU_WIDTH, 600)));
            EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(ROOT_MENU_WIDTH, 600)), new Color(0,0,0, .5f));

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < rootMenuNames.Length; i++)
            {
                if (rootPage == (RootPage)i)
                {
                    blackButtonStyle.normal.background = selectedTexture;

                }
                if (GUILayout.Button(rootMenuNames[i], blackButtonStyle, GUILayout.Height(50), GUILayout.Width(150)))
                {
                    rootPage = (RootPage)i;
                }
                if (rootPage == (RootPage)i)
                {
                    blackButtonStyle.normal.background = normalTexture;
                    EditorGUI.DrawRect(new Rect(new Vector2(0, 50*i), new Vector2(5, 50)),
        new Color(1, 1, 1, .8f));
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private static void DrawSecondaryMenu()
        {
            GUILayout.BeginArea(new Rect(new Vector2(ROOT_MENU_WIDTH, TOP_BAR_HEIGHT), 
                new Vector2(150, 600)));

            if (rootPage == RootPage.Item)
            {
                EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(ROOT_MENU_WIDTH, 600)), new Color(0, 0, 0, .2f));
                EditorGUILayout.BeginVertical();

                int drawnCount = 0;
                for (int i = 0; i < asset.categoryData.categoryNames.Length; i++)
                {
                    if (!asset.categoryData.categoryIsActive[i])
                        continue;
                    string cname = (asset.categoryData.categoryNames[i]!=null &&
                       asset.categoryData.categoryNames[i].Length>0) ? asset.categoryData.categoryNames[i]: $"Category {i+1}";
                    if (asset.categoryData.categoryLocalIDs[i] >= 0)
                        cname = SKLocalization.GetLocalizationText(asset.categoryData.categoryLocalIDs[i], previewLanguage);
                    if (item_selectedCategory == i)
                    {
                        blackButtonStyle.normal.background = selectedTexture;
                    }
                    if (GUILayout.Button(cname, blackButtonStyle, GUILayout.Height(25)))
                    {
                        item_selectedCategory = i;
                    }
                    if (item_selectedCategory == i)
                    {
                        blackButtonStyle.normal.background = normalTexture;
                        EditorGUI.DrawRect(new Rect(new Vector2(0, 25 * drawnCount), new Vector2(5, 25)),
new Color(1, 1, 1, .5f));
                    }
                    drawnCount++;
                }
                EditorGUILayout.Space(5);
                EditorGUILayout.EndVertical();
            }
            else if (rootPage == RootPage.Category)
            {

            }

            GUILayout.EndArea();
        }

        static Vector2 scrollPos;
        static float iconWidth = 30f;
        static float iconHeight = 30f;
        static float horizontalSpacing = 5f;
        private static void DrawCenter()
        {
            if(rootPage == RootPage.Item)
            {
                int sectionWidth = 500;
                GUILayout.BeginArea(new Rect(new Vector2(ROOT_MENU_WIDTH * 2, TOP_BAR_HEIGHT),
                new Vector2(sectionWidth, 600)));
                EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(1, 600)), new Color(1,1,1, .4f)); //left line
                EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(sectionWidth, 600)), new Color(0, 0, 0, .2f)); //center fill
                EditorGUI.DrawRect(new Rect(new Vector2(sectionWidth-1, 0), new Vector2(1, 600)), new Color(1, 1, 1, .4f)); //right line

                List<SKInventoryItemData> categoryItems = new List<SKInventoryItemData>();
                foreach (var item in asset.itemData)
                {
                    if (item.category == item_selectedCategory)
                        categoryItems.Add(item);
                }
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                if (GUILayout.Button("Add new item...", itemButtonStyle,GUILayout.Width(100), GUILayout.Height(20)))
                {
                    SKInventoryItemData data = new SKInventoryItemData();
                    data.id = (item_selectedCategory+1) * 1000 + categoryItems.Count;
                    data.name = "New Item";
                    data.category = item_selectedCategory;
                    asset.itemData.Add(data);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUI.skin.label.fontSize = 12;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(600 - TOP_BAR_HEIGHT - 20));
                EditorGUILayout.BeginVertical();
    


                for (int i = 0; i < categoryItems.Count/2+1; i++)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < 2; j++)
                    {
                        if (categoryItems.Count <= i * 2 + j)
                            break;
                        SKInventoryItemData itemData = categoryItems[i * 2 + j];
                        if(item_SelectedItem == itemData)
                        {
                            itemButtonStyle.normal.background = selectedTexture;
                        }
                        if(GUILayout.Button("", itemButtonStyle, GUILayout.Width(sectionWidth/2-20f), GUILayout.Height(40)))
                        {
                            item_SelectedItem = itemData;
                        }
                        if (item_SelectedItem == itemData)
                        {
                            itemButtonStyle.normal.background = itemFrameTexture;
                        }
                        GUILayout.Space(-(sectionWidth / 2 - 20f) +40);
                        EditorGUILayout.BeginVertical();
                        GUILayout.Label(itemData.name_LocalID < 0 ? itemData.name : SKLocalization.GetLocalizationText(itemData.name_LocalID, previewLanguage),
                            GUILayout.Width(180));
                        GUILayout.Label($"ID: {itemData.id}");
                        EditorGUILayout.EndVertical();

                        float x = (sectionWidth / 2) * j  - horizontalSpacing + 15;
                        float y = (40 + 5) * i +10;
                        Rect centerRect = new Rect(x, y, iconWidth, iconHeight);
                        Texture2D toDisplay = itemData.icon == null ? (Texture2D)SKAssetLibrary.Texture_Transparent : itemData.icon;
                        EditorGUI.DrawTextureTransparent(centerRect, toDisplay, ScaleMode.ScaleToFit);

                        EditorGUILayout.Space(10f);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();

                GUILayout.EndArea();


                //Details Panel
                GUILayout.BeginArea(new Rect(new Vector2(ROOT_MENU_WIDTH * 2+sectionWidth +10, TOP_BAR_HEIGHT),
                 new Vector2(WINDOW_WIDTH- sectionWidth - ROOT_MENU_WIDTH * 2, 600)));

                sectionWidth = WINDOW_WIDTH - sectionWidth;
                if(item_SelectedItem == null)
                {
                    GUI.Label(new Rect(sectionWidth / 2, WINDOW_HEIGHT / 2, sectionWidth, WINDOW_HEIGHT), "Select an item to edit.");
                }
                else
                {
                    SKInventoryItemData displayItem = item_SelectedItem;

                    GUI.skin.label.fontSize = 16;
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                    string displayName = displayItem.name_LocalID < 0 ? displayItem.name : SKLocalization.GetLocalizationText(displayItem.name_LocalID,previewLanguage);
                    GUI.Label(new Rect(0,0, sectionWidth / 2, 50), displayName);

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.Space(50);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("ID: ", GUILayout.Width(150));
                    displayItem.id = EditorGUILayout.IntField(displayItem.id, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Icon: ", GUILayout.Width(150));

                    displayItem.icon = (Texture2D)EditorGUILayout.ObjectField(displayItem.icon, typeof(Texture2D), false, GUILayout.Height(100), GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("To enable localized texts, enter a non-negative local ID.");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Text: ", GUILayout.Width(150));
                    displayItem.name = EditorGUILayout.TextField(displayItem.name, GUILayout.Width(400));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name Local ID: ", GUILayout.Width(150));
                    displayItem.name_LocalID = EditorGUILayout.IntField(displayItem.name_LocalID, GUILayout.Width(400));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Description Text: ", GUILayout.Width(150));
                    displayItem.description = EditorGUILayout.TextField(displayItem.description, textFieldStyle, GUILayout.Width(400), GUILayout.Height(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Description Local ID: ", GUILayout.Width(150));
                    displayItem.descrip_LocalID = EditorGUILayout.IntField(displayItem.descrip_LocalID, GUILayout.Width(400));
                    EditorGUILayout.EndHorizontal();

                    if (displayItem.descrip_LocalID >= 0)
                    {
                        EditorGUILayout.LabelField(SKLocalization.GetLocalizationText(displayItem.descrip_LocalID, previewLanguage), textFieldStyle, GUILayout.Width(400), GUILayout.Height(100));
                    }

                    EditorGUILayout.Space(10);
                    GUIStyle redTextStyle = new GUIStyle(GUI.skin.button);
                    redTextStyle.normal.textColor = new Color(.9f, 0.8f, 0.7f, 1f);
                    redTextStyle.fontStyle = FontStyle.Bold;
                    if (GUILayout.Button("Delete Item", redTextStyle,GUILayout.Width(100)))
                    {
                        asset.itemData.Remove(displayItem);
                        categoryItems.Remove(displayItem);
                        if (item_SelectedItem == displayItem)
                            item_SelectedItem = null;

                        EditorGUILayout.EndVertical();
                        GUILayout.EndArea();
                        return;
                    }

                    EditorGUILayout.EndVertical();
                }

                GUILayout.EndArea();
            }
            else if (rootPage == RootPage.Category)
            {
                GUILayout.BeginArea(new Rect(new Vector2(ROOT_MENU_WIDTH+20, TOP_BAR_HEIGHT+20),
                     new Vector2(WINDOW_WIDTH - ROOT_MENU_WIDTH, 600)));

                EditorGUILayout.BeginVertical();
    
                EditorGUILayout.LabelField("To enable localized texts, enter a non-negative local ID.");
                EditorGUILayout.Space();
                for (int i = 0; i < asset.categoryData.categoryNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{i + 1}: ", GUILayout.Width(50));
                    if (asset.categoryData.categoryLocalIDs[i] >= 0)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField(
                        SKLocalization.GetLocalizationText(asset.categoryData.categoryLocalIDs[i], previewLanguage), GUILayout.Width(300));
                        GUI.enabled = true;
                    }
                    else
                    {
                        asset.categoryData.categoryNames[i] = EditorGUILayout.TextField(asset.categoryData.categoryNames[i], GUILayout.Width(300));
                    }


                    EditorGUILayout.LabelField($"Local ID: ", GUILayout.Width(70));
                    asset.categoryData.categoryLocalIDs[i] = EditorGUILayout.IntField(asset.categoryData.categoryLocalIDs[i], GUILayout.Width(100));
                    if (i == 0)
                        GUI.enabled = false;
                    EditorGUILayout.LabelField($"Is Active: ", GUILayout.Width(80));
                        asset.categoryData.categoryIsActive[i] = EditorGUILayout.Toggle(asset.categoryData.categoryIsActive[i]);
                    if(i==0)
                        GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
                GUILayout.EndArea();
            }
            else if (rootPage == RootPage.Settings) 
            {
                GUILayout.BeginArea(new Rect(new Vector2(ROOT_MENU_WIDTH + 20, TOP_BAR_HEIGHT + 20),
     new Vector2(WINDOW_WIDTH - ROOT_MENU_WIDTH, 600)));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preview language for localized texts: ", GUILayout.Width(250));
                previewLanguage = (LanguageSupport)EditorGUILayout.EnumPopup(previewLanguage, GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();

                GUILayout.EndArea();
            }
        }

        #endregion
        private static void LoadAsset()
        {
            asset = SKAssetLibrary.InventoryAsset;
        }
        private static void SaveAsset()
        {
#if UNITY_EDITOR
            if (asset != null)
            {
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
        }

        private enum RootPage
        {
            Item,
            Category,
            Settings
        }
    }

#endif

    /// <summary>
    /// Class used for json serialization
    /// </summary>
    [Serializable]
    public class SKInventoryAssetJson
    {
        public List<SKInventoryItemData> itemData;
        public SKInventoryCategoryData categoryData;
        public SKInventoryAssetJson(SKInventoryAsset data)
        {
            this.itemData = data.itemData;
            this.categoryData = data.categoryData;
        }
    }

    [Serializable]
    public class SKInventoryItemData
    {
        public int id;

        public int name_LocalID, descrip_LocalID;
        public string name, description;

        public Texture2D icon;
        public int category;

        public Vector2Int slotSize = Vector2Int.one;

        public bool canUse;
        public Action onUse;

        public SKInventoryItemData()
        {
            id = -1;
            name_LocalID = -1;
            descrip_LocalID = -1;
        }
        public SKInventoryItemData(SKInventoryItemData data)
        {
            id = data.id;
            name_LocalID = data.name_LocalID;   
            descrip_LocalID = data.descrip_LocalID;
            name = data.name;
            description = data.description;
            icon = data.icon;
            category = data.category;
            slotSize = data.slotSize;
        }
    }

    [Serializable]
    public class SKInventoryCategoryData
    {
        public string[] categoryNames;
        public int[] categoryLocalIDs;
        public bool[] categoryIsActive;

        public SKInventoryCategoryData()
        {
            categoryLocalIDs = new int[25];
            SKUtils.FillArray(categoryLocalIDs, -1);
            categoryNames = new string[25];
            categoryNames[0] = "General";

            categoryIsActive = new bool[25];
            categoryIsActive[0] = true;
        }
    }
}