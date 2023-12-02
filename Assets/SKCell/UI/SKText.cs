using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

namespace SKCell
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SKLocalization))]
    [RequireComponent(typeof(SKTextAnimator))]
    [AddComponentMenu("SKCell/UI/SKText")]
    public class SKText : TextMeshProUGUI
    {
        //Localization
        SKLocalization skLocal;
        //Animator
        //[HideInInspector]
        public SKTextAnimator textAnimator;
        //Dynamic arguments
        int[] intArgs;
        float[] floatArgs;
        string[] strArgs;

        protected override void Start()
        {
            base.Start();

        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }
        private void Initialize()
        {
            skLocal = SKUtils.GetComponentNonAlloc<SKLocalization>(gameObject);
            textAnimator = SKUtils.GetComponentNonAlloc<SKTextAnimator>(gameObject);
        }
        /// <summary>
        /// Change the text to newText. 
        /// </summary>
        /// <param name="newText"></param>
        public void UpdateTextDirectly(string newText)
        {
            if(textAnimator && Application.isPlaying)
                textAnimator.UpdateText(newText);
            else
                text = newText;
        }
        /// <summary>
        /// Change the localization ID to localID, then apply localization according to the current game language.
        /// </summary>
        /// <param name="localID"></param>
        public void UpdateLocalID( int localID )
        {
            if (!skLocal)
            {
                skLocal = SKUtils.GetComponentNonAlloc<SKLocalization>(gameObject);
            }
            skLocal.localID = localID;
            ApplyLocalization(SKEnvironment.curLanguage);
        }
        /// <summary>
        /// Change the localization ID to localID only.
        /// </summary>
        /// <param name="localID"></param>
        public void UpdateLocalIDRaw(int localID)
        {
            if (!skLocal)
            {
                skLocal = SKUtils.GetComponentNonAlloc<SKLocalization>(gameObject);
            }
            skLocal.localID = localID;
        }
        /// <summary>
        /// Update text to the current game language with one dynamic argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg0"></param>
        public void UpdateText<T>(T arg0)
        {
            UpdateArgs(arg0);
            ApplyLocalization(SKEnvironment.curLanguage);
        }

        /// <summary>
        /// Update text to the current game language with two dynamic arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        public void UpdateText<T>(T arg0, T arg1)
        {
            UpdateArgs(arg0, arg1);
            ApplyLocalization(SKEnvironment.curLanguage);
        }
        /// <summary>
        /// Update text to the current game language with three dynamic arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void UpdateText<T>(T arg0, T arg1, T arg2)
        {
            UpdateArgs(arg0, arg1, arg2);
            ApplyLocalization(SKEnvironment.curLanguage);
        }

        /// <summary>
        /// Apply localization.
        /// </summary>
        /// <param name="lang"></param>
        public void ApplyLocalization(LanguageSupport lang)
        {
            if (skLocal==null || skLocal.localID == -1)
                return;
            if (intArgs != null)
            {
                switch (intArgs.Length)
                {
                    case 1:
                        skLocal.ApplyLocalization<int>(lang, LocalizationType.Text, intArgs[0]);
                        break;
                    case 2:
                        skLocal.ApplyLocalization<int>(lang, LocalizationType.Text, intArgs[0], intArgs[1]);
                        break;
                    case 3:
                        skLocal.ApplyLocalization<int>(lang, LocalizationType.Text, intArgs[0], intArgs[1], intArgs[2]);
                        break;
                }
            }
            else if (floatArgs != null)
            {
                switch (floatArgs.Length)
                {
                    case 1:
                        skLocal.ApplyLocalization<float>(lang, LocalizationType.Text, floatArgs[0]);
                        break;
                    case 2:
                        skLocal.ApplyLocalization<float>(lang, LocalizationType.Text, floatArgs[0], floatArgs[1]);
                        break;
                    case 3:
                        skLocal.ApplyLocalization<float>(lang, LocalizationType.Text, floatArgs[0], floatArgs[1], floatArgs[2]);
                        break;
                }
            }
            else if (strArgs != null)
            {
                switch (strArgs.Length)
                {
                    case 1:
                        skLocal.ApplyLocalization<string>(lang, LocalizationType.Text, strArgs[0]);
                        break;
                    case 2:
                        skLocal.ApplyLocalization<string>(lang, LocalizationType.Text, strArgs[0], strArgs[1]);
                        break;
                    case 3:
                        skLocal.ApplyLocalization<string>(lang, LocalizationType.Text, strArgs[0], strArgs[1], strArgs[2]);
                        break;
                }
            }
            else
            {
                skLocal.ApplyLocalization(lang, LocalizationType.Text);
            }
        }

        #region Dynamic Args
        private void UpdateArgs<T>(T arg0)
        {
            if (arg0.GetType() == Type.GetType("System.Int32"))
            {
                ResetArgs(DynamicArgType.Int);
                intArgs = new int[1];
                intArgs[0] = int.Parse(arg0.ToString());
            }
            if (arg0.GetType() == Type.GetType("System.Single"))
            {
                ResetArgs(DynamicArgType.Float);
                floatArgs = new float[1];
                floatArgs[0] = float.Parse(arg0.ToString());
            }
            if (arg0.GetType() == Type.GetType("System.String"))
            {
                ResetArgs(DynamicArgType.String);
                strArgs = new string[1];
                strArgs[0] = arg0.ToString();
            }
        }
        private void UpdateArgs<T>(T arg0, T arg1)
        {
            if (arg0.GetType() == Type.GetType("System.Int32"))
            {
                ResetArgs(DynamicArgType.Int);
                intArgs = new int[2] { int.Parse(arg0.ToString()), int.Parse(arg1.ToString()) };
            }
            if (arg0.GetType() == Type.GetType("System.Single"))
            {
                ResetArgs(DynamicArgType.Float);
                floatArgs = new float[2] { float.Parse(arg0.ToString()) , float.Parse(arg1.ToString()) };
            }
            if (arg0.GetType() == Type.GetType("System.String"))
            {
                ResetArgs(DynamicArgType.String);
                strArgs = new string[2] { arg0.ToString(), arg1.ToString() };
            }
        }
        private void UpdateArgs<T>(T arg0, T arg1, T arg2)
        {
            if (arg0.GetType() == Type.GetType("System.Int32"))
            {
                ResetArgs(DynamicArgType.Int);
                intArgs = new int[3] { int.Parse(arg0.ToString()), int.Parse(arg1.ToString()), int.Parse(arg2.ToString()) };
            }
            if (arg0.GetType() == Type.GetType("System.Single"))
            {
                ResetArgs(DynamicArgType.Float);
                floatArgs = new float[3] { float.Parse(arg0.ToString()), float.Parse(arg1.ToString()), float.Parse(arg2.ToString()) };
            }
            if (arg0.GetType() == Type.GetType("System.String"))
            {
                ResetArgs(DynamicArgType.String);
                strArgs = new string[3] { arg0.ToString(), arg1.ToString(), arg2.ToString() };
            }
        }
        public void ResetArgs(DynamicArgType toExclude)
        {
            if (toExclude == DynamicArgType.Int)
            {
                floatArgs = null;
                strArgs = null;
            }
            else if (toExclude == DynamicArgType.Float)
            {
                intArgs = null;
                strArgs = null;
            }
            else if (toExclude == DynamicArgType.String)
            {
                intArgs = null;
                floatArgs = null;
            }
        }

        public enum DynamicArgType
        {
            Int,
            Float,
            String
        }

        #endregion

        #region PublicMethods
        public void InitiateNumberIncrement(float start, float end, float lerpSpeed, string format)
        {
            StopAllCoroutines();
            StartCoroutine(NumberIncrementCR(start, end, lerpSpeed, format));
        }

        IEnumerator NumberIncrementCR(float start, float end, float lerpSpeed, string format)
        {
            text = start.ToString(format);
            for (int i = 0; i < 20; i++)
            {
                text = Mathf.Lerp(float.Parse(text), end, lerpSpeed).ToString($"{format}");
                yield return new WaitForEndOfFrame();
            }
            text = end.ToString($"{format}");
        }

        public void InitiateNumberIncrementPercentage(float start, float end, float lerpSpeed, int deci)
        {
            StopAllCoroutines();
            StartCoroutine(NumberIncrementPercentageCR(start, end, lerpSpeed, deci));
        }

        IEnumerator NumberIncrementPercentageCR(float start, float end, float lerpSpeed, int deci)
        {
            float num = start;

            text = start.ToString($"p{deci}");
            for (int i = 0; i < 20; i++)
            {
                num = Mathf.Lerp(num, end, lerpSpeed);
                text = num.ToString($"p{deci}");
                yield return new WaitForEndOfFrame();
            }
            text = end.ToString($"p{deci}");
        }

        #endregion
    }

}
