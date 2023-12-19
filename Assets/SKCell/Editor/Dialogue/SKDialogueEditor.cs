using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SKCell 
{
    public class SKDialogueEditor : EditorWindow
    {
        static Color COL_BACKGROUND = new Color(.1f, .1f, .1f);
        static Color COL_WIRE = new Color(1, 1, 1, .07f);
        static Color COL_TITLE = new Color(1, 1, 1, .6f);
        static Color COL_YELLOW = new Color(1f, .95f, .6f, 0.9f);
        static Color COL_BLUE = new Color(.4f, .7f, .9f, 0.8f);
        static Color COL_ORANGE = new Color(1f, .7f, .7f, 0.8f);
        static Rect BG_RECT = new Rect(-1000, -1000, 10000, 10000);

        static GUIStyle titleStyle, node_whiteStyle, node_yellowStyle, node_greenStyle, node_greyStyle, node_purpleStyle, node_titleStyle, inspector_bgStyle, inspector_buttonStyle;
        static GUIStyle node_subtitleStyle,node_centerStyle, node_centerStyle2, node_centerStyle3, node_subtitle2Style, inspector_titleStyle, text_orangeStyle, node_blueStyle;

        Vector2 inspector_scrollPos_0, inspector_scrollPos_1, inspector_scrollPos_2;
         
        public SKDialogueAsset asset;
        public List<SKDialogueEditorNode> nodes; 
        public SKDialogueEditorNode inspectorNode;

        //connection state
        bool isConnecting = false;
        SKDialogueEditorNode connectFromNode;

        private GameObject prevSelection;
        private SKDialogueAsset f_asset;


        float panX = 0;
        float panY = 0;

        [MenuItem("Tools/SKCell/Diagolue Editor", priority = 2)]
        static void OpenWindow()
        {
            SKDialogueEditor editor = EditorWindow.GetWindow<SKDialogueEditor>("SK Dialogue Editor",typeof(UnityEditor.SceneView));
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnBecameVisible()
        {
            InitializeGUIStyles();
        }

        private void OnLostFocus()
        {
            if(asset != null)
            EditorUtility.SetDirty(asset); 
        }
        public void Init()
        {
            if(asset!=null)
                LoadAsset(asset);
            else
            LoadAsset(null);
            inspectorNode = null;
        }

        private void InitializeGUIStyles()
        {
            titleStyle = new GUIStyle()
            {
                alignment = TextAnchor.UpperLeft,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 21,
            };
            titleStyle.normal.textColor = COL_TITLE;

            node_titleStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 10,
            };
            node_titleStyle.normal.textColor = COL_TITLE;

            text_orangeStyle = new GUIStyle();
            text_orangeStyle.normal.textColor = COL_ORANGE;
            text_orangeStyle.alignment = TextAnchor.MiddleLeft;

            node_subtitleStyle = new GUIStyle()
            {
                alignment = TextAnchor.UpperLeft,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 11,
                wordWrap = true
            };
            node_subtitleStyle.normal.textColor = COL_TITLE;
            node_centerStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 12,
                wordWrap = true
            };
            node_centerStyle.normal.textColor = COL_TITLE; 
            node_centerStyle2= new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 12,
                wordWrap = true
            };
            node_centerStyle2.normal.textColor = COL_BLUE;
            node_centerStyle3 = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 12,
                wordWrap = true
            };
            node_centerStyle3.normal.textColor = COL_ORANGE;

            node_subtitle2Style = new GUIStyle()
            {
                alignment = TextAnchor.UpperLeft,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 11,
                wordWrap = true
            };
            node_subtitle2Style.normal.textColor = COL_YELLOW;

            node_whiteStyle = new GUIStyle();
            node_whiteStyle.normal.background = MakeTex(2, 2, new Color(1, 1, 1, 0.6f));

            node_yellowStyle = new GUIStyle();
            node_yellowStyle.normal.background = MakeTex(2, 2, new Color(1f, .95f, .5f, 0.6f));
            node_greyStyle = new GUIStyle();
            node_greyStyle.normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.6f));
            node_purpleStyle = new GUIStyle();
            node_purpleStyle.normal.background = MakeTex(2, 2, new Color(.7f, .85f, .25f, 0.8f));
            node_greenStyle = new GUIStyle();
            node_greenStyle.normal.background = MakeTex(2, 2, new Color(.6f, 1f, .8f, 0.6f));
            node_blueStyle = new GUIStyle();
            node_blueStyle.normal.background = MakeTex(2, 2, new Color(.4f, .7f, .9f, 0.8f));

            inspector_titleStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                font = SKAssetLibrary.DefaultFont,
                fontSize = 14,
            };
            inspector_titleStyle.normal.textColor = new Color(1f, .95f, .7f, 0.75f);

            inspector_bgStyle = new GUIStyle();
            inspector_bgStyle.font = SKAssetLibrary.DefaultFont;
            inspector_bgStyle.fontSize = 11;
            inspector_bgStyle.alignment = TextAnchor.MiddleCenter;
            inspector_bgStyle.normal.background = MakeTex(2, 2, new Color(.18f, .18f, .18f, 0.9f));
            inspector_bgStyle.normal.textColor = COL_TITLE;

            inspector_buttonStyle = new GUIStyle();
            inspector_buttonStyle.font = SKAssetLibrary.DefaultFont;
            inspector_buttonStyle.fontSize = 11;
            inspector_buttonStyle.alignment = TextAnchor.MiddleCenter;
            inspector_buttonStyle.normal.background = MakeTex(2, 2, new Color(.18f, .18f, .18f, 0.9f));
            inspector_buttonStyle.normal.textColor = COL_TITLE;
            inspector_buttonStyle.hover.background = MakeTex(2, 2, new Color(.3f, .3f, .3f, 0.9f));
            inspector_buttonStyle.hover.textColor = new Color(1f, .95f, .5f, 0.6f);
        } 

        public void SaveAsset()
        {

        }
        public void LoadAsset(SKDialogueAsset asset)
        {
            if (asset == null)
            {
                this.asset = null;
                nodes = null;
                SKDialogueManager.curEditorAsset = null;
                return;
            }
            this.asset = asset;
            this.f_asset = asset;
            nodes = asset.editorNodes;
            SKDialogueManager.curEditorAsset = asset;

            if (asset.editorNodes.Count == 0)
            {
                SKDialogueEditorNode start = AddNewNode(SKDialogueEditorNodeType.Start, new Vector2(100, 100));
                SKDialogueEditorNode end = AddNewNode(SKDialogueEditorNodeType.End, new Vector2(400, 100));
            }
        }
        private void Update()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject != prevSelection)
            {
                SKDialoguePlayer player = Selection.activeGameObject.GetComponent<SKDialoguePlayer>();
                if (player != null)
                {
                    if (asset != player.asset)
                    {
                        LoadAsset(player.asset);
                        Repaint();
                    }
                }
            }
            prevSelection = Selection.activeGameObject;
        }
        void OnGUI()
        {            
            GUI.depth = 0;
            DrawBackground();
            HandleMouseInput();
            HandleConnectionState();

            GUI.BeginGroup(new Rect(panX - 1000, panY - 1000, 100000, 100000));
            BeginWindows();
            if(nodes!=null && SKDialogueManager.curEditorAsset != null)
                for (int i = 0; i < nodes.Count; i++)
                {
                    DrawNode(nodes[i], i);
                }
            EndWindows();

            GUI.EndGroup();

            DrawInspector();

            if (EditorWindow.mouseOverWindow == this)
            {
                Repaint();
            }
        }

        void DrawBackground()
        {
            GUI.backgroundColor = new Color(.8f, .8f, .8f);
            EditorGUI.DrawRect(BG_RECT, COL_BACKGROUND);

            for (int i = 0; i < 4000; i += 30)
            {
                EditorGUI.DrawRect(new Rect(panX - 1000, panY - 1000 + i, 10000, 1), COL_WIRE);
                EditorGUI.DrawRect(new Rect(panX - 1000 + i, panY - 1000, 1, 10000), COL_WIRE);
            }


            if (IsMouseInInspector())
                return;

            Event e = Event.current;
            if (e.type == EventType.MouseDrag)
            {
                if (GetMouseOverNode() != null) return;
                panX += e.delta.x;
                panY += e.delta.y;
            }
        }

        void DrawInspector()
        {
            GUI.Label(new Rect(2,2, 100, 50), "SK Dialogue Editor", titleStyle);
            f_asset = EditorGUI.ObjectField(new Rect(2,30, 150, 20),f_asset, typeof(SKDialogueAsset),false) as SKDialogueAsset;
            if(GUI.Button(new Rect(160,30, 40, 20), "Load"))
            {
                if(asset==null || asset!=f_asset)
                LoadAsset(f_asset);
            }
            
            if (asset == null)
            {
                GUI.Label(new Rect(position.xMax/4, position.yMax/2-20, 100, 50), "Create a new asset to continue.", titleStyle);
            }
            if (inspectorNode == null) return;
            SKDialogueEditorNode node = inspectorNode;
            float sizeX = 200, sizeY = 300;


            //avatar frame
            if (node.type == SKDialogueEditorNodeType.Sentence)
            {
                GUILayout.BeginArea(new Rect(sizeX+20, this.position.yMax - sizeY *0.5f-20, sizeX*0.7f, sizeY / 3+20));
                GUILayout.Box("", inspector_bgStyle, GUILayout.Width(sizeX), GUILayout.Height(sizeY / 3));
                GUILayout.Space(-sizeY/3);
                GUILayout.Label("Avatar sprite: ");
                node.info.avatar = (Texture2D)EditorGUILayout.ObjectField(node.info.avatar, typeof(Texture2D), false);
                GUILayout.Label("Audio: ");
                node.info.audio = (AudioClip)EditorGUILayout.ObjectField(node.info.audio, typeof(AudioClip), false);
                GUILayout.EndArea();
            }

            GUILayout.BeginArea(new Rect(0,this.position.yMax- sizeY, sizeX, sizeY));
            GUILayout.Box("", inspector_bgStyle, GUILayout.Width(sizeX), GUILayout.Height(sizeY));
            GUILayout.Space(-sizeY);

            GUILayout.Label($"{node.type.ToString()} Node", inspector_titleStyle);
            node.name = GUILayout.TextField(node.name);

            //main inspector
            if (node.type == SKDialogueEditorNodeType.Sentence || node.type == SKDialogueEditorNodeType.Choice)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Text type: ");
                node.textType = (SKDialogueEditorNodeTextType)EditorGUILayout.EnumPopup(node.textType);
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (node.textType == SKDialogueEditorNodeTextType.Localized)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Speaker Local ID: ");
                    node.info.speaker_localID = EditorGUILayout.IntField(node.info.speaker_localID);
                    GUILayout.EndHorizontal();
                    try
                    {
                        string s = SKLocalization.GetLocalizationText(node.info.speaker_localID);
                        GUILayout.TextArea(s);
                        GUILayout.Space(5);
                    }
                    catch { }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Content Local ID: ");
                    node.info.content_localID = EditorGUILayout.IntField(node.info.content_localID);
                    GUILayout.EndHorizontal();
                    try
                    {
                        string s = SKLocalization.GetLocalizationText(node.info.content_localID);
                        GUILayout.TextArea(s);
                        GUILayout.Space(5);
                    }
                    catch { }

                }
                else if (node.textType == SKDialogueEditorNodeTextType.Text)
                {
                    GUILayout.Label("Speaker: ");
                    inspector_scrollPos_0 = EditorGUILayout.BeginScrollView(inspector_scrollPos_0, GUILayout.Height(30));
                    node.info.speaker = GUILayout.TextArea(node.info.speaker);
                    EditorGUILayout.EndScrollView();
                    GUILayout.Space(5);
                    GUILayout.Label("Content: ");
                    inspector_scrollPos_1 = EditorGUILayout.BeginScrollView(inspector_scrollPos_1, GUILayout.Height(60));
                    node.info.content = GUILayout.TextArea(node.info.content);
                    EditorGUILayout.EndScrollView();
                }
            }
            else if (node.type== SKDialogueEditorNodeType.Event)
            {
                GUILayout.Space(8);
                GUILayout.Label("Event name: ");

                node.event_name = GUILayout.TextField(node.event_name);
                GUILayout.Label("Arg 0 (float): ");
                node.event_arg0 = EditorGUILayout.FloatField(node.event_arg0);
                GUILayout.Label("Arg 1 (float): ");
                node.event_arg1 = EditorGUILayout.FloatField(node.event_arg1);
            }
            else if (node.type == SKDialogueEditorNodeType.Set)
            {
                GUILayout.Space(8);
                GUILayout.Label("Variable name: ");

                node.int_property_name = GUILayout.TextField(node.int_property_name);
                GUILayout.Space(8);
                GUILayout.Label("Variable value: ");
                node.int_property_value = EditorGUILayout.IntField(node.int_property_value);
            }
            else if (node.type == SKDialogueEditorNodeType.If) 
            {
                GUILayout.Space(8);
                GUILayout.Label("Variable name: ");
                node.if_property_name = GUILayout.TextField(node.if_property_name);
                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Comparator: ");
                node.if_comparator = (SKDE_Comparator)EditorGUILayout.EnumPopup(node.if_comparator);
                GUILayout.EndHorizontal();
                GUILayout.Space(8);
                GUILayout.Label("Variable value: ");
                node.if_property_value = EditorGUILayout.IntField(node.if_property_value);
            }

            GUILayout.BeginArea(new Rect(sizeX / 2 - 25, sizeY - 100, 50, 120));
                if (GUILayout.Button("Close"))
                {
                    inspectorNode = null;
                }
                GUILayout.EndArea();

                GUILayout.EndArea();
        }
        void DrawNode(SKDialogueEditorNode node, int id)
        {
            if (node == null)
                return;

            node.rect = GUI.Window(id, node.rect, DrawNodeWindow,node.name);

            Rect iconRect = new Rect(node.rect.x-20, node.rect.y-20,  25, 25);
            if(node.type == SKDialogueEditorNodeType.Sentence)
                GUI.Box(iconRect, SKAssetLibrary.Texture_Conv_Bubble);

            if (node.type!=SKDialogueEditorNodeType.Start &&
                node.type != SKDialogueEditorNodeType.Random &&
                node.type != SKDialogueEditorNodeType.End &&
                GUI.Button(new Rect(node.rect.xMin, node.rect.yMax + 5, node.rect.width, 15), "Edit", inspector_buttonStyle))
            {
                inspectorNode = node; 
            }

            foreach (var link in node.linkedNodes)
            {
                DrawNodeCurve(node.rect, link.rect);
            }
        }

        void HandleMouseInput()
        {
            if (asset==null || IsMouseInInspector())
                return;

            Event e = Event.current;
            //right click
            if(e.type == EventType.MouseUp && e.button==1)
            {
                SKDialogueEditorNode node = GetMouseOverNode();

                GenericMenu menu = new GenericMenu();
                if(node==null || node.type!= SKDialogueEditorNodeType.End)
                {
                    //create nodes
                    menu.AddItem(new GUIContent("Create New Node/Sentence"), false, menu_AddNodeSentenceNode, (e.mousePosition, node));
                    menu.AddItem(new GUIContent("Create New Node/Choice"), false, menu_AddNodeOptionNode, (e.mousePosition, node));
                    menu.AddItem(new GUIContent("Create New Node/Event"), false, menu_AddNodeActionNode, (e.mousePosition, node));
                    menu.AddItem(new GUIContent("Create New Node/Random"), false, menu_AddNodeRandomNode, (e.mousePosition, node));
                    menu.AddSeparator("Create New Node/");
                    menu.AddItem(new GUIContent("Create New Node/Set"), false, menu_AddNodeSetNode, (e.mousePosition, node));
                    menu.AddItem(new GUIContent("Create New Node/If"), false, menu_AddNodeIfNode, (e.mousePosition, node));
                    menu.AddSeparator("Create New Node/");
                    menu.AddItem(new GUIContent("Create New Node/End"), false, menu_AddNodeEndNode, (e.mousePosition, node));
                }

                if (node != null)
                {
                    if (node.type != SKDialogueEditorNodeType.End)
                        menu.AddItem(new GUIContent("Link To..."), false, menu_ConnectTo, node);
                    if (node.type != SKDialogueEditorNodeType.End)
                        foreach (var link in node.linkedNodes)
                    {
                        menu.AddItem(new GUIContent($"Unlink.../{link.name}"), false, menu_Detach, (node,link));
                    }
                    if(node.type!= SKDialogueEditorNodeType.Start)
                    menu.AddItem(new GUIContent("Delete"), false, menu_DeleteNode, node);
                }

                menu.ShowAsContext();
            }
        }

        void HandleConnectionState()
        {
            if (isConnecting)
            {
                Event e = Event.current;
                if (e != null && connectFromNode!=null)
                {
                    Rect from = new Rect(connectFromNode.rect);
                    from.x -= 1000-panX;
                    from.y -= 1000-panY;
                    DrawNodeCurve(from, new Rect(e.mousePosition.x, e.mousePosition.y, 1, 1));
                }
                if(e!=null && e.type == EventType.MouseUp)
                {
                    SKDialogueEditorNode node = GetMouseOverNode();
                    if (node == null)
                    {
                        isConnecting = false;
                        connectFromNode = null;
                        return;
                    }
                    else
                    {
                        LinkNodes(connectFromNode, node);
                        isConnecting = false;
                        connectFromNode = null;
                    }
                }
            }
        }
        void menu_Detach(object obj)
        {
            (SKDialogueEditorNode, SKDialogueEditorNode) nodes = ((SKDialogueEditorNode, SKDialogueEditorNode))obj;
            UnlinkNodes(nodes.Item1, nodes.Item2);
        }
        void menu_ConnectTo(object obj)
        {
            SKDialogueEditorNode node = (SKDialogueEditorNode)obj;
            connectFromNode = node;
            isConnecting = true;
        }
        void menu_DeleteNode(object obj)
        {
            SKDialogueEditorNode node = (SKDialogueEditorNode)obj;
            DeleteNode(node);
        }
        void menu_AddNodeSentenceNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.Sentence, value.Item1, value.Item2) ;
        }
        void menu_AddNodeOptionNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.Choice, value.Item1, value.Item2);
        } 
        void menu_AddNodeActionNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.Event, value.Item1, value.Item2);
        }
        void menu_AddNodeRandomNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.Random, value.Item1, value.Item2);
        }
        void menu_AddNodeSetNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.Set, value.Item1, value.Item2);
        }
        void menu_AddNodeIfNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.If, value.Item1, value.Item2);
        }
        void menu_AddNodeEndNode(object obj)
        {
            (Vector2, SKDialogueEditorNode) value = ((Vector2, SKDialogueEditorNode))obj;
            AddNewNode(SKDialogueEditorNodeType.End, value.Item1, value.Item2);
        }
        SKDialogueEditorNode AddNewNode(SKDialogueEditorNodeType type, Vector2 position, SKDialogueEditorNode fromNode = null)
        {
            SKDialogueEditorNode node = new SKDialogueEditorNode();
            node.uid = Random.Range(1000, 1000000);
            if(type== SKDialogueEditorNodeType.Start)
                node.name = "Start";
            else if (type== SKDialogueEditorNodeType.Random)
                node.name = "Random";
            else if (type == SKDialogueEditorNodeType.Set)
                node.name = "Set";
            else if (type == SKDialogueEditorNodeType.If)
                node.name = "If";
            else if (type == SKDialogueEditorNodeType.End)
                node.name = "End";
            else
                node.name = type.ToString()+" "+nodes.Count;

            node.type = type;
            node.rect.x = position.x+1000-panX;
            node.rect.y = position.y+1000-panY;
            Vector2 size = node.GetSize();
            node.rect.width = size.x;
            node.rect.height = size.y;

            if (fromNode != null)
                LinkNodes(fromNode, node);
            nodes.Add(node);

            return node;
        }

        void DeleteNode(SKDialogueEditorNode node)
        {
            foreach(var from in node.linkedFromNodes)
            {
                SKUtils.RemoveFromList(from.linkedNodesID, node.uid);
            }
            foreach (var to in node.linkedNodes)
            {
                SKUtils.RemoveFromList(to.linkedFromNodesID, node.uid);
            }
            nodes.Remove(node);
        }
         
        void DrawNodeWindow(int id)
        {
            SKDialogueEditorNode node = nodes[id];
            GUI.DragWindow();

            if (node.type == SKDialogueEditorNodeType.Sentence)
            {
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;


                GUILayout.Box("", node_yellowStyle, GUILayout.Width(500), GUILayout.Height(2));

                GUILayout.Space(8);
                GUILayout.Label(node.GetSpeakerString(), node_subtitle2Style);
                GUILayout.Space(8);
                
                GUILayout.TextArea(node.GetContentString(), node_subtitleStyle);
            }
            else if (node.type == SKDialogueEditorNodeType.Choice)
            {
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_greenStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
                GUILayout.Space(5);
                GUILayout.Label(node.GetSpeakerString(), node_subtitle2Style);
                GUILayout.Space(5);

                GUILayout.TextArea(node.GetContentString(), node_subtitleStyle);
            }
            else if (node.type == SKDialogueEditorNodeType.Start)
            {
                GUI.contentColor = new Color(1, 1, 1, 0.07f);
                GUI.backgroundColor = Color.clear;
                Rect iconRect = new Rect(node.rect.width / 2 -27.5f, node.rect.height / 2 - 15, 55, 55);
                GUI.Box(iconRect, SKAssetLibrary.Texture_CircArrow);
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_greenStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
            }
            else if (node.type == SKDialogueEditorNodeType.End)
            {
                GUI.contentColor = new Color(1, 1, 1, 0.07f);
                GUI.backgroundColor = Color.clear;
                Rect iconRect = new Rect(node.rect.width / 2 - 27.5f, node.rect.height / 2 - 15, 55, 55);
                GUI.Box(iconRect, SKAssetLibrary.Texture_CircArrow);
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_greenStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
            }
            else if (node.type == SKDialogueEditorNodeType.Event)
            {
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_purpleStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
                GUILayout.Space(5);
                GUILayout.Label(node.event_name, node_subtitle2Style);
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                GUILayout.Label(node.event_arg0.ToString("f2"), node_subtitleStyle);
                GUILayout.Label(node.event_arg1.ToString("f2"), node_subtitleStyle);
                GUILayout.EndHorizontal();
            }
            else if (node.type == SKDialogueEditorNodeType.Random)
            {
                GUI.contentColor = new Color(1, 1, 1, 0.07f);
                GUI.backgroundColor = Color.clear;
                Rect iconRect = new Rect(node.rect.width / 2 - 27.5f, node.rect.height / 2 - 15, 55, 55);
                GUI.Box(iconRect, SKAssetLibrary.Texture_Random);
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_blueStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
            }
            else if (node.type == SKDialogueEditorNodeType.Set)
            {
                GUI.contentColor = new Color(1, 1, 1, .6f);
                GUI.backgroundColor = Color.clear;
                Rect iconRect = new Rect(node.rect.width / 2 - 8f, node.rect.height / 2 +1, 16, 16);
                GUI.Box(iconRect, SKAssetLibrary.Texture_ArrowDown);
                GUI.backgroundColor = new Color(.8f, .8f, .8f);
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_greyStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
                GUILayout.Space(2);
                GUILayout.Label(node.int_property_name, node_centerStyle3);
                GUILayout.Space(18);
                GUILayout.Label(node.int_property_value.ToString(), node_centerStyle2);
            } 
            else if (node.type == SKDialogueEditorNodeType.If)
            {
                GUI.backgroundColor = new Color(.8f, .8f, .8f); 
                GUI.contentColor = Color.white;

                GUILayout.BeginVertical();
                GUILayout.Box("", node_greyStyle, GUILayout.Width(500), GUILayout.Height(2));
                GUILayout.EndVertical();
                GUILayout.Space(2);
                GUILayout.Label(node.if_property_name, node_centerStyle3);
                GUILayout.Space(3);
                GUILayout.Label(node.if_comparator.ToString(), node_centerStyle);
                GUILayout.Space(3);
                GUILayout.Label(node.if_property_value.ToString(), node_centerStyle2);
            }
        }

        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(1, 1, 1, 0.06f);
            for (int i = 0; i < 3; i++) // Draw a shadow           
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.gray, null, 1);
        }

        void UnlinkNodes(SKDialogueEditorNode from, SKDialogueEditorNode to)
        {
            SKUtils.RemoveFromList(from.linkedNodesID, to.uid);
            SKUtils.RemoveFromList(to.linkedFromNodesID, from.uid);
        }
        void LinkNodes(SKDialogueEditorNode from, SKDialogueEditorNode to)
        {
            SKUtils.InsertToList(from.linkedNodesID, to.uid,false);
            SKUtils.InsertToList(to.linkedFromNodesID, from.uid,false);
        }
        SKDialogueEditorNode GetMouseOverNode()
        {
            if (nodes == null) return null;
            foreach (var node in nodes)
            {
                if (IsMouseInRect(node.rect))
                    return node;
            }
            return null;
        }
        bool IsMouseInInspector()
        {
            Rect rect = new Rect(-500, position.yMax-300, 700, 300);
            Event e = Event.current;
            return rect.Contains(e.mousePosition);
        }
        bool IsMouseInRect(Rect rect)
        {
            Event e = Event.current;
            return rect.Contains(e.mousePosition - new Vector2(panX - 1000, panY - 1000));
        }
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }

    public enum SKDialogueEditorState
    {
        Normal,
        Connecting
    }
}
