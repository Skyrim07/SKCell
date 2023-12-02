using System.IO;
using UnityEngine;
using UnityEditor;
namespace SKCell {
    public class TexturePostProcessor : AssetPostprocessor
    {

    }

    public class SKSpriteEditor : EditorWindow
    {
        static EditorWindow window;

        const int TOP_BAR_HEIGHT = 40;
        const int LEFT_COL_WIDTH = 180;
        const int CENTER_WIDTH = 800;
        Color COLOR_GOLD = new Color(.9f,.8f,.7f);
        Color COLOR_GOLD_BRIGHT = new Color(.9f,.8f,.7f)*1.8f;
        /// <summary>
        /// Texture to work with
        /// </summary>
        private Texture2D texture, prevTetxure;
        private Texture2D otex;
        private string texturePath;

        private static EditMode mode = EditMode.Select;
        private static Brush brush;

        private TextureImporter importer;
        private SpriteProperties properties, prevProperties;

        private Color colorToErase = Color.white;
        private float eraseTolerance = 0.5f;

        private bool isMouseInImage = false;
        private int updateCounter = 0;
        private Vector2 selectStartPos, selectEndPos,selectStartPosPixel;
        private Rect selectedPixels;
        private bool areaSelected;

        private static GUIContent selectButtonCnt, brushButtonCnt, eraserButtonCnt, pickerButtonCnt;
        private static GUIContent alphaCircleCnt, alphaCircleSmoothCnt, aimCnt;
        private static string toolString = "Select";
        private static Color oBackgroundColor, oContentColor;
        private static Vector2 oBrushPos;
        private static EditMode oMode;

        private static LinearBlur linearBlur; 

        private Vector2Int newImageSize = new Vector2Int(128,128);
        private Color newImageColor;

        [MenuItem("Tools/SKCell/Sprite Editor",priority =0)]
        static void Init()
        {
            window = GetWindow(typeof(SKSpriteEditor));
            window.maxSize = window.minSize = new Vector2(1280, 720);
            window.Show(); 
             
            //if(brush==null)
            brush = new Brush();
            linearBlur = new LinearBlur();
            selectButtonCnt = new GUIContent(SKAssetLibrary.LoadTexture("cursor"));
            brushButtonCnt = new GUIContent(SKAssetLibrary.LoadTexture("brush"));
            eraserButtonCnt = new GUIContent(SKAssetLibrary.LoadTexture("eraser"));
            pickerButtonCnt = new GUIContent(SKAssetLibrary.LoadTexture("picker"));
            aimCnt = new GUIContent(SKAssetLibrary.LoadTexture("aim"));
            alphaCircleCnt = new GUIContent(SKAssetLibrary.LoadTexture("alpha_circle"));
            alphaCircleSmoothCnt = new GUIContent(SKAssetLibrary.LoadTexture("alpha_circle_smooth"));

        }
        private void OnEnable()
        {
            Init();
            OnTextureLoaded();
        }
        private void OnGUI()
        {
            Repaint();
            DrawTopRow();
            DrawLeftColumn();
            DrawRightColumn();
            DrawCenter();
        }

        void DrawTopRow()
        {
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(1400, 30)), new Color(.15f, .15f, .15f));
            GUILayout.Label("SK Sprite Editor");
            EditorGUI.DrawRect(new Rect(new Vector2(0, 30), new Vector2(1400, 4)), Color.gray);
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.fontStyle = FontStyle.Normal;

