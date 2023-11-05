using System;
using UnityEngine;
namespace SKCell
{
    /// <summary>
    /// Start a folder in the inspector.
    /// </summary>
    public class SKFolderAttribute : PropertyAttribute
    {
        public string name;
        public bool foldout;
        public SKFolderAttribute(string name)
        {
            this.name = name;
            this.foldout = true;
        }
    }

    /// <summary>
    /// End the current folder.
    /// </summary>
    public class SKEndFolderAttribute : PropertyAttribute
    {
        public SKEndFolderAttribute(){}
    }

    /// <summary>
    /// Make a button in the inspector that executes a function.
    /// </summary>
    public class SKInspectorButtonAttribute : Attribute
    {
        public string name;
        public bool onTop = true;
        public SKInspectorButtonAttribute(string name, bool onTop=true)
        {
            this.name = name;
            this.onTop = onTop;
        }
    }

    /// <summary>
    /// Display the field in the inspector only if the specified field matches the value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SKConditionalFieldAttribute : PropertyAttribute
    {
        public string ConditionalField { get; }
        public object ConditionalValue { get; }

        public SKConditionalFieldAttribute(string conditionalField, object conditionalValue)
        {
            ConditionalField = conditionalField;
            ConditionalValue = conditionalValue;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SKResettableAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SKSeparatorAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Instead of the field name, display the specidied text in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class SKFieldAliasAttribute : PropertyAttribute
    {
        public string Message { get; }

        public SKFieldAliasAttribute(string message)
        {
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SKInspectorTextAttribute : PropertyAttribute
    {
        public string Message { get; }

        public SKInspectorTextAttribute(string message)
        {
            Message = message;
        }
    }
}