using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [Serializable] 
    public class SKCurve
    {

        public CurveType curveType;
        public CurveDir curveDir;

        public static SKCurve LinearIn { get { return new SKCurve(CurveType.Linear, CurveDir.In); } }
        public static SKCurve LinearOut { get { return new SKCurve(CurveType.Linear, CurveDir.Out); } }
        public static SKCurve QuadraticIn { get { return new SKCurve(CurveType.Quadratic, CurveDir.In); } }
        public static SKCurve QuadraticOut { get { return new SKCurve(CurveType.Quadratic, CurveDir.Out); } }
        public static SKCurve CubicIn { get { return new SKCurve(CurveType.Cubic, CurveDir.In); } }
        public static SKCurve CubicOut { get { return new SKCurve(CurveType.Cubic, CurveDir.Out); } }
        public static SKCurve QuarticIn { get { return new SKCurve(CurveType.Quartic, CurveDir.In); } }
        public static SKCurve QuarticOut { get { return new SKCurve(CurveType.Quartic, CurveDir.Out); } }
        public static SKCurve QuinticIn { get { return new SKCurve(CurveType.Quintic, CurveDir.In); } }
        public static SKCurve QuinticOut { get { return new SKCurve(CurveType.Quintic, CurveDir.Out); } }
        public static SKCurve QuadraticDoubleIn { get { return new SKCurve(CurveType.QuadraticDouble, CurveDir.In); } }
        public static SKCurve QuadraticDoubleOut { get { return new SKCurve(CurveType.QuadraticDouble, CurveDir.Out); } }
        public static SKCurve CubicDoubleIn { get { return new SKCurve(CurveType.CubicDouble, CurveDir.In); } }
        public static SKCurve CubicDoubleOut { get { return new SKCurve(CurveType.CubicDouble, CurveDir.Out); } }
        public static SKCurve QuarticDoubleIn { get { return new SKCurve(CurveType.QuarticDouble, CurveDir.In); } }
        public static SKCurve QuarticDoubleOut { get { return new SKCurve(CurveType.QuarticDouble, CurveDir.Out); } }
        public static SKCurve QuinticDoubleIn { get { return new SKCurve(CurveType.QuinticDouble, CurveDir.In); } }
        public static SKCurve QuinticDoubleOut { get { return new SKCurve(CurveType.QuinticDouble, CurveDir.Out); } }
        public static SKCurve SineIn { get { return new SKCurve(CurveType.Sine, CurveDir.In); } }
        public static SKCurve SineOut { get { return new SKCurve(CurveType.Sine, CurveDir.Out); } }
        public static SKCurve SineDoubleIn { get { return new SKCurve(CurveType.SineDouble, CurveDir.In); } }
        public static SKCurve SineDoubleOut { get { return new SKCurve(CurveType.SineDouble, CurveDir.Out); } }
        public static SKCurve ExpoIn { get { return new SKCurve(CurveType.Expo, CurveDir.In); } }
        public static SKCurve ExpoOut { get { return new SKCurve(CurveType.Expo, CurveDir.Out); } }
        public static SKCurve ExpoDoubleIn { get { return new SKCurve(CurveType.ExpoDouble, CurveDir.In); } }
        public static SKCurve ExpoDoubleOut { get { return new SKCurve(CurveType.ExpoDouble, CurveDir.Out); } }
        public static SKCurve ElasticIn { get { return new SKCurve(CurveType.Elastic, CurveDir.In); } }
        public static SKCurve ElasticOut { get { return new SKCurve(CurveType.Elastic, CurveDir.Out); } }
        public static SKCurve ElasticDoubleIn { get { return new SKCurve(CurveType.ElasticDouble, CurveDir.In); } }
        public static SKCurve ElasticDoubleOut { get { return new SKCurve(CurveType.ElasticDouble, CurveDir.Out); } }
        public static SKCurve CircIn { get { return new SKCurve(CurveType.Circ, CurveDir.In); } }
        public static SKCurve CircOut { get { return new SKCurve(CurveType.Circ, CurveDir.Out); } }
        public static SKCurve CircDoubleIn { get { return new SKCurve(CurveType.CircDouble, CurveDir.In); } }
        public static SKCurve CircDoubleOut { get { return new SKCurve(CurveType.CircDouble, CurveDir.Out); } }
        public static SKCurve BackIn { get { return new SKCurve(CurveType.Back, CurveDir.In); } }
        public static SKCurve BackOut { get { return new SKCurve(CurveType.Back, CurveDir.Out); } }
        public static SKCurve BackDoubleIn { get { return new SKCurve(CurveType.BackDouble, CurveDir.In); } }
        public static SKCurve BackDoubleOut { get { return new SKCurve(CurveType.BackDouble, CurveDir.Out); } }
        public static SKCurve BounceIn { get { return new SKCurve(CurveType.Bounce, CurveDir.In); } }
        public static SKCurve BounceOut { get { return new SKCurve(CurveType.Bounce, CurveDir.Out); } }
        public static SKCurve BounceDoubleIn { get { return new SKCurve(CurveType.BounceDouble, CurveDir.In); } }
        public static SKCurve BounceDoubleOut { get { return new SKCurve(CurveType.BounceDouble, CurveDir.Out); } }
        public Func<float, float> func { get; private set; }
        public SKCurve(CurveType curveType, CurveDir curveDir)
        {
            this.curveType = curveType;
            this.curveDir = curveDir;
        }
        public SKCurve(Func<float, float> func)
        {
            this.func = func;
        }

        public SKCurve Reverse()
        {
            return  new SKCurve(curveType, curveDir == CurveDir.In ? CurveDir.Out : CurveDir.In);
        }
    }
}