            if (GUI.Button(new Rect(1020, 3, 120, 20), "Revert Changes"))
            {
                if (texture)
                {
                    OnTextureLoaded();
                }
            }
            if (GUI.Button(new Rect(1150,3,120,20),"Apply Changes"))
            {
                if (texture)
                {
                    SetTextureImporterFormat(texture);
                    byte[] itemBGBytes = texture.EncodeToPNG();
                    File.WriteAllBytes(texturePath, itemBGBytes);
                    AssetDatabase.ImportAsset(texturePath);
                    AssetDatabase.Refresh();
                }
            }
        }
        void DrawCenter()
        {
            Rect areaRect = new Rect(new Vector2(LEFT_COL_WIDTH, TOP_BAR_HEIGHT - 10), new Vector2(1280, 720 - TOP_BAR_HEIGHT + 10));
            GUILayout.BeginArea(areaRect);

            Rect centerRect = new Rect(new Vector2(50, TOP_BAR_HEIGHT - 10), new Vector2(CENTER_WIDTH, 720 - TOP_BAR_HEIGHT + 10));
            EditorGUI.DrawRect(centerRect, new Color(.17f, .17f, .17f));
            if (texture)
            {
                float SELF_X = 800.0f, SELF_Y = 690.0f;
                EditorGUI.DrawTextureTransparent(centerRect, texture, ScaleMode.ScaleToFit);

                //Get mouse position in image pixels
                Vector2 mousePosInEditor = Event.current.mousePosition;
                Vector2 mousePosInWindow = GUIUtility.GUIToScreenPoint(mousePosInEditor) - SKSpriteEditor.window.position.position;
                Vector2 mousePosInArea = mousePosInWindow - (centerRect.position+ areaRect.position) - Vector2.up*20;
                float aspect = (float)texture.width/texture.height;
                float selfAspect = SELF_X / SELF_Y;
                float fitAspect = 1;
                Vector2 mousePosInPixel = -Vector2.one;
                if (aspect > selfAspect) //fit in x
                {
                    fitAspect = SELF_X / texture.width;
                    mousePosInPixel.x = (mousePosInArea.x/ SELF_X) * texture.width;
                    float sizeY = SELF_X / aspect;
                    float borderN = SELF_Y / 2 - sizeY / 2, borderP = SELF_Y / 2 + sizeY / 2;
                    mousePosInPixel.y = Mathf.InverseLerp(borderN, borderP, mousePosInArea.y) * texture.height;
                }
                else //fit in y
                {
                    fitAspect = SELF_Y / texture.height;
                    mousePosInPixel.y = (mousePosInArea.y / SELF_Y) * texture.height;
                    float sizeX = SELF_Y * aspect;
                    float borderN = SELF_X / 2 - sizeX / 2, borderP = SELF_X/2+sizeX/2;
                    mousePosInPixel.x = Mathf.InverseLerp(borderN, borderP, mousePosInArea.x) * texture.width;

                }
                mousePosInPixel.y = texture.height - mousePosInPixel.y;
                isMouseInImage = mousePosInPixel.x>0 && mousePosInPixel.x<texture.width && mousePosInPixel.y>0 && mousePosInPixel.y< texture.height;

                float brushSize = brush.size;
                float brushSizeInEditor = brushSize * fitAspect;
                if(mode == EditMode.Select)
                {
                    if (isMouseInImage)
                    {
                        if (IsMousePressed(0, EventType.MouseDown))
                        {
                            selectStartPos = mousePosInEditor;
                            selectEndPos = mousePosInEditor;
                            selectStartPosPixel = mousePosInPixel;
                            areaSelected = false;
                        }

                        float boxSize = 30;
                        Rect brushRect = new Rect(mousePosInEditor - Vector2.one * boxSize / 2, Vector2.one * boxSize);
                        SetBackgroundColor(Color.clear);
                        GUI.Box(brushRect, aimCnt);
                        RestoreBackgroundColor();
                    }
                    if (IsMousePressed(0, EventType.MouseDrag))
                    {
                        areaSelected = true;
                        selectEndPos = mousePosInEditor;
                        selectedPixels = new Rect(selectStartPosPixel, mousePosInPixel - selectStartPosPixel);
                        selectedPixels = NormalizeRect(selectedPixels);
                    }

                    if (areaSelected)
                    {
                        if (IsMousePressed(0, EventType.MouseUp))
                        {
                           
                        }
                    }
                }
                else if (mode == EditMode.Draw ||  mode == EditMode.Erase)
                {
                    if (isMouseInImage)
                    {
                        Rect brushRect = new Rect(mousePosInEditor - Vector2.one * brushSizeInEditor / 2, Vector2.one * brushSizeInEditor);
                        SetBackgroundColor(Color.clear);
                        GUI.Box(brushRect, brush.type== BrushType.Smooth? alphaCircleSmoothCnt: alphaCircleCnt);
                        RestoreBackgroundColor();
                        bool firstStroke = false;
                        if(IsMousePressed(0, EventType.MouseDown))
                        {
                            firstStroke = true;
                            oBrushPos = mousePosInPixel;
                        }
                        if (IsMousePressed(0))
                        {
                            TextureFill(mousePosInPixel, brush, mode == EditMode.Erase);
                            if (!firstStroke)
                            {
                                float dist = Vector2.Distance(oBrushPos, mousePosInPixel);
                                float gap = brush.size / 5;
                                int steps = Mathf.CeilToInt(dist / gap);
                                for (int i = 0; i < steps; i++)
                                {
                                    TextureFill(Vector2.Lerp(oBrushPos, mousePosInPixel, i / (float)steps), brush, mode == EditMode.Erase);
                                }
                            }
                            texture.Apply();

                            oBrushPos = mousePosInPixel;
                        }
                    }
                }
                else if (mode == EditMode.Pick)
                {
                    if (isMouseInImage)
                    {
                        float boxSize = 40;
                        Rect brushRect = new Rect(mousePosInEditor - Vector2.one * boxSize / 2 + Vector2.down* 30, Vector2.one * boxSize);
                        boxSize *= 1.1f;
                        Rect brushRect2 = new Rect(mousePosInEditor - Vector2.one * boxSize / 2 + Vector2.down * 30, Vector2.one * boxSize);
                        Color c = texture.GetPixel((int)mousePosInPixel.x, (int)mousePosInPixel.y);
                        EditorGUI.DrawRect(brushRect2, COLOR_GOLD);
                        EditorGUI.DrawRect(brushRect, c);
                        Rect textRect = new Rect(brushRect);
                        textRect.position += Vector2.right * boxSize;
                        textRect.size  += Vector2.right*100;
                        GUI.skin.label.fontSize = 5;
                        GUI.skin.label.alignment = TextAnchor.UpperLeft;
                        EditorGUI.LabelField(textRect, $"({c.r.ToString("f2")}, {c.g.ToString("f2")}, {c.b.ToString("f2")})");
                        GUI.skin.label.fontSize = 12;
                        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                        if (IsMousePressed(0, EventType.MouseDown))
                        {
                            brush.color = c;
                        }
                    }
                }

                if (areaSelected)
                {
                    Rect rect = new Rect(selectStartPos, selectEndPos - selectStartPos);
                    EditorGUI.DrawRect(rect, new Color(.9f, .9f, .9f, .35f+.2f *(m.sin(Time.realtimeSinceStartup * 5)+1)/2));
                }
                //Global color picker shortcut
                if (isMouseInImage)
                {
                    if (IsMousePressed(1, EventType.MouseDown))
                    {
                        oMode = mode;
                        mode = EditMode.Pick;
                    }
                    if (IsMousePressed(1, EventType.MouseUp))
                    {
                        mode = oMode;
                        brush.color = texture.GetPixel((int)mousePosInPixel.x, (int)mousePosInPixel.y);
                    }
                }
            }
            else
            {
                GUI.Label(new Rect(370,300,200,20), "Load a texture to continue.");
            }
            GUILayout.EndArea();
        }
        void DrawLeftColumn()
        {
            GUILayout.BeginArea(new Rect(new Vector2(0, TOP_BAR_HEIGHT), new Vector2(LEFT_COL_WIDTH, 1080)));
            EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(LEFT_COL_WIDTH, 1080)), new Color(.18f, .18f, .18f));

            EditorGUILayout.BeginVertical();
            BoldTitle();
            GUILayout.Label("File");
            NormalTitle();
            EditorGUI.DrawRect(new Rect(new Vector2(0, 20), new Vector2(LEFT_COL_WIDTH, 2)), new Color(.7f, .8f, .9f));
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Texture: ");
            otex = EditorGUILayout.ObjectField(otex, typeof(Texture2D), GUILayout.Width(50), GUILayout.Height(50)) as Texture2D;
            if (otex == null)
                texture = null;

            if (otex != prevTetxure)
            {
                OnTextureLoaded();
            }
            prevTetxure = otex;

            BoldSubtitle();
            GUILayout.Label("Info");
            NormalSubtitle();
            if (!texture || !otex)
            {
                GUILayout.Label("No texture selected.");
            }
            else
            {
                GUILayout.Label($"{otex.name}\n{texture.graphicsFormat} \n{texture.width} x {texture.height}");
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Save As...") && texture!=null)
            {
                string path = EditorUtility.SaveFilePanel("Save As...", texturePath.Substring(0,texturePath.LastIndexOf("/")), texture.name, "png");
                if (path.IndexOf("Assets") > 0)
                {
                    Texture2D t = new Texture2D(texture.width, texture.height);
                    t.SetPixels(texture.GetPixels());
                    t.Apply();
                    byte[] itemBGBytes = t.EncodeToPNG();
                    File.WriteAllBytes(path, itemBGBytes);
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.Refresh();
                    texture = t;
                    texturePath = path;
                }
            }

            if (GUILayout.Button("Save Selected Area As...") && areaSelected)
            {
                string path = EditorUtility.SaveFilePanel("Save Selected Area As...", texturePath.Substring(0, texturePath.LastIndexOf("/")), texture.name, "png");
                if (path.IndexOf("Assets") > 0)
                {
                    Texture2D t = new Texture2D((int)selectedPixels.width, (int)selectedPixels.height);
                    t.SetPixels( texture.GetPixels((int)selectedPixels.x, (int)selectedPixels.y, (int)selectedPixels.width, (int)selectedPixels.height));
                    t.Apply();
                    byte[] itemBGBytes = t.EncodeToPNG();
                    File.WriteAllBytes(path, itemBGBytes);
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.Refresh();
                    texture = t;
                    texturePath = path;

                    areaSelected = false;
                }
            }

            EditorGUILayout.Space(105);
            BoldTitle();
            GUILayout.Label("Create");
            NormalTitle();
            EditorGUI.DrawRect(new Rect(new Vector2(0, 320+25 * (texture==null?0:1)), new Vector2(LEFT_COL_WIDTH, 2)), new Color(.8f, .7f, .9f));
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Width: ", GUILayout.Width(60));
            newImageSize.x = EditorGUILayout.IntField(newImageSize.x);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Height: ", GUILayout.Width(60));
            newImageSize.y = EditorGUILayout.IntField(newImageSize.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("Background Color: ", GUILayout.Width(150));
            newImageColor = EditorGUILayout.ColorField(newImageColor);

            EditorGUILayout.Separator();
            if (GUILayout.Button("Create New Image..."))
            {
                string folderPath = "Assets/";
                if (texture != null)
                    folderPath = texturePath.Substring(0, texturePath.LastIndexOf("/"));
                string path = EditorUtility.SaveFilePanel("Create New Image...", folderPath, $"NewImage{newImageSize.x}x{newImageSize.y}", "png");
                if (path.IndexOf("Assets") > 0)
                {
                    Texture2D t = new Texture2D(newImageSize.x, newImageSize.y);
                    t.SetColor(newImageColor);
                    t.Apply();
                    byte[] itemBGBytes = t.EncodeToPNG();
                    File.WriteAllBytes(path, itemBGBytes);
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.Refresh();
                    texture = t;
                    otex = AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(Texture2D)) as Texture2D;
                    texturePath = path;
                    OnTextureLoaded();
     
                }
            }
            GUILayout.EndArea();
        }
        void DrawRightColumn()
        {
            bool dirty = false;

            Rect rect = new Rect(new Vector2(LEFT_COL_WIDTH + 918, TOP_BAR_HEIGHT), new Vector2(LEFT_COL_WIDTH, 1080));
            GUILayout.BeginArea(rect);
            EditorGUI.DrawRect(new Rect(new Vector2(0, 0), new Vector2(LEFT_COL_WIDTH, 1080)), new Color(.18f, .18f, .18f));

            EditorGUILayout.BeginVertical();
            BoldTitle();
            GUILayout.Label("Editor");
            EditorGUI.DrawRect(new Rect(new Vector2(0, 20), new Vector2(LEFT_COL_WIDTH, 2)), new Color(.9f, .8f, .7f));

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            if (mode == EditMode.Select)
                SetBackgroundColor(COLOR_GOLD_BRIGHT);
            if (GUILayout.Button(selectButtonCnt, GUILayout.Width(25), GUILayout.Height(25)))
            {
                SelectTool(EditMode.Select);
            }
            if (mode == EditMode.Select)
                RestoreBackgroundColor();

            if (mode == EditMode.Draw)
                SetBackgroundColor(COLOR_GOLD_BRIGHT);
            if (GUILayout.Button(brushButtonCnt, GUILayout.Width(25), GUILayout.Height(25)))
            {
                SelectTool(EditMode.Draw);
            }
            if (mode == EditMode.Draw)
                RestoreBackgroundColor();

            if (mode == EditMode.Erase)
                SetBackgroundColor(COLOR_GOLD_BRIGHT);
            if (GUILayout.Button(eraserButtonCnt, GUILayout.Width(25), GUILayout.Height(25)))
            {
                SelectTool(EditMode.Erase);
            }
            if (mode == EditMode.Erase)
                RestoreBackgroundColor();

            if (mode == EditMode.Pick)
                SetBackgroundColor(COLOR_GOLD_BRIGHT);
            if (GUILayout.Button(pickerButtonCnt, GUILayout.Width(25), GUILayout.Height(25)))
            {
                SelectTool(EditMode.Pick);
            }
            if (mode == EditMode.Pick)
                RestoreBackgroundColor();

            EditorGUILayout.EndHorizontal();
            NormalSubtitle();
            GUILayout.Label("Current Tool: "+ toolString);

            BoldSubtitle();
            EditorGUILayout.Separator();
            GUILayout.Label("Brush Palette");
            EditorGUILayout.BeginHorizontal();
            brush.color = EditorGUILayout.ColorField(brush.color, GUILayout.Width(64), GUILayout.Height(48));
            EditorGUI.DrawTextureTransparent(new Rect(70, 120, 30, 30), brush.type == BrushType.Smooth?SKAssetLibrary.Texture_Smooth_Circle: SKAssetLibrary.Texture_Solid_Circle);
            brush.type = (BrushType)EditorGUILayout.EnumPopup(brush.type);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Size: "); 
            brush.size = EditorGUILayout.Slider(brush.size, 1, texture?texture.width/5:100);
            EditorGUILayout.EndHorizontal();
            if (brush.type == BrushType.Smooth)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Smooth: ");
                brush.smoothness = EditorGUILayout.Slider(brush.smoothness, 1f, 10f);
                EditorGUILayout.EndHorizontal();
            }
            if (texture)
            {
                    
                NormalTitle();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                NormalSubtitle();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Type");

                importer.textureType = (TextureImporterType)EditorGUILayout.EnumPopup(importer.textureType);
                EditorGUILayout.EndHorizontal();

                BoldSubtitle();
                GUILayout.Label("Color Properties");
                NormalSubtitle();

                //Saturation
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Saturation");
                properties.saturation = EditorGUILayout.Slider(properties.saturation, 0, 3);
                EditorGUILayout.EndHorizontal();
                dirty |= properties.saturation != prevProperties.saturation;
                prevProperties.saturation = properties.saturation;

                //Contrast
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Contrast");
                properties.contrast = EditorGUILayout.Slider(properties.contrast, 0, 3);
                EditorGUILayout.EndHorizontal();
                dirty |= properties.contrast != prevProperties.contrast;
                prevProperties.contrast = properties.contrast;

                //Brightness
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Brightness");
                properties.brightness = EditorGUILayout.Slider(properties.brightness, 0, 3);
                EditorGUILayout.EndHorizontal();
                dirty |= properties.brightness != prevProperties.brightness;
                prevProperties.brightness = properties.brightness;

                if (dirty)
                    UpdateTextureColor();

                EditorGUILayout.Separator();
                BoldSubtitle();
                GUILayout.Label("Erase Color");
                NormalSubtitle();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Tolerance: ");
                eraseTolerance = EditorGUILayout.Slider(eraseTolerance, 0, 1);
                colorToErase = EditorGUILayout.ColorField(colorToErase);
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Erase"))
                {
                    EraseTextureColor(colorToErase);
                }

                EditorGUILayout.Separator();
                BoldSubtitle();
                GUILayout.Label("Gaussian Blur");
                NormalSubtitle();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Radius: ");
                properties.blur = EditorGUILayout.Slider(properties.blur, 0, 15);
                EditorGUILayout.EndHorizontal();
                prevProperties.blur = properties.blur;
                if (GUILayout.Button("Blur"))
                {
                    texture = linearBlur.Blur(otex, (int)properties.blur, 3, selectedPixels, areaSelected);
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.EndArea();
        }

        void TextureFill(Vector2 position, Brush brush, bool isErase = false)
        {
            Color bc = isErase?Color.clear: brush.color;
            Color c;
            float halfSize = brush.size / 2;  
            Vector2Int lb = new Vector2Int((int)(position.x- halfSize), (int)(position.y- halfSize));
            Vector2Int rt = new Vector2Int((int)(position.x + halfSize), (int)(position.y + halfSize));
            int tw = texture.width, th = texture.height;
            bool isSmooth = brush.type == BrushType.Smooth;

            int startX=0, startY = 0, endX = 0, endY = 0;
            if (areaSelected)
            {
                startX = (int)selectedPixels.position.x;
                startY = (int)selectedPixels.position.y;
                endX = (int)selectedPixels.x + (int)selectedPixels.width;
                endY = (int)selectedPixels.y + (int)selectedPixels.height;
            }
            for (int i = lb.x; i < rt.x; i++)
            {
                for (int j = lb.y; j < rt.y; j++)
                {
                    if (i >= tw || j >= th)
                        continue;
                    if (i < 0 || j < 0)
                        continue;
                    if (areaSelected)
                    {
                        if (i < startX || i > endX || j < startY || j > endY)
                            continue;
                    }
                        float dist = Vector2.Distance(new Vector2(i, j), position);
                    if(dist < halfSize)
                    {
                        dist/=halfSize;
                        if (isSmooth)
                        {
                            c = texture.GetPixel(i, j);
                            if(c.a==0)
                            {
                                c = bc;
                                c.a = 0;
                            }
                            float alpha = Mathf.SmoothStep(1,0,dist);
                            alpha *= 1 / brush.smoothness;
                            Color diff = bc - c;
                            c += diff * Mathf.Clamp01(alpha);
                            c = c.Saturate();

                        }
                        else
                        {
                            c = bc;
                        }
                        texture.SetPixel(i, j, c);
                    }

                }
            }
        }
        void EraseTextureColor(Color color)
        {
            int startX, startY, endX, endY;
            if (areaSelected)
            {
                startX = (int)selectedPixels.position.x;
                startY = (int)selectedPixels.position.y;
                endX = (int)selectedPixels.x + (int)selectedPixels.width;
                endY = (int)selectedPixels.y + (int)selectedPixels.height;
            }
            else
            {
                startX = 0;
                startY = 0;
                endX = texture.width;
                endY = texture.height;
            }
            Color c;
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    c = texture.GetPixel(i, j);
                    if(c.Distance(color)< eraseTolerance)
                    {
                        c = Color.clear;
                    }
                    texture.SetPixel(i, j, c);
                }
            }
            texture.Apply();
        }
        void UpdateTextureColor()
        {
            updateCounter += 1;
            if (updateCounter % 2 != 0)
            {
                //return;
            }
            int startX, startY, endX, endY;
            if (areaSelected)
            {
                startX = (int)selectedPixels.position.x;
                startY = (int)selectedPixels.position.y;
                endX = (int)selectedPixels.x + (int)selectedPixels.width;
                endY = (int)selectedPixels.y + (int)selectedPixels.height;
            }
            else
            {
                startX = 0;
                startY = 0;
                endX = texture.width;
                endY = texture.height;
            }
            Color c;
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    c = otex.GetPixel(i, j);
                    float a = c.a;
                    float l = c.Luminance();
                    Color lc = new Color(l, l, l);
                    c = lc + properties.saturation * (c - lc);
                    c = Color.grey + properties.contrast * (c - Color.grey);
                    c *= properties.brightness;
                    c.a = a;
                    texture.SetPixel(i, j, c);
                }
            }
            texture.Apply();
        }

        void OnTextureLoaded()
        {
            if (otex)
            {
                texturePath = AssetDatabase.GetAssetPath(otex);
                importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                properties = new SpriteProperties();
                prevProperties = new SpriteProperties();
                SetTextureImporterFormat(otex);
                texture = new Texture2D(otex.width, otex.height);
                texture.SetPixels(otex.GetPixels());
                texture.Apply();
            }
        }

        private static void SetBackgroundColor(Color color)
        {
            oBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }
        private static void SetContentColor(Color color)
        {
            oContentColor = GUI.contentColor;
            GUI.contentColor = color;
        }
        private static void RestoreBackgroundColor()
        {
            GUI.backgroundColor = oBackgroundColor;
        }

        private static void NormalSubtitle()
        {
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        }
        private static void NormalTitle()
        {
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
        }
        private static void BoldSubtitle()
        {
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        }
        private static void BoldTitle()
        {
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        }
        public void SetTextureImporterFormat(Texture2D texture)
        {
            if (null == texture) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = importer.textureType;
                tImporter.isReadable = true;

                AssetDatabase.ImportAsset(assetPath);
                AssetDatabase.Refresh();
            }
        }

        private Rect NormalizeRect(Rect rect)
        {
            Rect r = new Rect(rect);
            rect.position = new Vector2(rect.width < 0 ? rect.position.x + rect.width : rect.position.x, rect.height < 0 ? rect.position.y + rect.height : rect.position.y);
            rect.size = new Vector2(m.abs(rect.width), m.abs(rect.height));
            return rect;
        }
        private void SelectTool(EditMode mode)
        {
            SKSpriteEditor.mode = mode;

            switch (mode)
            {
                case EditMode.Select:
                    toolString = "Selector";
                    break;
                case EditMode.Draw:
                    toolString = "Paint Brush";
                    break;
                case EditMode.Erase:
                    toolString = "Eraser";
                    break;
                case EditMode.Pick:
                    toolString = "Color Picker";
                    break;
                default:
                    break;
            }
        }
        private bool IsMousePressed(int button, EventType type = EventType.MouseDrag)
        {
            return Event.current.isMouse && Event.current.type == type && Event.current.button == button;
        }
        class SpriteProperties
        {
            public float saturation = 1, contrast = 1, brightness = 1;
            public float blur = 0;
        }

        class Brush
        {
            public BrushType type = BrushType.Smooth;
            public BrushShape shape = BrushShape.Circle;
            public Color color = Color.white;
            public float size = 50.0f;
            public float smoothness = 1.5f;
        }

        enum BrushShape
        {
            Circle
        }
        enum BrushType
        {
            Smooth,
            Solid
        }
        enum EditMode
        {
            Select,
            Draw,
            Erase,
            Pick
        }

        class LinearBlur
        {
            private float _rSum = 0;
            private float _gSum = 0;
            private float _bSum = 0;
            private float _aSum = 0;

            private Texture2D _sourceImage;
            private int _sourceWidth;
            private int _sourceHeight;
            private int _windowSize;

            private Rect selectedPixels;
            private bool areaSelected;
            public Texture2D Blur(Texture2D image, int radius, int iterations, Rect selectedPixels, bool areaSelected)
            {
                this.selectedPixels = selectedPixels;
                this.areaSelected = areaSelected;
                _windowSize = radius * 2 + 1;
                _sourceWidth = image.width;
                _sourceHeight = image.height;

                var tex = image;

                for (var i = 0; i < iterations; i++)
                {
                    tex = OneDimensialBlur(tex, radius, true);
                    tex = OneDimensialBlur(tex, radius, false);
                }

                return tex;
            }

            private Texture2D OneDimensialBlur(Texture2D image, int radius, bool horizontal)
            {
                _sourceImage = image;

                var blurred = new Texture2D(image.width, image.height, image.format, false);

                if (horizontal)
                {
                    for (int imgY = 0; imgY < _sourceHeight; ++imgY)
                    {
                        ResetSum();

                        for (int imgX = 0; imgX < _sourceWidth; imgX++)
                        {
                            EditorUtility.DisplayProgressBar("Applying blur...", "blurring...",(imgY * _sourceWidth + imgX) / (float)(_sourceHeight * _sourceWidth));
                            if (imgX == 0)
                                for (int x = radius * -1; x <= radius; ++x)
                                    AddPixel(GetPixelWithXCheck(x, imgY));
                            else
                            {
                                var toExclude = GetPixelWithXCheck(imgX - radius - 1, imgY);
                                var toInclude = GetPixelWithXCheck(imgX + radius, imgY);

                                SubstPixel(toExclude);
                                AddPixel(toInclude);
                            }

                            blurred.SetPixel(imgX, imgY, CalcPixelFromSum());
                        }
                    }
                }

                else
                {
                    for (int imgX = 0; imgX < _sourceWidth; imgX++)
                    {
                        ResetSum();

                        for (int imgY = 0; imgY < _sourceHeight; ++imgY)
                        {
                            EditorUtility.DisplayProgressBar("Applying blur...", "blurring...", (imgY * _sourceWidth + imgX) / (float)(_sourceHeight * _sourceWidth));
                            if (imgY == 0)
                                for (int y = radius * -1; y <= radius; ++y)
                                    AddPixel(GetPixelWithYCheck(imgX, y));
                            else
                            {
                                var toExclude = GetPixelWithYCheck(imgX, imgY - radius - 1);
                                var toInclude = GetPixelWithYCheck(imgX, imgY + radius);

                                SubstPixel(toExclude);
                                AddPixel(toInclude);
                            }

                            blurred.SetPixel(imgX, imgY, CalcPixelFromSum());
                        }
                    }
                }

                blurred.Apply();
                EditorUtility.ClearProgressBar();
                return blurred;
            }

            private Color GetPixelWithXCheck(int x, int y)
            {
                if (x <= 0) return _sourceImage.GetPixel(0, y);
                if (x >= _sourceWidth) return _sourceImage.GetPixel(_sourceWidth - 1, y);
                Color c = _sourceImage.GetPixel(x, y);
              
                return c;
            }

            private Color GetPixelWithYCheck(int x, int y)
            {
                if (y <= 0) return _sourceImage.GetPixel(x, 0);
                if (y >= _sourceHeight) return _sourceImage.GetPixel(x, _sourceHeight - 1);
                Color c = _sourceImage.GetPixel(x, y);
              
                return c;
            }
            private void AddPixel(Color pixel)
            {
                _rSum += pixel.r;
                _gSum += pixel.g;
                _bSum += pixel.b;
                _aSum += pixel.a;
            }

            private void SubstPixel(Color pixel)
            {
                _rSum -= pixel.r;
                _gSum -= pixel.g;
                _bSum -= pixel.b;
                _aSum -= pixel.a;
            }

            private void ResetSum()
            {
                _rSum = 0.0f;
                _gSum = 0.0f;
                _bSum = 0.0f;
                _aSum = 0.0f;
            }

            Color CalcPixelFromSum()
            {
                return new Color(_rSum / _windowSize, _gSum / _windowSize, _bSum / _windowSize, _aSum / _windowSize);
            }
        }
    }
}
