using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SKCell
{
    /// <summary>
    /// Custom MonoBehaviour inspector.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class SKMonoBehaviourEditor : Editor
    {
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
        private Stack<bool> foldoutStack = new Stack<bool>();

        GUIStyle boldFoldoutStyle, boldButtonStyle;


        public Texture foldoutIcon, foldoutIcon_Open;
        private void OnEnable()
        {
            foldoutIcon = SKAssetLibrary.LoadTexture($"ObjectIcons/obj_icon_1");
            foldoutIcon_Open = SKAssetLibrary.LoadTexture($"ObjectIcons/obj_icon_1_1");
        }
        public override void OnInspectorGUI()
        {
            InitializeGUIStyles();
            serializedObject.Update();

            //Function Attributes
            var mono = target as MonoBehaviour;
            var methods = mono.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            bool hasButton = false;
            foreach (var method in methods)
            {
                var buttons = method.GetCustomAttributes(typeof(SKInspectorButtonAttribute), true);
                if (buttons.Length > 0)
                {
                    hasButton = true;
                    var btn = buttons[0] as SKInspectorButtonAttribute;
                    if (GUILayout.Button($"< {btn.name} >", boldButtonStyle))
                    {
                        method.Invoke(mono, null);
                    }
                }
            }
            if (hasButton) {
                EditorGUILayout.Space(3);
            }

            //Property Attributes
            foldoutStack = new Stack<bool>();
            SerializedProperty iterator = serializedObject.GetIterator();

            while (iterator.NextVisible(true))
            {
                if (iterator.serializedObject == null || iterator.serializedObject.targetObject == null)
                    return;
                FieldInfo fieldInfo = iterator.serializedObject.targetObject.GetType().GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                SKFolderAttribute folderAttr = fieldInfo != null ? Attribute.GetCustomAttribute(fieldInfo, typeof(SKFolderAttribute)) as SKFolderAttribute : null;
                SKEndFolderAttribute endFolderAttr = fieldInfo != null ? Attribute.GetCustomAttribute(fieldInfo, typeof(SKEndFolderAttribute)) as SKEndFolderAttribute : null;
                SKSeparatorAttribute separatorAttr = fieldInfo != null ? Attribute.GetCustomAttribute(fieldInfo, typeof(SKSeparatorAttribute)) as SKSeparatorAttribute : null;



                if (endFolderAttr != null)
                {
                    if (foldoutStack.Count > 0)
                    {
                        foldoutStack.Pop();
                    }
                }


                if (folderAttr != null)
                {
                    if (foldoutStack.Count > 0)
                    {
                        foldoutStack.Pop();
                    }

                    if (!foldoutStates.ContainsKey(folderAttr.name))
                    {
                        foldoutStates.Add(folderAttr.name, true);
                    }
                    foldoutStates[folderAttr.name] = EditorGUILayout.Foldout(foldoutStates[folderAttr.name], "     "+ folderAttr.name, true, boldFoldoutStyle);
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.y -= 20;
                    rect.x -= 20;
                    rect.width += 1000;
                    EditorGUI.DrawRect(rect, new Color(1, 1, 1, .07f));
                    EditorGUILayout.Space(-20);

                    rect.y +=0;
                    rect.height = .8f;
                    EditorGUI.DrawRect(rect, new Color(1, 1, 1, .2f));

                    rect.width = rect.height = 16;
                    rect.x += 19;
                    GUI.DrawTexture(rect, foldoutStates[folderAttr.name]? foldoutIcon_Open: foldoutIcon);

                    foldoutStack.Push(foldoutStates[folderAttr.name]);
                }

                if (separatorAttr != null)
                {
                    EditorGUILayout.Space(5);
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.height = 1.5f;
                    rect.x -= 20;
                    rect.width += 200;

                    EditorGUI.DrawRect(rect, new Color(.9f, .8f, .7f, .2f));
                    EditorGUILayout.Space(-10);
                }


                if (foldoutStack.Count == 0 || foldoutStack.Peek())
                {
                    if((foldoutStack.Count > 0) && foldoutStack.Peek())
                        EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(iterator, true);

                    //Skip over child properties when displaying collections, events, etc.
                    int childrenCount = GetChildrenCount(iterator);
                    childrenCount--;
                    for (int i = 0; i < childrenCount; i++)
                    {
                        iterator.NextVisible(true);
                    }
                }
                EditorGUI.indentLevel = 0;
            }
            serializedObject.ApplyModifiedProperties();
        }
        internal static int GetChildrenCount(SerializedProperty property)
        {
            int count = 0;
            SerializedProperty iterator = property.Copy();
            var end = iterator.GetEndProperty();
            while (!SerializedProperty.EqualContents(iterator, end))
            {
                count++;
                iterator.NextVisible(true);
            }
            return count;
        }
        private void InitializeGUIStyles()
        {
            boldFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            boldFoldoutStyle.fontStyle = FontStyle.Bold;
            Color c = new Color(.9f, .8f, .7f);
            boldFoldoutStyle.normal.textColor = c;
            boldFoldoutStyle.onNormal.textColor = c;

            boldButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
            boldButtonStyle.fontStyle = FontStyle.Bold;
            boldButtonStyle.normal.textColor = c;
        }
    }
}