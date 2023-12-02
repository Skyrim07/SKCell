using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SKCell
{
    public static class SKTextUtils
    {
        public static Dictionary<TMP_Text, List<string>> CRids = new Dictionary<TMP_Text, List<string>>(); // all coroutine id's
        public static Dictionary<TMP_Text, List<string>> PRids = new Dictionary<TMP_Text, List<string>>(); // all procedure id's
        public static Dictionary<TMP_Text, List<string>> ARids = new Dictionary<TMP_Text, List<string>>(); // all plain coroutine id's

        public static float WAVE_AMPLITUDE = 13.0f;
        private static string GetAndAddCRID(TMP_Text text)
        {
            int r = Random.Range(0, 999999);
            string s = "skte" + r;
            if (!CRids.ContainsKey(text))
            {
                CRids.Add(text, new List<string>());
            }
            CRids[text].Add(s);
            return s;
        }
        private static string GetAndAddPRID(TMP_Text text)
        {
            int r = Random.Range(0, 999999);
            string s = "skte" + r;
            if (!PRids.ContainsKey(text))
            {
                PRids.Add(text, new List<string>());
            }
            PRids[text].Add(s);
            return s;
        }
        private static string GetAndAddARID(TMP_Text text)
        {
            int r = Random.Range(0, 999999);
            string s = "skte" + r;
            if (!ARids.ContainsKey(text))
            {
                ARids.Add(text, new List<string>());
            }
            ARids[text].Add(s);
            return s;
        }
        /// <summary>
        /// Stop all routines (action invokes & procedures) associated to this text.
        /// </summary>
        /// <param name="text"></param>
        public static void StopAllRoutines(TMP_Text text)
        {
            if (CRids.ContainsKey(text))
            {
                foreach(string s in CRids[text])
                {
                    SKUtils.CancelInvoke(s);
                }
                CRids[text].Clear();
            }
            if (PRids.ContainsKey(text))
            {
                foreach (string s in PRids[text])
                {
                    SKUtils.StopProcedure(s);
                }
                PRids[text].Clear();
            }
            if (ARids.ContainsKey(text))
            {
                foreach (string s in ARids[text])
                {
                    SKUtils.StopCoroutine(s);
                }
                ARids[text].Clear();
            }
        }
        #region Typewriter
        /// <summary>
        /// Alpha fade (built-in) typewriter.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="data"></param>
        /// <param name="timePerChar"></param>
        /// <param name="interval"></param>
        public static void TypeWriter_Standard(SKTextAnimation anim, TMP_Text text, SKTextData data, float timePerChar, float interval = 1)
        {
            text.ForceMeshUpdate(true, true);
            Set_Alpha(text, 0);
            float time = data.charData.Length * timePerChar;
            int index = 0;

            string crid = GetAndAddCRID(text);
            anim.animator.curTypewriterCRID = crid;
            SKUtils.InvokeAction(0, () =>
            {
                Anim_Alpha(text, data, index, index, 0, timePerChar, SKTextAnimMode.OneWay, SKCurve.LinearOut, true, anim);
                index++;
            }, data.charData.Length - 1, timePerChar*interval, crid);
        }

        /// <summary>
        /// Alpha fade (built-in) + custom effects typewriter.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="data"></param>
        /// <param name="timePerChar"></param>
        /// <param name="interval"></param>
        /// <param name="effects"></param>
        public static void TypeWriter_Standard_Effects(SKTextAnimation anim, TMP_Text text, SKTextData data, float timePerChar, float interval = 1, params SKTextEffect[] effects)
        {
            text.ForceMeshUpdate(true, true);
            Set_Alpha(text, 0);
            int index = 0;
            string crid = GetAndAddCRID(text);
            anim.animator.curTypewriterCRID = crid;

            SKUtils.InvokeAction(0.05f, () =>
            {
    
                for (int i = 0; i < effects.Length; i++)
                {
                    effects[i].startIndex = Mathf.Clamp(index, 0, data.charData.Length-1);
                    effects[i].endIndex = Mathf.Clamp(index, 0, data.charData.Length - 1);
                    effects[i].time = timePerChar;
                }
                Anim_Alpha(text, data, index, index, 0, timePerChar, SKTextAnimMode.OneWay, SKCurve.LinearOut, true, anim);
                ApplyTextEffect(text, data, effects);
                index++;
            }, data.charData.Length - 1, timePerChar * interval, crid);
        }

        /// <summary>
        /// Custom effects typewriter.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="data"></param>
        /// <param name="timePerChar"></param>
        /// <param name="interval"></param>
        /// <param name="effects"></param>
        public static void TypeWriter_Effects(TMP_Text text, SKTextData data, float timePerChar, float interval = 1, params SKTextEffect[] effects)
        {
            text.ForceMeshUpdate(true, true);
            Set_Alpha(text, 0);
            int index = 0;

            SKUtils.InvokeAction(0, () =>
            {
                for(int i = 0; i < effects.Length; i++)
                {
                    effects[i].startIndex = index;
                    effects[i].endIndex = index;
                    effects[i].time = timePerChar;
                }
                ApplyTextEffect(text, data, effects);
                index++;
            }, data.charData.Length - 1, timePerChar * interval, GetAndAddCRID(text));
        }
        #endregion

        #region General
        /// <summary>
        /// Apply a series of elementary text effects to text component.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="data"></param>
        /// <param name="effects"></param>
        public static void ApplyTextEffect(TMP_Text text, SKTextData data, params SKTextEffect[] effects)
        {
            for(int i=0; i<effects.Length; i++)
            {
                SKTextEffect e = effects[i];
                switch (e.type)
                {
                    case SKTextEffectType.None:
                        break;
                    case SKTextEffectType.Scaling:
                        Anim_Scaling(text, data, e.startIndex, e.endIndex, e.scale, e.time, e.mode, e.curve);
                        break;
                    case SKTextEffectType.Translation:
                        Anim_Translation(text, data, e.startIndex, e.endIndex, e.translation_delta, e.time, e.mode, e.curve);
                        break;
                    case SKTextEffectType.Rotation:
                        Anim_Rotation(text, data, e.startIndex, e.endIndex, e.angle_deg, e.time, e.mode, e.curve, e.rot_center);
                        break;
                    case SKTextEffectType.Color:
                        Anim_Color(text, data, e.startIndex, e.endIndex, e.color, e.time, e.mode, e.curve);
                        break;
                    case SKTextEffectType.Alpha:
                        Anim_Alpha(text, data, e.startIndex, e.endIndex, e.alpha, e.time, e.mode, e.curve);
                        break;
                    case SKTextEffectType.Shake:
                        Anim_Shake(text, data, e.startIndex, e.endIndex, e.shake_Amplitude, e.shake_Frequency, e.time, e.mode, e.curve);
                        break;
                    case SKTextEffectType.Wave:
                        if (e.loop)
                            Anim_Wave_Loop(text, data, e.startIndex, e.endIndex, e.wave_Amplitude, e.timePerChar, e.interval);
                        else
                            Anim_Wave(text, data, e.startIndex, e.endIndex, e.wave_Amplitude, e.timePerChar, e.interval);
                        break;
                    case SKTextEffectType.Twinkle:
                        Anim_Twinkle(text, data, e.startIndex, e.endIndex, e.twinkle_Frequency, e.timePerChar, e.curve);
                        break;
                    case SKTextEffectType.Rainbow:
                        Anim_Rainbow(text, data, e.startIndex, e.endIndex, e.rainbow_Frequency, e.color, e.timePerChar, e.curve);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Anim Wave
        /// <summary>
        /// Wave effect.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="data"></param>
        /// <param name="timePerChar"></param>
        /// <param name="interval"></param>
        public static void Anim_Wave(TMP_Text text, SKTextData data, int start, int end, float amplitude, float timePerChar, float interval = 1)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            float time = (end-start+1) * timePerChar;
            int index = 0;
            SKUtils.InvokeAction(0, () =>
            {
                if (start + index > end)
                    return;
                Anim_Translation_Ping_Pong(text, data, start+index, start + index, Vector3.up*amplitude* WAVE_AMPLITUDE, timePerChar*2, SKCurve.LinearIn);
                index++;
            }, data.charData.Length - 1, timePerChar * interval, GetAndAddCRID(text));
        }
        public static void Anim_Wave_Loop(TMP_Text text, SKTextData data, int start, int end, float amplitude, float timePerChar, float interval = 1)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            float time = (end - start) * timePerChar;
            SKUtils.InvokeActionUnlimited(0, () =>
            {
                int index = 0;
                SKUtils.InvokeAction(0, () =>
                {
                    if (start + index > end)
                        return;
                    Anim_Translation_Ping_Pong(text, data, start + index, start + index, Vector3.up * amplitude * WAVE_AMPLITUDE, timePerChar * 2, SKCurve.LinearIn);
                    index++;
                }, data.charData.Length - 1, timePerChar * interval, GetAndAddCRID(text));
            }, timePerChar*5, GetAndAddCRID(text));
        }
        #endregion
        #region Anim Shake
        public static void Anim_Shake(TMP_Text text, SKTextData data, int start, int end, float amplitude, float frequency, float time, SKTextAnimMode mode, SKCurve curve)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            Anim_Shake_OneWay(text, data, start, end, amplitude, frequency, time, curve);
        }


        /// <summary>
        /// Apply shake animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Shake_OneWay(TMP_Text text, SKTextData data, int start, int end, float amplitude, float frequency, float time, SKCurve curve)
        {
            int len = end - start + 1;
            float singleTime = time / (len);
            SKTextCharData[] chars = new SKTextCharData[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = data.charData[start + i];
            }
            int index = 0;
            SKUtils.InvokeAction(0, () =>
            {
                Anim_Shake_Char(text, chars[index++], amplitude, frequency, time, curve);
            }, len - 1, 0, GetAndAddCRID(text));
        }
        /// <summary>
        /// Apply shake animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Shake_Char(TMP_Text text, SKTextCharData data, float amplitude, float frequency, float time, SKCurve curve)
        {
            int count = m.floor(frequency * time);
            float interval = 1 / frequency;
            SKUtils.InvokeAction(0, () =>
            {
                Vector3 r = SKUtils.RandomVector2();
                Vector3 diff = r * amplitude * 10;
                for (int i = 0; i < 4; i++)
                {
                    int index = i;
                    SKUtils.StartProcedureUnscaled(curve, interval, (f) =>
                    {
                        data.shake_translation = Vector3.Lerp(data.vertices[index], diff + data.oVertices[index], 0.2f) - data.oVertices[index];
                        UpdateCharVertexPos(data);

                        UpdateCharData(data, text);
                        text.UpdateVertexData();
                        
                    },null, GetAndAddPRID(text));
                }

            }, count, interval, GetAndAddCRID(text),() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    int index = i;
                    SKUtils.StartProcedureUnscaled(curve, time, (f) =>
                    {
                        data.shake_translation = Vector3.Lerp(data.translation, Vector3.zero, 0.2f);
                        UpdateCharVertexPos(data);

                        UpdateCharData(data, text);
                        text.UpdateVertexData();
                    },null, GetAndAddPRID(text));
                }
            });

        }
        #endregion
        #region Anim Twinkle
        public static void Anim_Twinkle(TMP_Text text, SKTextData data, int start, int end, float frequency, float time, SKCurve curve)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            Anim_Twinkle_OneWay(text, data, start, end, frequency, time, curve);
        }

        /// <summary>
        /// Apply twinkle animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Twinkle_OneWay(TMP_Text text, SKTextData data, int start, int end, float frequency, float time, SKCurve curve)
        {
            int len = end - start + 1;
            float singleTime = time / (len);
            SKTextCharData[] chars = new SKTextCharData[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = data.charData[start + i];
                Anim_Twinkle_Char(text, chars[i], frequency, time, curve);
            }
        }
        /// <summary>
        /// Apply twinkle animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Twinkle_Char(TMP_Text text, SKTextCharData data, float frequency, float time, SKCurve curve)
        {
            int count = m.floor(frequency * time);
            float interval = 1 / frequency;

            //fade in
            SKUtils.InvokeActionUnlimited(0, () =>
            {
                SKUtils.StartProcedureUnscaled(curve, interval, (f) =>
                {
                    for (int i = 0; i < 4; i++)
                    {
                        data.colors32[i].a = (byte)m.lerp(data.oColors32[i].a, 0, f);
                        UpdateCharData(data, text);
                        text.UpdateVertexData();
                    }
                }, (f) =>
                {
                    //fade in
                    SKUtils.StartProcedureUnscaled(curve, interval, (f) =>
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            data.colors32[i].a = (byte)m.lerp(data.oColors32[i].a, 0, 1 - f);
                            UpdateCharData(data, text);
                            text.UpdateVertexData();
                        }
                    },null, GetAndAddPRID(text));
                }, GetAndAddPRID(text));
            }, interval*2, GetAndAddCRID(text));
           
        }
        #endregion
        #region Anim Rainbow
        public static void Anim_Rainbow(TMP_Text text, SKTextData data, int start, int end, float frequency, Color32 color,  float time, SKCurve curve)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            Anim_Rainbow_OneWay(text, data, start, end, frequency, color, time, curve);
        }

        /// <summary>
        /// Apply rainbow animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Rainbow_OneWay(TMP_Text text, SKTextData data, int start, int end, float frequency, Color32 color, float time, SKCurve curve)
        {
            int len = end - start + 1;
            SKTextCharData[] chars = new SKTextCharData[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = data.charData[start + i];
                SKUtils.StartCoroutine(Anim_Rainbow_Char(text, chars[i], start, end, frequency, color, time, curve), true, GetAndAddARID(text));
            }
        }
        /// <summary>
        /// Apply rainbow animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static IEnumerator Anim_Rainbow_Char(TMP_Text text, SKTextCharData data, int start, int end, float frequency, Color32 color, float time, SKCurve curve)
        {
            float currentPos = start-5;

            while(true)
            {
                currentPos += 0.01f* frequency;
                if (currentPos >= end + 5)
                {
                    currentPos = start-5;
                }
                float sample = m.smoothstep(currentPos, currentPos + 3f, data.index);
                sample *= m.smoothstep(currentPos + 6, currentPos + 3f, data.index);

                for (int i = 0; i < 4; i++)
                {
                    data.colors32[i] = Color32.Lerp(data.oColors32[i], color, sample);
                    UpdateCharData(data, text);
                    text.UpdateVertexData();
                }
                yield return null;
            }

        }
        #endregion
        #region Anim Rotation
        /// <summary>
        /// Rotate the vertices of the character data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void RotateCharVertices(SKTextCharData data, float angle, Vector3 center)
        {
            if (data == null)
                return;
            data.angle_rad = angle * Mathf.Deg2Rad;
            data.rot_center = center;
            UpdateCharVertexPos(data);
        }
        public static void Anim_Rotation(TMP_Text text, SKTextData data, int start, int end, float angle, float time, SKTextAnimMode mode, SKCurve curve, Vector3 center)
        {
            end = m.clamp(end, start, data.charData.Length-1);
            if (mode == SKTextAnimMode.OneWay)
            {
                Anim_Rotation_OneWay(text, data, start, end, angle, time, curve, center);
            }
            else if (mode == SKTextAnimMode.PingPong)
            {
                Anim_Rotation_Ping_Pong(text, data, start, end, angle, time, curve, center);
            }
        }
        private static void Anim_Rotation_Ping_Pong(TMP_Text text, SKTextData data, int start, int end, float angle, float time, SKCurve curve, Vector3 center)
        {
            Anim_Rotation_OneWay(text, data, start, end, angle, time / 2, curve, center);
            SKUtils.InvokeAction(time / 8, () =>
            {
                Anim_Rotation_OneWay(text, data, start, end, angle, time / 2, curve.Reverse(), center);
            });
        }

        /// <summary>
        /// Apply translation animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Rotation_OneWay(TMP_Text text, SKTextData data, int start, int end, float angle, float time, SKCurve curve, Vector3 center)
        {
            int len = end - start + 1;
            float singleTime = time / (len);
            SKTextCharData[] chars = new SKTextCharData[len];
            for (int i = 0; i < len; i++)
            {
                if(i>=0 && i<chars.Length && start + i>=0 && start + i < data.charData.Length)
                {
                    chars[i] = data.charData[start + i];
                }
            }
            int index = 0;
            SKUtils.InvokeAction(0, () =>
            {
                Anim_Rotation_Char(text, chars[index++], angle, singleTime, curve, center);
            }, len - 1, singleTime/3, GetAndAddCRID(text));
        }
        /// <summary>
        /// Apply translation animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Rotation_Char(TMP_Text text, SKTextCharData data, float angle, float time, SKCurve curve, Vector3 center)
        {
            SKUtils.StartProcedureUnscaled(curve, time, (f) =>
            {
                RotateCharVertices(data, angle * f, center);
                UpdateCharData(data, text);
                text.UpdateVertexData();
            },null, GetAndAddPRID(text));
        }
        #endregion
        #region Anim Translation
        /// <summary>
        /// Translate the vertices of the character data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void TranslateCharVertices(SKTextCharData data, Vector3 delta)
        {
            for (int i = 0; i < 4; i++)
            {
                if (data == null) return;
                data.translation = delta;
            }
            UpdateCharVertexPos(data);
        }
        public static void Anim_Translation(TMP_Text text, SKTextData data, int start, int end, Vector3 delta, float time, SKTextAnimMode mode, SKCurve curve)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            if (mode == SKTextAnimMode.OneWay)
            {
                Anim_Translation_OneWay(text, data, start, end, delta, time, curve);
            }
            else if (mode == SKTextAnimMode.PingPong)
            {
                Anim_Translation_Ping_Pong(text, data, start, end, delta, time, curve);
            }
        }
        private static void Anim_Translation_Ping_Pong(TMP_Text text, SKTextData data, int start, int end, Vector3 delta, float time, SKCurve curve)
        {
            Anim_Translation_OneWay(text, data, start, end, delta, time / 2, curve);
            SKUtils.InvokeAction(time / 2 * 1.3f, () =>
            {
                Anim_Translation_OneWay(text, data, start, end, delta, time / 2, curve.Reverse());
            }, 0, 1, GetAndAddCRID(text));
        }

        /// <summary>
        /// Apply translation animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Translation_OneWay(TMP_Text text, SKTextData data, int start, int end, Vector3 delta, float time, SKCurve curve)
        {
            int len = end - start + 1;
            float singleTime = time / (len);
            SKTextCharData[] chars = new SKTextCharData[len];
            for (int i = 0; i < len; i++)
            {
                if (start + i >= data.charData.Length || i>=chars.Length || i< 0 ||start+i<0)
                    continue;
                chars[i] = data.charData[start + i];
            }
            int index = 0;
            SKUtils.InvokeAction(0, () =>
            {
                Anim_Translation_Char(text, chars[index++], delta, singleTime, curve);
            }, len - 1, singleTime, GetAndAddCRID(text));
        }
        /// <summary>
        /// Apply translation animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Translation_Char(TMP_Text text, SKTextCharData data, Vector3 delta, float time, SKCurve curve)
        {
            SKUtils.StartProcedureUnscaled(curve, time, (f) =>
            {
                TranslateCharVertices(data, delta * f);
                UpdateCharData(data, text);
                text.UpdateVertexData();
            },null, GetAndAddPRID(text));
        }
        #endregion
        #region Anim Color
        public static void Anim_Alpha(TMP_Text text, SKTextData data, int start, int end, float alpha, float time, SKTextAnimMode mode, SKCurve curve, bool typewriter=false, SKTextAnimation anim = null)
        {
            end = Mathf.Clamp(end, 0, data.charData.Length-1);
            start = Mathf.Clamp(start, 0, data.charData.Length-1);
            if (start < 0) return;
            Color32 c = data.charData[start].colors32[0];
            Anim_Color(text, data, start, end, new Color32(c.r, c.g, c.b, (byte)(alpha*255)), time, mode, curve, typewriter, anim);
        }
        public static void Anim_Color(TMP_Text text, SKTextData data, int start, int end, Color32 color, float time, SKTextAnimMode mode, SKCurve curve, bool typewriter = false, SKTextAnimation anim = null)
        {
            end = m.clamp(end, start, data.charData.Length - 1);
            if (mode == SKTextAnimMode.OneWay)
            {
                Anim_Color_OneWay(text, data, start, end, color, time, curve, typewriter, anim);
            }
            else if (mode == SKTextAnimMode.PingPong)
            {
                Anim_Color_PingPong(text, data, start, end, color, time, curve);
            }
        }
        public static void Set_Alpha(TMP_Text text, float alpha, bool typewriter=false)
        {
            Color32 c = text.color.ToColor32();
            //Color32 c = text.textInfo.meshInfo[0].colors32[0];
            c.a = (byte)(alpha*255);
            Set_Color(text, c, typewriter);
        }
        public static void Set_Color(TMP_Text text, Color32 color, bool typewriter=false)
        {
            for(int i = 0; i < text.textInfo.meshInfo[0].colors32.Length; i++)
            {
                text.textInfo.meshInfo[0].colors32[i] = color;
            }
            text.UpdateVertexData();
        }
        public static void Set_Original_Color(TMP_Text text, SKTextData data)
        {
            for (int i = 0; i < data.charData.Length; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    text.textInfo.meshInfo[0].colors32[i*4+j] = data.charData[i].colors32[j] = data.charData[i].oColors32[j];
                }
            }
            text.UpdateVertexData();
        }
        /// <summary>
        /// Apply scaling animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Color_OneWay(TMP_Text text, SKTextData data, int start, int end, Color32 color, float time, SKCurve curve, bool typewriter = false, SKTextAnimation anim=null)
        {
            int len = end - start + 1;
            float singleTime = time / (len);
            SKTextCharData[] chars = new SKTextCharData[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = data.charData[start + i];
            }
            int index = 0;
            SKUtils.InvokeAction(0, () =>
            {
                Anim_Color_Char(text, chars[index++], color, singleTime, curve, typewriter, anim, data);
            }, len - 1, singleTime, GetAndAddCRID(text));
        }
        private static void Anim_Color_PingPong(TMP_Text text, SKTextData data, int start, int end, Color32 color, float time, SKCurve curve)
        {
            Anim_Color_OneWay(text, data, start, end, color, time / 2, curve);
            SKUtils.InvokeAction(time / 2 * 1.3f, () =>
            {
                Anim_Color_OneWay(text, data, start, end, color, time / 2, curve.Reverse());
            });
        }
        /// <summary>
        /// Apply color animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Color_Char(TMP_Text text, SKTextCharData data, Color32 color, float time, SKCurve curve, bool typewriter = false, SKTextAnimation anim=null, SKTextData textData=null)
        {
            SKUtils.StartProcedureUnscaled(curve, time, (f) =>
            {
                AlphaCharVertices(data, Color32.Lerp(data.oColors32[0], color, f), typewriter);
                UpdateCharData(data, text);
                text.UpdateVertexData();
            }, (f) =>
            {
                if (typewriter)
                {
                    if (anim != null)
                    {
                        if (data.index == textData.charData.Length-1)
                        {
                            if (anim.animator != null && anim.animator.onTypeWriterFinished != null)
                                anim.animator.onTypeWriterFinished.Invoke();
                        }
                        anim.animator.curTypewriterChar = data.index;
                    }
                }
            }, GetAndAddPRID(text));
            
        }
        /// <summary>
        /// Scale the vertices of the character data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void AlphaCharVertices(SKTextCharData data, Color32 color, bool typewriter = false)
        {

            for (int i = 0; i < 4; i++)
            {
                if (typewriter)
                    data.tColors32[i] = color;
                else
                    data.colors32[i] = color;
            }
        }
        #endregion
        #region Anim Scaling
        /// <summary>
        /// Scale the vertices of the character data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void ScaleCharVertices(SKTextCharData data, float scale)
        {
            if (data == null) return;
            for(int i = 0; i < 4; i++)
            {
                data.scale = scale;
            }
            UpdateCharVertexPos(data);
        }
        public static void Anim_Scaling(TMP_Text text, SKTextData data, int start, int end, float scale, float time, SKTextAnimMode mode, SKCurve curve)
        {
            end = m.clamp(end, 0, data.charData.Length - 1);
            start = m.clamp(start, 0, data.charData.Length - 1);
            if (mode == SKTextAnimMode.OneWay)
            {
                Anim_Scaling_OneWay(text, data, start, end, scale, time, curve);
            }
            else if (mode == SKTextAnimMode.PingPong)
            {
                Anim_Scaling_Ping_Pong(text, data, start, end, scale, time, curve);
            }
        }
        private static void Anim_Scaling_Ping_Pong(TMP_Text text, SKTextData data, int start, int end, float scale, float time, SKCurve curve)
        {
            Anim_Scaling_OneWay(text, data, start, end, scale, time/2, curve);
            SKUtils.InvokeAction((time/5), () =>
            {
                Anim_Scaling_OneWay(text, data, start, end, scale, time / 2, curve.Reverse());
            });
        }

        /// <summary>
        /// Apply scaling animation to a text sequence.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Scaling_OneWay(TMP_Text text, SKTextData data, int start, int end, float scale, float time, SKCurve curve)
        {

            int len = end - start + 1;
            float singleTime = time/(len);
            SKTextCharData[] chars = new SKTextCharData[len];
            for(int i = 0; i < len; i++)
            {
                if (i >= chars.Length || start + i >= data.charData.Length)
                    break;
                chars[i] = data.charData[start+i];
            }
            int index = 0;
            SKUtils.InvokeAction(0, () =>
            {
                    Anim_Scaling_Char(text, chars[index++], scale, singleTime, curve);
            }, len-1, singleTime, GetAndAddCRID(text));
        }
        /// <summary>
        /// Apply scaling animation to a character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        /// <param name="speed"></param>
        private static void Anim_Scaling_Char(TMP_Text text, SKTextCharData data, float scale, float time, SKCurve curve)
        {
            SKUtils.StartProcedureUnscaled(curve, time, (f) =>
            {
                ScaleCharVertices(data, 1 + (scale - 1) * f);
                UpdateCharData(data, text);
                text.UpdateVertexData();
            },null, GetAndAddPRID(text));
        }
        #endregion

        public static void UpdateCharVertexPos(SKTextCharData data)
        {
            Vector3[] curPos = new Vector3[4];

            //scaling
            for (int i = 0; i < 4; i++)
            {
                Vector3 diff = data.oVertices[i] - data.center;
                curPos[i] = data.center + data.scale * diff;
            }
            //rotation
            Vector3 cent = data.rot_center == -Vector3.one ? data.center : GetCenter(data, data.rot_center);
            float dx = cent.x;
            float dy = cent.y;
            for (int i = 0; i < 4; i++)
            {
                float x = curPos[i].x;
                float y = curPos[i].y;
                curPos[i].x = ((x - dx) * m.cos(data.angle_rad)) - ((y - dy) * m.sin(data.angle_rad)) + dx;
                curPos[i].y = dy - ((dy - y) * m.cos(data.angle_rad)) + ((x - dx) * m.sin(data.angle_rad));
            }

            //translation
            for (int i = 0; i < 4; i++)
            {
                curPos[i] += data.translation;
                curPos[i] += data.shake_translation;
                data.vertices[i] = curPos[i];
            }
        }
        private static Vector3 GetCenter(SKTextCharData data, Vector3 cent)
        {
            if (cent.x == 0)
            {
                if(cent.y==0)
                    return data.vertices[0];
                else
                    return data.vertices[3];
            }
            else
            {
                if (cent.y == 0)
                    return data.vertices[1];
                else
                    return data.vertices[2];
            }
        }
        public static void UpdateCharData(SKTextCharData data, TMP_Text text)
        {
            if(data==null) return;
            TMP_TextInfo info =  text.textInfo;
            for(int i = 0; i < 4; i++)
            {
                info.meshInfo[0].vertices[data.index * 4 + i] = data.vertices[i];
                info.meshInfo[0].colors32[data.index * 4 + i] = new Color32(data.colors32[i].r, data.colors32[i].g, data.colors32[i].b, (byte)(data.colors32[i].a * ((float)data.tColors32[i].a / 255.0f)));
            }

        }

        #region Semantics
        public const string CM_SHAKE = "shake"; //time
        public const string CM_BANNER = "banner"; //speed, color.r, color.g, color.b
        public const string CM_FADE = "fade"; //speed
        public const string CM_TWINKLE = "twinkle";//speed
        public const string CM_DANGLE = "dangle";//speed
        public const string CM_EXCLAIM = "excl";//speed, color.r, color.g, color.b
        public const string CM_EXCLAIM_TIMED = "exclt";//speed, time, color.r, color.g, color.b
        public const string CM_WAVE_LOOP = "wave";//speed
        public const string CM_SCALE_UP = "scaleup";//speed
        public const string CM_SCALE_DOWN = "scaledn";//speed
        public const string CM_SCALE_ROTATE = "rot";//speed, angle
        public const string CM_SCALE_COLOR = "col";//speed, color.r, color.g, color.b

        #endregion
    }
    public enum SKTextAnimMode
    {
        OneWay,
        PingPong
    }
}
