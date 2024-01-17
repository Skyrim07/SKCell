using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public static class SKExtensions
    {
        #region TransformExt
        /// <summary>
        /// Reset the transform to default
        /// </summary>
        /// <param name="tf"></param>
        public static void ResetTransform(this Transform tf, bool isLocal)
        {
            if (!isLocal)
            {
                tf.position = Vector3.zero;
                tf.localScale = Vector3.one;
                tf.rotation = Quaternion.identity;
            }
            else
            {
                tf.localPosition = Vector3.zero;
                tf.localScale = Vector3.one;
                tf.localRotation = Quaternion.identity;
            }
        }
        public static void ResetPosition(this Transform tf, bool isLocal)
        {
            if (!isLocal)
            {
                tf.position = Vector3.zero;
            }
            else
            {
                tf.localPosition = Vector3.zero;
            }
        }

        public static void ResetLocalScale(this Transform tf)
        {
            tf.localScale = Vector3.one;
        }

        public static void ResetLocalRotation(this Transform tf)
        {
            tf.localRotation = Quaternion.identity;
        }
        public static void ResetGlobalRotation(this Transform tf)
        {
            tf.rotation = Quaternion.identity;
        }

        public static void SetPositionX(this Transform tf, float x)
        {
            tf.position = new Vector3(x, tf.position.y, tf.position.z);
        }
        public static void SetPositionY(this Transform tf, float y)
        {
            tf.position = new Vector3(tf.position.x, y, tf.position.z);
        }
        public static void SetPositionZ(this Transform tf, float z)
        {
            tf.position = new Vector3(tf.position.x, tf.position.y, z);
        }
        public static void SetRotationX(this Transform tf, float x)
        {
            tf.rotation = Quaternion.Euler(x, tf.rotation.eulerAngles.y, tf.rotation.eulerAngles.z);
        }
        public static void SetRotationY(this Transform tf, float y)
        {
            tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, y, tf.rotation.eulerAngles.z);
        }
        public static void SetRotationZ(this Transform tf, float z)
        {
            tf.rotation = Quaternion.Euler(tf.rotation.eulerAngles.x, tf.rotation.eulerAngles.y, z);
        }
        public static void SetScaleX(this Transform tf, float x)
        {
            tf.localScale = new Vector3(x, tf.localScale.y, tf.localScale.z);
        }
        public static void SetScaleY(this Transform tf, float y)
        {
            tf.localScale = new Vector3(tf.localScale.x,y, tf.localScale.z);
        }
        public static void SetScaleZ(this Transform tf, float z)
        {
            tf.localScale = new Vector3(tf.localScale.x, tf.localScale.y, z);
        }
        public static Vector3 Get2DPosition(this Transform tf)
        {
            return new Vector3(tf.position.x, tf.position.y, 0);
        }

        /// <summary>
        /// Set the transform value from another transform
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="tf">The other transform</param>
        public static void CopyFrom(this Transform selfTf, Transform otherTf)
        {
            selfTf.position = otherTf.position;
            selfTf.localScale = otherTf.localScale;
            selfTf.rotation = otherTf.rotation;
        }
        /// <summary>
        /// Return an enumerable list of all the childs of the given transform
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static List<Transform> GetAllChildren(this Transform tf)
        {
            List<Transform> result = new List<Transform>();
            GetAllChildrenHelper(tf, result);
            return result;
        }
        public static List<Transform> GetAllChildren(this Transform tf, List<Transform> result)
        {
            GetAllChildrenHelper(tf, result);
            return result;
        }
        private static void GetAllChildrenHelper(Transform tf, List<Transform> curRes)
        {
            if (tf.childCount == 0)
                return;
            for (int i = 0; i < tf.childCount; i++)
            {
                curRes.Add(tf.GetChild(i));
                GetAllChildrenHelper(tf.GetChild(i), curRes);
            }
        }
        public static void ClearChildren(this Transform tf)
        {
            if (tf == null) return;
            for (int i = 0; i < tf.childCount; i++)
            {
                if(tf.GetChild(i))
                SKUtils.Destroy(tf.GetChild(i).gameObject);
            }
        }
        public static void ClearChildrenImmediate(this Transform tf)
        {
            if (tf == null) return;
            for (int i = 0; i < tf.childCount; i++)
            {
                if (tf.GetChild(i))
                    GameObject.DestroyImmediate(tf.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// Create an empty child gameobject
        /// </summary>
        /// <param name="tf"></param>
        /// <returns>Transform of the child</returns>
        public static Transform CreateChild(this Transform tf)
        {
            GameObject child = new GameObject("Child");
            child.transform.SetParent(tf);
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.localRotation = Quaternion.identity;
            return child.transform;
        }
        public static Transform CreateChild(this Transform tf, string name)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(tf);
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.localRotation = Quaternion.identity;
            return child.transform;
        }
        /// <summary>
        /// Create a parent with child's world transform unchanged
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static Transform CreateParent(this Transform tf)
        {
            GameObject parent = new GameObject("Parent");
            tf.SetParent(parent.transform, true);
            return parent.transform;
        }
        public static Transform CreateParent(this Transform tf, string name)
        {
            GameObject parent = new GameObject(name);
            tf.SetParent(parent.transform, true);
            return parent.transform;
        }
        public static void AttachChild(this Transform tf, Transform otherTF, bool resetChildTF)
        {
            otherTF.SetParent(tf);
            if (resetChildTF)
                otherTF.ResetPosition(true);
        }
        /// <summary>
        /// Swap sibling order with another sibling
        /// </summary>
        /// <param name="selfTf"></param>
        /// <param name="otherTf"></param>
        public static void SwapSiblingOrder(this Transform selfTf, Transform otherTf)
        {
            if (selfTf.parent != otherTf.parent)
            {
                SKUtils.EditorLogWarning("Trying to swap sibling order with transform under a different object!");
                return;
            }
            int o1 = selfTf.GetSiblingIndex();
            int o2 = otherTf.GetSiblingIndex();
            selfTf.SetSiblingIndex(o2);
            otherTf.SetSiblingIndex(o1);
        }
        public static bool Contains(this RectTransform a, RectTransform b)
        {
            Rect worldRect = a.WorldRect();
            return worldRect.Contains(b.WorldRect().min) && worldRect.Contains(b.WorldRect().max);
        }
        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            return a.WorldRect().Overlaps(b.WorldRect());
        }
        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
        {
            return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
        }

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
        }
        #endregion

        #region AnimatorExt
        /// <summary>
        ///  anim.SetBool("Appear", true);
        /// </summary>
        /// <param name="anim"></param>
        public static void Appear(this Animator anim)
        {
            anim.SetBool("Appear", true);
        }
        /// <summary>
        /// anim.SetBool("Appear", false);
        /// </summary>
        /// <param name="anim"></param>
        public static void Disappear(this Animator anim)
        {
            anim.SetBool("Appear", false);
        }
        public static void Pop(this Animator anim)
        {
            anim.ResetTrigger("Pop");
            anim.SetTrigger("Pop");
        }

        #endregion

        #region ColorExt
        public static Color32 ToColor32(this Color col)
        {
            return new Color32((byte)(col.r * 255f), (byte)(col.g * 255f), (byte)(col.b * 255f), (byte)(col.a * 255f));
        }

        public static void SetQuad(this Texture2D t, Vector2Int lb, Vector2Int rt, Color color)
        {
            for (int i = lb.x; i < rt.x; i++)
            {
                for (int j = lb.y; j < rt.y; j++)
                {
                    if (i >= t.width || j >= t.height)
                        continue;
                    if (i < 0 || j < 0)
                        continue;
                    t.SetPixel(i, j, color);
                }
            }
        }
        public static void AddQuad(this Texture2D t, Vector2Int lb, Vector2Int rt, Color color)
        {
            for (int i = lb.x; i < rt.x; i++)
            {
                for (int j = lb.y; j < rt.y; j++)
                {
                    if (i >= t.width || j >= t.height)
                        continue;
                    if (i <0 || j <0)
                        continue;
                    t.SetPixel(i, j, color+t.GetPixel(i,j));
                }
            }
        }
        public static void MultiplyQuad(this Texture2D t, Vector2Int lb, Vector2Int rt, Color color)
        {
            for (int i = lb.x; i < rt.x; i++)
            {
                for (int j = lb.y; j < rt.y; j++)
                {
                    if (i >= t.width || j >= t.height)
                        continue;
                    if (i < 0 || j < 0)
                        continue;
                    t.SetPixel(i, j, color * t.GetPixel(i, j));
                }
            }
        }
        public static void SetColor(this Texture2D t, Color color)
        {
            for (int i = 0; i < t.width; i++)
            {
                for (int j = 0; j < t.height; j++)
                {
                    t.SetPixel(i, j, color);
                }
            }
        }

        public static float Distance(this Color color, Color other)
        {
            return Mathf.Abs(color.r - other.r) + Mathf.Abs(color.g - other.g) + Mathf.Abs(color.b - other.b);
        }
        public static float Luminance(this Color color)
        {
            return color.r * 0.2125f + color.g * 0.7154f + color.b * 0.0721f;
        }
        public static Color Saturate(this Color color)
        {
            return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), Mathf.Clamp01(color.a));
        }
        #endregion

        #region BaseExt
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        /// <summary>
        /// Fill the list with <count> copies of content. *List will be cleared at first.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="content"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> PopulateList<T>(this List<T> list, T content, int count)
        {
            if (list == null)
                list = new List<T>();
            list.Clear();
            for (int i = 0; i < count; i++)
            {
                list.Add(content);
            }
            return list;
        }
        /// <summary>
        /// Fill the array with <count> copies of content. *Array will be cleared and resized at first.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="content"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] PopulateArray<T>(this T[] array, T content, int count)
        {
            if (array == null)
                array = new T[count];
            Array.Resize<T>(ref array, count);
            for (int i = 0; i < count; i++)
            {
                array[i] = content;
            }
            return array;
        }

        public static float SimpleDistance(this Vector3 v, Vector3 v1)
        {
            return Mathf.Abs(v.x - v1.x) + Mathf.Abs(v.y - v1.y) + Mathf.Abs(v.z - v1.z);
        }

        public static float SimpleDistanceSigned(this Vector3 v, Vector3 v1)
        {
            return v.x - v1.x + v.y - v1.y + v.z - v1.z;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T _comp = go.GetComponent<T>();
            if (_comp == null)
            {
                return go.AddComponent<T>();
            }
            return _comp;
        }

        #endregion

    }
}
