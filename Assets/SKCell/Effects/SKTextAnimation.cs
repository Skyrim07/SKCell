using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SKCell
{
    public class SKTextAnimation : MonoBehaviour
    {
        private TMP_Text text;
        private TMP_TextInfo textInfo;
        [HideInInspector]
        public SKTextAnimator animator;

        SKTextData textData;
        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            animator = GetComponent<SKTextAnimator>();
            textInfo = text.textInfo;
        }
        public void InitializeData()
        {
            textData = new SKTextData();
            textData.ConstructCharData(text);
         }

        /// <summary>
        /// Apply custom text data to TMP_Text.
        /// </summary>
        /// <param name="data"></param>
        public void ApplyTextData(SKTextData data)
        {
            for(int i = 0; i < data.charData.Length; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    textInfo.meshInfo[0].vertices[i * 4 + j] = data.charData[i].vertices[j];
                    textInfo.meshInfo[0].colors32[i * 4 + j] = data.charData[i].colors32[j];
                }
            }
            text.UpdateVertexData();
        }

        #region Effect Presets
        public void Banner(float speed, Color32 color, int startIndex, int endIndex)
        {
            speed *= 2;
            SKTextEffect e = new SKTextEffect()
            {
                type = SKTextEffectType.Rainbow,
                rainbow_Frequency = speed,
                color = color,
                startIndex = startIndex,
                endIndex = endIndex
            };
            SKTextUtils.ApplyTextEffect(text, textData, e);
        }
        public void Fade(float speed, int startIndex, int endIndex)
        {
            speed *= 0.25f;
            Alpha(speed,0,startIndex,endIndex);
        }
        public void Twinkle(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e = new SKTextEffect()
            {
                type = SKTextEffectType.Twinkle,
                time = 500,
                curve = SKCurve.LinearIn,
                twinkle_Frequency = 2 * speed,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e);
        }
        public void TwinkleSlow(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e = new SKTextEffect()
            {
                type = SKTextEffectType.Twinkle,
                time = 500,
                curve = SKCurve.LinearIn,
                twinkle_Frequency = 1.4f * speed,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e);
        }
        public void Dangle(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e = new SKTextEffect()
            {
                type = SKTextEffectType.Rotation,
                angle_deg = 15,
                rot_center = new Vector3(1, 1, 0),
                mode = SKTextAnimMode.OneWay,
                time = (0.5f / speed) * (endIndex - startIndex + 1),
                curve = SKCurve.BounceIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e);
        }
        public void Exclaim(float speed, int startIndex, int endIndex)
        {
            speed /= 2;
            ScaleUp(speed*8, startIndex, endIndex);
            SKUtils.InvokeAction(0, () =>
             {
                 Shake(startIndex, endIndex);
             });
        }
        public void ExclaimTimed(float speed, float time, int startIndex, int endIndex)
        {
            speed /= 2;
            ScaleUp(speed * 8, startIndex, endIndex);
            SKUtils.InvokeAction(0, () =>
            {
                ShakeTimed(time + (0.003f / speed) * (endIndex - startIndex + 1), startIndex, endIndex);
                SKUtils.InvokeAction(time, () =>
                {
                    ScaleIndentity(speed * 4, startIndex, endIndex);
                });
            });
        }
        public void ExclaimColor(float speed, Color32 color,  int startIndex, int endIndex)
        {
            speed /= 2;
            ScaleUp(speed * 8, startIndex, endIndex);
            Color(speed*8, color, startIndex, endIndex);
            SKUtils.InvokeAction(0, () =>
            {
                Shake(startIndex, endIndex);
            });
        }
        public void ExclaimColorTimed(float speed, float time, Color32 color, int startIndex, int endIndex)
        {
            speed /= 2;
            ScaleUp(speed * 8, startIndex, endIndex);
            Color(speed * 8, color, startIndex, endIndex);
            SKUtils.InvokeAction(0, () =>
            {
                ShakeTimed(time + (0.003f / speed) * (endIndex - startIndex + 1), startIndex, endIndex);
                SKUtils.InvokeAction(time, () =>
                {
                    ScaleIndentity(speed * 4, startIndex, endIndex);
                    SKUtils.InvokeAction((0.007f / speed) * (endIndex - startIndex + 1), () =>
                    {
                        SKTextUtils.Set_Original_Color(text, textData);
                    });
                });
            });
        }
        public void WaveLoop(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Wave,
                timePerChar = 0.2f / speed,
                interval = 0.15f / speed,
                startIndex = startIndex,
                endIndex = endIndex,
                loop = true,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void Wave(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Wave,
                timePerChar = 0.2f/speed,
                interval = 0.15f/speed,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void Shake(int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Shake,
                shake_Amplitude = 0.4f,
                shake_Frequency =40f,
                time = 500000,

                startIndex = startIndex,
                endIndex=endIndex,
            };

            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ShakeTimed(float time, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Shake,
                shake_Amplitude = 0.4f,
                shake_Frequency = 40f,
                time = time,

                startIndex = startIndex,
                endIndex = endIndex,
            };

            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ShakeSmooth(int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Shake,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.Anim_Shake(text, textData, startIndex, endIndex, 0.65f, 7, 500000, SKTextAnimMode.OneWay, SKCurve.LinearIn);
        }
        public void ScaleUpPingPong(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                scale = 1.4f,
                type = SKTextEffectType.Scaling,
                time= 0.14f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.SineDoubleIn,
                startIndex = startIndex,
                endIndex  = endIndex,
                mode = SKTextAnimMode.PingPong,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ScaleDownPingPong(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                scale = 0.8f,
                type = SKTextEffectType.Scaling,
                time = 0.14f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
                mode = SKTextAnimMode.PingPong,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ScaleUp(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                scale = 1.4f,
                type = SKTextEffectType.Scaling,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ScaleDown(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                scale = 0.8f,
                type = SKTextEffectType.Scaling,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ScaleIndentity(float speed, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                scale = 1f,
                type = SKTextEffectType.Scaling,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void Rotate(float speed, float angle_deg, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Rotation,
                angle_deg = -angle_deg,
                time = 0.1f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
                mode = SKTextAnimMode.OneWay,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void RotatePingPong(float speed, float angle_deg, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Rotation,
                angle_deg = -angle_deg,
                time = 0.2f / speed * (endIndex-startIndex+1),
                curve = SKCurve.LinearIn,
                startIndex=startIndex,
                endIndex = endIndex,
                mode = SKTextAnimMode.PingPong,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void Color(float speed, Color32 color, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Color,
                color = color,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void ColorPingPong(float speed, Color32 color, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Color,
                color = color,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
                mode=SKTextAnimMode.PingPong,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void Alpha(float speed, float alpha, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Alpha,
                alpha = alpha,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void FadeIn(float speed, float alpha, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Alpha,
                alpha = alpha,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearOut,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        public void Translate(float speed, Vector3 delta, int startIndex, int endIndex)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Translation,
                translation_delta = delta,
                time = 0.03f / speed * (endIndex - startIndex + 1),
                curve = SKCurve.LinearIn,
                startIndex = startIndex,
                endIndex = endIndex,
            };
            SKTextUtils.ApplyTextEffect(text, textData, e1);
        }
        

        /// <summary>
        /// Standard alpha fade typewriter.
        /// </summary>
        /// <param name="speed"></param>
        public void TypeWriter(float speed = 1.0f)
        {
            SKTextUtils.TypeWriter_Standard_Effects(this, text, textData, 0.3f / speed, 0.15f / speed);
        }
        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriterScaling(float speed = 1.0f)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                scale = 1.5f,
                type = SKTextEffectType.Scaling,
                time = 5/speed,
                curve = SKCurve.LinearOut,
                isTypewriter = true
            };
            TypeWriter(speed, e1);
        }
        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriterTranslating(float speed = 1.0f)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                translation_delta = Vector3.left * 10,
                type = SKTextEffectType.Translation,
                time = 5 / speed,
                curve = SKCurve.LinearOut
            };
            TypeWriter(speed, e1);
        }
        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriterRotating(float speed = 1.0f)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Rotation,
                angle_deg=60,
                curve = SKCurve.QuadraticOut
            };
            TypeWriter(speed, e1);
        }
        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriterShaking(float speed = 1.0f)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Shake,
            };
            TypeWriter(speed, e1);
        }
        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriterWave(float speed = 1.0f)
        {
            SKTextEffect e1 = new SKTextEffect()
            {
                type = SKTextEffectType.Wave,
               timePerChar=0.2f,
               interval=0.15f,
            };
            TypeWriter(speed, e1);
        }
        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriter(float speed = 1.0f, params SKTextEffect[] effects)
        {
            SKTextUtils.TypeWriter_Standard_Effects(this, text, textData, 0.3f/speed, 0.15f/speed, effects);
        }
        /// <summary>
        /// Custom effects typewriter.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="effects"></param>
        public void TypeWriter_Custom(float speed = 1.0f, params SKTextEffect[] effects)
        {
            SKTextUtils.TypeWriter_Effects(text, textData, 0.3f / speed, 0.15f / speed, effects);
        }
        #endregion

        public void UpdateTextInfo()
        {
            textInfo = text.textInfo;
            InitializeData();
        }
        public void UpdateTextInfo(string s)
        {
            if(text)
            textInfo = text.GetTextInfo(s);
            InitializeData();
        }
        public void UpdateTextDataColor(Color32 c)
        {
            if (textData == null)
                return;
            for (int i = 0; i < textData.charData.Length; i++)
            {
                for (int x = 0; x < 4; x++)
                {
                    textData.charData[i].colors32[x] = c;
                    textData.charData[i].oColors32[x] = c;
                }
            }
        }
    }
}
