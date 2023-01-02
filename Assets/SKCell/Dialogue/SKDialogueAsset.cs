using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [CreateAssetMenu(fileName ="SKDialogueAsset", menuName ="SKCell/Dialogue Asset", order=5)]
    public class SKDialogueAsset : ScriptableObject
    {
        public string m_name;
        public List<SKDialogueEditorNode> editorNodes = new List<SKDialogueEditorNode>();
        public List<SKDE_IntProperty> int_properties = new List<SKDE_IntProperty>();  
        public List<SKDE_EventProperty> event_properties = new List<SKDE_EventProperty>();  
    }
}
