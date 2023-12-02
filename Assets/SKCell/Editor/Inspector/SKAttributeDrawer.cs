using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SKCell
{
    [CustomPropertyDrawer(typeof(SKConditionalFieldAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldHide(property))
                return 0;

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldHide(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool ShouldHide(SerializedProperty property)
        {
            SKConditionalFieldAttribute hideAttribute = (SKConditionalFieldAttribute)attribute;
            string propertyName = hideAttribute.ConditionalField;
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(propertyName);

            if (sourcePropertyValue == null)
            {
                Debug.LogWarning($"Cannot find property named {propertyName}");
                return false;
            }

            if (sourcePropertyValue.propertyType == SerializedPropertyType.Boolean)
            {
                return sourcePropertyValue.boolValue != (bool)hideAttribute.ConditionalValue;
            }
            return false;
        }
    }

    [CustomPropertyDrawer(typeof(SKResettableAttribute))]
    public class ResettablePropertyDrawer : PropertyDrawer
    {
        private static Dictionary<string, object> initialValues = new Dictionary<string, object>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialValues.ContainsKey(property.propertyPath))
            {
                initialValues[property.propertyPath] = GetPropertyValue(property);
            }

            float buttonWidth = 14f;
            Rect propertyRect = new Rect(position.x, position.y, position.width - buttonWidth - 4, position.height);
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth - 2, position.y + 2, buttonWidth, position.height);

            EditorGUI.PropertyField(propertyRect, property, label);

            if (GUI.Button(buttonRect, SKHierarchy.resetContent, SKHierarchy.transparentButtonStyle))
            {
                ResetProperty(property);
            }
        }

        private void ResetProperty(SerializedProperty property)
        {
            if (initialValues.ContainsKey(property.propertyPath))
            {
                SetPropertyValue(property, initialValues[property.propertyPath]);
            }
        }

        private object GetPropertyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer: return property.intValue;
                case SerializedPropertyType.Boolean: return property.boolValue;
                case SerializedPropertyType.Float: return property.floatValue;
                case SerializedPropertyType.String: return property.stringValue;
                case SerializedPropertyType.Color: return property.colorValue;
                case SerializedPropertyType.ObjectReference: return property.objectReferenceValue;
                case SerializedPropertyType.Vector2: return property.vector2Value;
                case SerializedPropertyType.Vector3: return property.vector3Value;
                case SerializedPropertyType.Vector4: return property.vector4Value;
                // ... Handle other types as needed
                default: return null;
            }
        }

        private void SetPropertyValue(SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer: property.intValue = (int)value; break;
                case SerializedPropertyType.Boolean: property.boolValue = (bool)value; break;
                case SerializedPropertyType.Float: property.floatValue = (float)value; break;
                case SerializedPropertyType.String: property.stringValue = (string)value; break;
                case SerializedPropertyType.Color: property.colorValue = (Color)value; break;
                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = value as UnityEngine.Object; break;
                case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; break;
                case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; break;
                case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; break;
                    // ... Handle other types as needed
            }
        }
    }


    [CustomPropertyDrawer(typeof(SKFieldAliasAttribute))]
    public class SKFieldAliasDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SKFieldAliasAttribute textAttribute = (SKFieldAliasAttribute)attribute;

            EditorGUI.LabelField(position, textAttribute.Message);
            EditorGUI.PropertyField(position, property, true);
        }
    }

    [CustomPropertyDrawer(typeof(SKInspectorTextAttribute))]
    public class SKInspectorTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SKInspectorTextAttribute textAttribute = (SKInspectorTextAttribute)attribute;

            position.height = GetPropertyHeight(property, label) * 2;
            EditorGUI.LabelField(position, textAttribute.Message);
            EditorGUILayout.Space(GetPropertyHeight(property, label));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
