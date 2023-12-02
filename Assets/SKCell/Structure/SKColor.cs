using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SKCell
{
    public struct SKColor : IEquatable<SKColor>, IFormattable
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public float Luminance
        {
            get
            {
                return r *0.2126f + g * 0.7152f + b * 0.0722f;
            }
        }

        public SKColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
        }
        public SKColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        #region Palettes
        public static SKColor White
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(1f, 1f, 1f, 1f);
            }
        }
        public static SKColor Black
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0f, 0f, 0f, 1f);
            }
        }
        public static SKColor DarkGrey
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.14f, 0.14f, 0.14f, 1f);
            }
        }
        public static SKColor Grey
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.5f, 0.5f, 0.5f, 1f);
            }
        }
        public static SKColor Clear
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0f, 0f, 0f, 0f);
            }
        }
        public static SKColorPalette_Pure pure
        {
            get
            {
                return SKColorPalette_Pure.instance;
            }
        }
        public static SKColorPalette_Standard standard
        {
            get
            {
                return SKColorPalette_Standard.instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Lerp between two SKColors. (Unclamped)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SKColor Lerp(SKColor a, SKColor b, float t)
        {
            return new SKColor(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
        }

        public static void RGBToHSV(SKColor rgbColor, out float H, out float S, out float V)
        {
            if (rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r)
            {
                RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
            }
            else if (rgbColor.g > rgbColor.r)
            {
                RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
            }
            else
            {
                RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
            }
        }

        private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
        {
            V = dominantcolor;
            if (V != 0f)
            {
                float num = 0f;
                num = ((!(colorone > colortwo)) ? colorone : colortwo);
                float num2 = V - num;
                if (num2 != 0f)
                {
                    S = num2 / V;
                    H = offset + (colorone - colortwo) / num2;
                }
                else
                {
                    S = 0f;
                    H = offset + (colorone - colortwo);
                }

                H /= 6f;
                if (H < 0f)
                {
                    H += 1f;
                }
            }
            else
            {
                S = 0f;
                H = 0f;
            }
        }

        public static SKColor HSVToRGB(float H, float S, float V)
        {
            return HSVToRGB(H, S, V, hdr: true);
        }

        public static SKColor HSVToRGB(float H, float S, float V, bool hdr)
        {
            SKColor result = new SKColor(1,1,1);
            if (S == 0f)
            {
                result.r = V;
                result.g = V;
                result.b = V;
            }
            else if (V == 0f)
            {
                result.r = 0f;
                result.g = 0f;
                result.b = 0f;
            }
            else
            {
                result.r = 0f;
                result.g = 0f;
                result.b = 0f;
                float num = H * 6f;
                int num2 = (int)Mathf.Floor(num);
                float num3 = num - (float)num2;
                float num4 = V * (1f - S);
                float num5 = V * (1f - S * num3);
                float num6 = V * (1f - S * (1f - num3));
                switch (num2)
                {
                    case 0:
                        result.r = V;
                        result.g = num6;
                        result.b = num4;
                        break;
                    case 1:
                        result.r = num5;
                        result.g = V;
                        result.b = num4;
                        break;
                    case 2:
                        result.r = num4;
                        result.g = V;
                        result.b = num6;
                        break;
                    case 3:
                        result.r = num4;
                        result.g = num5;
                        result.b = V;
                        break;
                    case 4:
                        result.r = num6;
                        result.g = num4;
                        result.b = V;
                        break;
                    case 5:
                        result.r = V;
                        result.g = num4;
                        result.b = num5;
                        break;
                    case 6:
                        result.r = V;
                        result.g = num6;
                        result.b = num4;
                        break;
                    case -1:
                        result.r = V;
                        result.g = num4;
                        result.b = num5;
                        break;
                }

                if (!hdr)
                {
                    result.r = Mathf.Clamp(result.r, 0f, 1f);
                    result.g = Mathf.Clamp(result.g, 0f, 1f);
                    result.b = Mathf.Clamp(result.b, 0f, 1f);
                }
            }

            return result;
        }

        /// <summary>
        /// Manhattan distance of the *RGB* components of two colors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SimpleDistance(SKColor a, SKColor b)
        {
            return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b-b.b);
        }
        /// <summary>
        /// Manhattan distance of the *RGBA* components of two colors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SimpleDistanceWithAlpha(SKColor a, SKColor b)
        {
            return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F3";
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return String.Format("RGBA({0}, {1}, {2}, {3})", r.ToString(format, formatProvider), g.ToString(format, formatProvider), b.ToString(format, formatProvider), a.ToString(format, formatProvider));
        }

        public override bool Equals(object other)
        {
            if (!(other is SKColor))
            {
                return false;
            }

            return Equals((SKColor)other);
        }

        public bool Equals(SKColor other)
        {
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }

        public override int GetHashCode()
        {
            return ((Vector4)this).GetHashCode();
        }

        #endregion

        #region Operators & Conversions
        public static SKColor operator +(SKColor a, SKColor b)
        {
            return new SKColor(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        }

        public static SKColor operator -(SKColor a, SKColor b)
        {
            return new SKColor(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        }

        public static SKColor operator *(SKColor a, SKColor b)
        {
            return new SKColor(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        public static SKColor operator *(SKColor a, float b)
        {
            return new SKColor(a.r * b, a.g * b, a.b * b, a.a * b);
        }

        public static SKColor operator *(float b, SKColor a)
        {
            return new SKColor(a.r * b, a.g * b, a.b * b, a.a * b);
        }

        public static SKColor operator /(SKColor a, float b)
        {
            return new SKColor(a.r / b, a.g / b, a.b / b, a.a / b);
        }

        public static bool operator ==(SKColor lhs, SKColor rhs)
        {
            return (Vector4)lhs == (Vector4)rhs;
        }
        public static bool operator !=(SKColor lhs, SKColor rhs)
        {
            return !(lhs == rhs);
        }
        public static implicit operator Vector4(SKColor v)
        {
            return new Vector4(v.r, v.g, v.b, v.a);
        }
        public static implicit operator Vector3(SKColor v)
        {
            return new Vector4(v.r, v.g, v.b, 1);
        }
        public static implicit operator SKColor(Vector4 v)
        {
            return new SKColor(v.x, v.y, v.z, v.w);
        }
        public static implicit operator SKColor(Vector3 v)
        {
            return new SKColor(v.x, v.y, v.z, 1);
        }
        public static implicit operator SKColor(Color v)
        {
            return new SKColor(v.r, v.g, v.b, v.a);
        }
        public static implicit operator Color(SKColor v)
        {
            return new SKColor(v.r, v.g, v.b, v.a);
        }
        #endregion
    }
    public static class SKColorPalette
    {
        public static SKColorPalette_Standard standard;
    }
    public class SKColorPalette_Standard : SKSingleton<SKColorPalette_Standard>
    {
        public SKColor Red
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(1f, 0.2f, 0.2f, 1f);
            }
        }
        public SKColor Green
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.4f, 1f, 0.4f, 1f);
            }
        }
        public SKColor Blue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.25f, 0.25f, 1f, 1f);
            }
        }
        public SKColor Yellow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(1f, 1f, 0.35f, 1f);
            }
        }
        public SKColor Cyan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.35f, 1f, 1f, 1f);
            }
        }
        public SKColor Pink
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.96f, 0.53f, 0.9f, 1f);
            }
        }
        public SKColor Orange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.96f, 0.7f, 0.14f, 1f);
            }
        }
        public SKColor Purple
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.64f, 0.37f, 1f, 1f);
            }
        }
        public SKColor Brown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.75f, 0.46f, 0.27f, 1f);
            }
        }
        public SKColor White
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.97f, 0.97f, 0.97f, 1f);
            }
        }
        public SKColor Black
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0.14f, 0.14f, 0.14f, 1f);
            }
        }
    }

    public class SKColorPalette_Pure :SKSingleton <SKColorPalette_Pure>
    {
        public SKColor Red
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(1f, 0f, 0f, 1f);
            }
        }
        public SKColor Green
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0f, 1f, 0f, 1f);
            }
        }
        public SKColor Blue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0f, 0f, 1f, 1f);
            }
        }
        public SKColor Yellow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(1f, 1f, 0f, 1f);
            }
        }
        public SKColor Cyan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SKColor(0f, 1f, 1f, 1f);
            }
        }
    }

}