using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SKCell
{
    [System.Serializable]
    public class SKDialogueEditorNode 
    {
        public string name;
        public int uid;
        public Rect rect, oRect;
        public SKDialogueEditorNodeType type;
        public SKDialogueEditorNodeTextType textType;
        public SKDialogueNodeInfo info;
        
        private List<SKDialogueEditorNode> m_linkedNodes, m_linkedFromNodes;
        private int m_orderInAsset;
        public int orderInAsset
        {
            get
            {
                int i = 0;
                foreach(var item in SKDialogueManager.curEditorAsset.editorNodes)
                {
                    if (item == this)
                        break;
                    i++;
                }
                m_orderInAsset = i;
                return m_orderInAsset;
            }
        }

        public List<SKDialogueEditorNode> linkedNodes { get
            {
                ConstructLinks();
                return m_linkedNodes;
            }}
        public List<SKDialogueEditorNode> linkedFromNodes
        {
            get
            {
                ConstructLinks();
                return m_linkedFromNodes;
            }
        }
        public List<int> linkedNodesID = new List<int>();
        public List<int> linkedFromNodesID = new List<int>();

        public string event_name = "<event name>";
        public float event_arg0, event_arg1;

        public string int_property_name = "<variable>";
        public int int_property_value = 0;

        public string if_property_name = "<variable>";
        public int if_property_value = 0;
        public SKDE_Comparator if_comparator = 0;


        public SKDialogueEditorNode()
        {
            info = new SKDialogueNodeInfo();
            info.content = "<content>";
            info.speaker = "<speaker>";
        }

        private void ConstructLinks()
        {
            if (SKDialogueManager.curEditorAsset == null) return;
            m_linkedNodes = new List<SKDialogueEditorNode>();
            m_linkedFromNodes = new List<SKDialogueEditorNode>();   
            foreach (var item in SKDialogueManager.curEditorAsset.editorNodes)
            {
                if (linkedNodesID.Contains(item.uid))
                {
                    m_linkedNodes.Add(item);
                }
                if (linkedFromNodesID.Contains(item.uid))
                {
                    m_linkedFromNodes.Add(item);
                }
            }
        }

        public string GetSpeakerString()
        {
            if (textType == SKDialogueEditorNodeTextType.Text)
            {
                return info.speaker;
            }
            else
            {
                try
                {
                    return SKLocalization.GetLocalizationText(info.speaker_localID);
                }
                catch { }
                return string.Empty;
            }
        }
        public string GetContentString()
        {
            if (textType == SKDialogueEditorNodeTextType.Text)
            {
                return info.content;
            }
            else
            {
                try
                {
                    return SKLocalization.GetLocalizationText(info.content_localID);
                }
                catch { }
                return string.Empty;
            }
        }
        public Vector2 GetSize()
        {
            switch (type)
            {
                case SKDialogueEditorNodeType.Sentence:
                    return new Vector2(100, 130);
                case SKDialogueEditorNodeType.Choice:
                    return new Vector2(100, 80);
                case SKDialogueEditorNodeType.Start:
                    return new Vector2(100, 80);
                case SKDialogueEditorNodeType.Set:
                    return new Vector2(100, 80);
                case SKDialogueEditorNodeType.Event:
                    return new Vector2(100, 80);
                case SKDialogueEditorNodeType.Random:
                    return new Vector2(100, 80);
                case SKDialogueEditorNodeType.If:
                    return new Vector2(100, 80);
                default:
                    return new Vector2(100, 80);
            }
        }
    }

    [System.Serializable]
    public class SKDE_IntProperty
    {
        public string name;
        public int value;
    }

    [System.Serializable]
    public class SKDE_EventProperty
    {
        public string name;
        public Action<float, float> action;
    }
    public enum SKDialogueEditorNodeType
    {
        Sentence,
        Choice,
        Random,
        Event,
        Start,
        Set,
        If,
        End
    }

    public enum SKDialogueEditorNodeTextType
    {
        Text,
        Localized
    }
    public enum SKDE_Comparator
    {
        Equals,
        GreaterThan,
        LessThan,
    }

}
