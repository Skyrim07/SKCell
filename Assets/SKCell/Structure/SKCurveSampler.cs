using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Provide various animation curve sampling.
    /// </summary>
    public static class SKCurveSampler
    {
        public static readonly Dictionary<CurveType, Func<float, float>> curveFuncs = new Dictionary<CurveType, Func<float, float>>()
    {
        { CurveType.Linear, (x)=>{ return x; } },
        { CurveType.Quadratic, (x)=>{ return 1 - (1-x)*(1-x); } },
        { CurveType.Cubic, (x)=>{ return 1 - (1-x)*(1-x)*(1-x); } },
        { CurveType.Quartic, (x)=>{ return 1 - (1-x)*(1-x)*(1-x)*(1-x); } },
        { CurveType.Quintic, (x)=>{ return 1 - (1-x)*(1-x)*(1-x)*(1-x)*(1-x); } },
        { CurveType.QuadraticDouble, (x)=>
        {
            return x<0.5f? 2*x*x:2*x*(2-x)-1; }
        },
        { CurveType.CubicDouble, (x)=>
        {
            return x<0.5f? 4*x*x*x:-4*(1-x)*(1-x)*(1-x)+1; }
        },
        { CurveType.QuarticDouble, (x)=>
        {
            return x<0.5f? 8*x*x*x*x:-8*(1-x)*(1-x)*(1-x)*(1-x)+1; }
        },
        { CurveType.QuinticDouble, (x)=>
        {
            return x<0.5f? 16*x*x*x*x*x:-16*(1-x)*(1-x)*(1-x)*(1-x)*(1-x)+1; }
        },
        { CurveType.Sine, (x)=>
        {
            return 0.5f * (1.0f - Mathf.Cos(x * Mathf.PI * 2)); }
        },
        { CurveType.SineDouble, (x)=>
        {
            return 0.5f * (1.0f - Mathf.Cos(x * Mathf.PI)); }
        },
        { CurveType.Expo, (x)=>
        {
            return x==0?0:Mathf.Pow(2.0f, 10.0f * (x - 1.0f)); }
        },
        { CurveType.ExpoDouble, (x)=>
        {
            return x==0?0:(x==1?1:(x<0.5f?0.5f * Mathf.Pow(2.0f,20.0f * x - 10.0f) :0.5f * (2.0f - Mathf.Pow( 2.0f,-20.0f * x + 10.0f)) )); }
        },
        { CurveType.Elastic, (x)=>
        {
            return x==0?0:(x==1?1:(-Mathf.Pow(2.0f,10.0f * x - 10.0f) * Mathf.Sin((3.33f * x - 3.58f) * Mathf.PI * 2))); }
        },
        { CurveType.ElasticDouble, (x)=>
        {
            return x==0?0:(x==1?1:(x<0.5f?-0.5f * Mathf.Pow(2.0f,20.0f * x - 10.0f) * Mathf.Sin((4.45f * x - 2.475f) * Mathf.PI*2):Mathf.Pow(2.0f, -20.0f * x + 10.0f) * Mathf.Sin((4.45f * x - 2.475f) * Mathf.PI*2) * 0.5f + 1.0f)); }
        },
        { CurveType.Circ, (x)=>
        {
            return 1.0f - Mathf.Sqrt(1.0f - x * x); }
        },
        { CurveType.CircDouble, (x)=>
        {
            return x<0.5?0.5f * (1.0f - Mathf.Sqrt(1.0f - 4.0f * x * x)): 0.5f * (Mathf.Sqrt(1.0f - (x*2-2) * (x*2-2)) + 1 ); }
        },
         { CurveType.Back, (x)=>
        {
            return x * x * (2.70158f * x - 1.70158f); }
        },
        { CurveType.BackDouble, (x)=>
        {
            return x<0.5?x * x * (14.379636f * x - 5.189818f): ((x-1) * (x-1)  * (14.379636f * (x-1)  + 5.189818f) + 1.0f); }
        },
        { CurveType.Bounce, (x)=>
        {
            if (x < 0.363636f)
            {
                return  7.5625f * x * x;
            }
            else if (x < 0.72727f)
            {
                x -= 0.545454f;
                return (7.5625f * x * x + 0.75f);
            }
            else if (x < 0.909091f)
            {
                x -= 0.818182f;
                return (7.5625f * x * x + 0.9375f);
            }
            else
            {
                x -= 0.954545f;
                return (7.5625f * x * x + 0.984375f);
            }
        }
        },
        { CurveType.BounceDouble, (x)=>
        {
            if (x < 0.5f)
            {
                if (x > 0.318182f)
                {
                    x = 1.0f - x * 2.0f;
                    return  (0.5f - 3.78125f * x * x);
                }
                else if (x > 0.136365f)
                {
                    x = 0.454546f - x * 2.0f;
                    return  (0.125f - 3.78125f * x * x);
                }
                else if (x > 0.045455f)
                {
                    x = 0.181818f - x * 2.0f;
                    return  (0.03125f - 3.78125f * x * x);
                }
                else
                {
                    x = 0.045455f - x * 2.0f;
                    return  (0.007813f - 3.78125f * x * x);
                }
            }

            if (x < 0.681818f)
            {
                x = x * 2.0f - 1.0f;
                return  (3.78125f * x * x + 0.5f);
            }
            else if (x < 0.863635f)
            {
                x = x * 2.0f - 1.545454f;
                return  (3.78125f * x * x + 0.875f);
            }
            else if (x < 0.954546f)
            {
                x = x * 2.0f - 1.818182f;
                return  (3.78125f * x * x + 0.96875f);
            }
            else
            {
                x = x * 2.0f - 1.954545f;
                return  (3.78125f * x * x + 0.992188f);
            }
        }
        },
    };


        /// <summary>
        /// Returns a float y (0,1) on the curve corresponding to the given x (0,1).
        /// </summary>
        /// <returns></returns>
        public static float SampleCurve(SKCurve curve, float x)
        {
            x = Mathf.Clamp01(x);
            CurveType type = curve.curveType;
            Func<float, float> func = curveFuncs[type];

            switch (curve.curveDir)
            {
                case CurveDir.In:
                    return func(x);

                case CurveDir.Out:
                    return 1 - func(x);

                default:
                    return -1;
            }
        }
    }


    public enum CurveType
    {
        Linear,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
        QuadraticDouble,
        CubicDouble,
        QuarticDouble,
        QuinticDouble,
        Sine,
        SineDouble,
        Expo,
        ExpoDouble,
        Elastic,
        ElasticDouble,
        Circ,
        CircDouble,
        Back,
        BackDouble,
        Bounce,
        BounceDouble
    }

    public enum CurveDir
    {
        In,
        Out,
    }
}