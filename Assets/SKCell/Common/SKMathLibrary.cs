using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Concise static math library
    /// </summary>
    public static class m
    {
        public const float PI = Mathf.PI;
        public const float TWO_PI = 2*Mathf.PI;
        public const float HALF_PI = 0.5f*Mathf.PI;

        public const float INF = float.PositiveInfinity;
        public const float NINF = float.NegativeInfinity;

        public const float E = (float)Math.E;
        public const float deg2rad = Mathf.Deg2Rad;
        public const float rad2deg = Mathf.Rad2Deg;

        #region Common
        /// <summary>
        /// Absolute value of float
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float abs(float f)
        {
            return Mathf.Abs(f);
        }
        /// <summary>
        /// Absolute value of int
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int abs(int i)
        {
            return Mathf.Abs(i);
        }
        /// <summary>
        /// Absolute value of each component of Vector2
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Vector2 abs(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }
        /// <summary>
        /// Absolute value of each component of Vector3
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Vector3 abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }
        /// <summary>
        /// X to the power of P
        /// </summary>
        /// <param name="x"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float pow(float x, float p)
        {
            return Mathf.Pow(x, p);
        }
        /// <summary>
        /// X to the power of P
        /// </summary>
        /// <param name="x"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float pow(int x, float p)
        {
            return Mathf.Pow(x, p);
        }
        /// <summary>
        /// Square root of X
        /// </summary>
        /// <param name="x"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float sqrt(float x)
        {
            return Mathf.Sqrt(x);
        }
        /// <summary>
        /// x^2
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float x2(float x)
        {
            return x * x;
        }
        /// <summary>
        /// x^3
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float x3(float x)
        {
            return x * x * x;
        }
        /// <summary>
        /// x^4
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float x4(float x)
        {
            return x * x * x * x;
        }
        /// <summary>
        /// x^5
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float x5(float x)
        {
            return x * x * x * x * x;
        }
        /// <summary>
        /// 1/x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float rcp(float x)
        {
            return 1.0f / x;
        }
        /// <summary>
        /// 1/x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float rcp(int x)
        {
            return 1.0f / x;
        }
        /// <summary>
        /// Fraction part of float
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float frac(float f)
        {
            return f - Mathf.FloorToInt(f);
        }
        /// <summary>
        /// Floor of float
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int floor(float f)
        {
            return Mathf.FloorToInt(f);
        }
        /// <summary>
        /// Ceiling of float
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int ceil(float f)
        {
            return Mathf.CeilToInt(f);
        }
        /// <summary>
        /// Round float to digits
        /// </summary>
        /// <param name="f"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float round(float f, int digits)
        {
            return (float)Math.Round((double)f, digits);
        }
        /// <summary>
        /// Clamp between two values
        /// </summary>
        /// <param name="f"></param>
        /// <param name="l"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static float clamp(float f, float l, float h)
        {
            return Mathf.Clamp(f, l, h);
        }
        /// <summary>
        /// Clamp between two values
        /// </summary>
        /// <param name="f"></param>
        /// <param name="l"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static int clamp(int f, int l, int h)
        {
            return Mathf.Clamp(f, l, h);
        }
        /// <summary>
        /// Clamp between 0 and 1
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float clamp01(float f)
        {
            return Mathf.Clamp01(f);
        }
        /// <summary>
        /// Clamp between 0 and 1
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float saturate(float f)
        {
            return Mathf.Clamp01(f);
        }
        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static float lerp(float f1, float f2, float step)
        {
            return Mathf.Lerp(f1, f2, step);
        }
        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Vector3 lerp(Vector3 f1, Vector3 f2, float step)
        {
            return Vector3.Lerp(f1, f2, step);
        }
        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Vector2 lerp(Vector2 f1, Vector2 f2, float step)
        {
            return Vector2.Lerp(f1, f2, step);
        }
        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Color lerp(Color f1, Color f2, float step)
        {
            return Color.Lerp(f1, f2, step);
        }
        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Quaternion lerp(Quaternion f1, Quaternion f2, float step)
        {
            return Quaternion.Lerp(f1, f2, step);
        }
        /// <summary>
        /// Angle (degrees) to 2D normalized vector
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 angle2vector(float angle)
        {
            float a = angle * deg2rad;
            return new Vector2(cos(a), sin(a));
        }
        /// <summary>
        /// 2D vector to angle (degrees)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float vector2angle(Vector2 vector)
        {
            return atan2(vector.x, vector.y);
        }
        /// <summary>
        /// Luminance of color (greyscale)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float luminance(Color c)
        {
            return c.r * 0.2126f + c.g * 0.7152f + c.b * 0.0722f;
        }
        /// <summary>
        /// 1 if x>=a, 0 if x<a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float step(float a, float x)
        {
            return x >= a ? 1 : 0;
        }
        /// <summary>
        /// 1 if x>=b, 0 if x<=a, smooth lerp if in between
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float smoothstep(float a, float b, float x)
        {
            float t = clamp((x - a) / (b - a), 0.0f, 1.0f);
            return t * t * (3.0f - 2.0f * t);
        }
        #endregion
        #region Stats
        /// <summary>
        /// Minimum of floats
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float min(params float[] f)
        {
            float res = float.MaxValue;
            for (int i = 0; i < f.Length; i++)
            {
                res = min(f[i], res);
            }
            return res;
        }
        /// <summary>
        /// Minimum of ints
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int min(params int[] f)
        {
            int res = int.MaxValue;
            for (int i = 0; i < f.Length; i++)
            {
                res = min(f[i], res);
            }
            return res;
        }        
        /// <summary>
        /// Maximum of floats
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float max(params float[] f)
        {
            float res = float.MinValue;
            for (int i = 0; i < f.Length; i++)
            {
                res = max(f[i], res);
            }
            return res;
        }
        /// <summary>
        /// Minimum of ints
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int max(params int[] f)
        {
            int res = int.MinValue;
            for (int i = 0; i < f.Length; i++)
            {
                res = max(f[i], res);
            }
            return res;
        }
        /// <summary>
        /// Mean of floats
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float mean(params float[] f)
        {
            float res = 0;
            for (int i = 0;i < f.Length;i++)
            {
                res+=f[i];
            }
            return res/f.Length;
        }
        /// <summary>
        /// Mean of ints
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float mean(params int[] f)
        {
            float res = 0;
            for (int i = 0; i < f.Length; i++)
            {
                res += f[i];
            }
            return res / f.Length;
        }
        /// <summary>
        /// Median of floats
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float median(params float[] f)
        {
            Array.Sort(f);
            return f.Length % 2 == 0 ? mean(f[f.Length / 2 - 1], f[f.Length / 2]) : f[f.Length / 2];
        }
        /// <summary>
        /// Median of ints
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float median(params int[] f)
        {
            Array.Sort(f);
            return f.Length % 2 == 0 ? mean(f[f.Length / 2 - 1], f[f.Length / 2]) : f[f.Length / 2];
        }
        #endregion
        #region Trigonometry
        /// <summary>
        /// Sine of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float sin(float x)
        {
            return Mathf.Sin(x);
        }
        /// <summary>
        /// Cosine of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float cos(float x)
        {
            return Mathf.Cos(x);
        }
        /// <summary>
        /// Tangent of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float tan(float x)
        {
            return Mathf.Tan(x);
        }
        /// <summary>
        /// Sine of radian x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float sinrad(float x)
        {
            return Mathf.Sin(x * deg2rad);
        }
        /// <summary>
        /// Cosine of radian x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float cosrad(float x)
        {
            return Mathf.Cos(x * deg2rad);
        }
        /// <summary>
        /// Tangent of radian x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float tanrad(float x)
        {
            return Mathf.Tan(x * deg2rad);
        }
        /// <summary>
        /// Arcsine of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float asin(float x)
        {
            return Mathf.Asin(x);
        }
        /// <summary>
        /// Arccosine of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float acos(float x)
        {
            return Mathf.Acos(x);
        }
        /// <summary>
        /// Arctangent of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float atan(float x)
        {
            return Mathf.Atan(x);
        }
        /// <summary>
        /// Angle in radians whose tan is y/x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float atan2(float x, float y)
        {
            return Mathf.Atan2(x, y);
        }
        /// <summary>
        /// Sine hyperbole of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float sinh(float x)
        {
            return (float)Math.Sinh((double)x);
        }
        /// <summary>
        /// Cosine hyperbole of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float cosh(float x)
        {
            return (float)Math.Cosh((double)x);
        }
        /// <summary>
        /// Tangent hyperbole of angle x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float tanh(float x)
        {
            return (float)Math.Tanh((double)x);
        }
        /// <summary>
        /// Sine of radian x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        #endregion
    }
}
