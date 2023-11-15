using System;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Transform))]
    [CanEditMultipleObjects]
    public class SKTransformEditorExt : Editor
    {
        Editor defaultEditor;
        Transform transform;
        public bool showTools;
        public bool copyPosition;
        public bool copyRotation;
        public bool copyScale;
        public bool pastePosition;
        public bool pasteRotation;
        public bool pasteScale;
        public bool selectionNullError;

        void OnDisable()
        {
            MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (disableMethod != null)
                disableMethod.Invoke(defaultEditor, null);
            DestroyImmediate(defaultEditor);
        }

        public override void OnInspectorGUI()
        {
            Transform t = (Transform)target;

            EditorGUILayout.LabelField("Local Space", EditorStyles.boldLabel);
            defaultEditor.OnInspectorGUI();

            //Show World Space Transform
            EditorGUILayout.Space();

            if (GUILayout.Button((showTools) ? "< Hide Transform Ext >" : "< Show Transform Ext >"))
            {
                showTools = !showTools;
                EditorPrefs.SetBool("ShowTools", showTools);
            }

            if (showTools)
            {
                if (!copyPosition && !copyRotation && !copyScale)
                {
                    selectionNullError = true;
                }
                else
                {
                    selectionNullError = false;
                }
                EditorGUILayout.LabelField("World Space", EditorStyles.boldLabel);

                GUI.enabled = false;
                Vector3 localPosition = transform.localPosition;
                transform.localPosition = transform.position;

                Quaternion localRotation = transform.localRotation;
                transform.localRotation = transform.rotation;

                Vector3 localScale = transform.localScale;
                transform.localScale = transform.lossyScale;

                defaultEditor.OnInspectorGUI();
                transform.localPosition = localPosition;
                transform.localRotation = localRotation;
                transform.localScale = localScale;
                GUI.enabled = true;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(selectionNullError ? "Nothing Selected" : "Copy Transform"))
                {
                    if (copyPosition)
                    {
                        EditorPrefs.SetFloat("LocalPosX", t.localPosition.x);
                        EditorPrefs.SetFloat("LocalPosY", t.localPosition.y);
                        EditorPrefs.SetFloat("LocalPosZ", t.localPosition.z);
                    }
                    if (copyRotation)
                    {
                        EditorPrefs.SetFloat("LocalRotX", t.localEulerAngles.x);
                        EditorPrefs.SetFloat("LocalRotY", t.localEulerAngles.y);
                        EditorPrefs.SetFloat("LocalRotZ", t.localEulerAngles.z);
                    }
                    if (copyScale)
                    {
                        EditorPrefs.SetFloat("LocalScaleX", t.localScale.x);
                        EditorPrefs.SetFloat("LocalScaleY", t.localScale.y);
                        EditorPrefs.SetFloat("LocalScaleZ", t.localScale.z);
                    }

                    Debug.Log("LP: " + t.localPosition + " LT: (" + t.localEulerAngles.x + ", " + t.localEulerAngles.y + ", " + t.localEulerAngles.z + ") LS: " + t.localScale);
                }
                if (GUILayout.Button("Paste Transform"))
                {
                    Vector3 tV3 = new Vector3();
                    if (pastePosition)
                    {
                        tV3.x = EditorPrefs.GetFloat("LocalPosX", 0.0f);
                        tV3.y = EditorPrefs.GetFloat("LocalPosY", 0.0f);
                        tV3.z = EditorPrefs.GetFloat("LocalPosZ", 0.0f);
                        t.localPosition = tV3;
                    }
                    if (pasteRotation)
                    {
                        tV3.x = EditorPrefs.GetFloat("LocalRotX", 0.0f);
                        tV3.y = EditorPrefs.GetFloat("LocalRotY", 0.0f);
                        tV3.z = EditorPrefs.GetFloat("LocalRotZ", 0.0f);
                        t.localEulerAngles = tV3;
                    }
                    if (pasteScale)
                    {
                        tV3.x = EditorPrefs.GetFloat("LocalScaleX", 1.0f);
                        tV3.y = EditorPrefs.GetFloat("LocalScaleY", 1.0f);
                        tV3.z = EditorPrefs.GetFloat("LocalScaleZ", 1.0f);
                        t.localScale = tV3;
                    }

                    Debug.Log("LP: " + t.localPosition + " LT: " + t.localEulerAngles + " LS: " + t.localScale);
                }
                EditorGUILayout.EndHorizontal();

                //EditorGUIUtility.LookLikeControls();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Position", GUILayout.Width(75));
                GUILayout.Label("Rotation", GUILayout.Width(75));
                GUILayout.Label("Scale", GUILayout.Width(50));
                if (GUILayout.Button("All", GUILayout.MaxWidth(40))) TransformCopyAll();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                copyPosition = EditorGUILayout.Toggle(copyPosition, GUILayout.Width(75));
                copyRotation = EditorGUILayout.Toggle(copyRotation, GUILayout.Width(65));
                copyScale = EditorGUILayout.Toggle(copyScale, GUILayout.Width(45));
                if (GUILayout.Button("None", GUILayout.MaxWidth(40))) TransformCopyNone();
                EditorGUILayout.EndHorizontal();
                //EditorGUIUtility.LookLikeInspector();
            }
        }
        private Vector3 FixIfNaN(Vector3 v)
        {
            if (float.IsNaN(v.x))
            {
                v.x = 0;
            }
            if (float.IsNaN(v.y))
            {
                v.y = 0;
            }
            if (float.IsNaN(v.z))
            {
                v.z = 0;
            }
            return v;
        }

        void OnEnable()
        {
            defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
            transform = target as Transform;
            showTools = EditorPrefs.GetBool("ShowTools", false);
            copyPosition = EditorPrefs.GetBool("Copy Position", true);
            copyRotation = EditorPrefs.GetBool("Copy Rotation", true);
            copyScale = EditorPrefs.GetBool("Copy Scale", true);
            pastePosition = EditorPrefs.GetBool("Paste Position", true);
            pasteRotation = EditorPrefs.GetBool("Paste Rotation", true);
            pasteScale = EditorPrefs.GetBool("Paste Scale", true);
        }

        void TransformCopyAll()
        {
            copyPosition = true;
            copyRotation = true;
            copyScale = true;
            GUI.changed = true;
        }

        void TransformCopyNone()
        {
            copyPosition = false;
            copyRotation = false;
            copyScale = false;
            GUI.changed = true;
        }

        void SetCopyPasteBools()
        {
            pastePosition = copyPosition;
            pasteRotation = copyRotation;
            pasteScale = copyScale;

            EditorPrefs.SetBool("Copy Position", copyPosition);
            EditorPrefs.SetBool("Copy Rotation", copyRotation);
            EditorPrefs.SetBool("Copy Scale", copyScale);
            EditorPrefs.SetBool("Paste Position", pastePosition);
            EditorPrefs.SetBool("Paste Rotation", pasteRotation);
            EditorPrefs.SetBool("Paste Scale", pasteScale);
        }
    }
#endif
}
