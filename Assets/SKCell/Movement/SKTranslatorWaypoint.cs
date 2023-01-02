using System;
using UnityEngine;

namespace SKCell
{
    [Serializable]
    public class SKTranslatorWaypoint 
    {
        public Vector3 localPosition;
        public Quaternion rotation;
        public SKTranslatorWaypointType type;
        public SKBezier bezier; //local pos
        public SKCurve curve;

        public float stayTime = 0;
    }

    public enum SKTranslatorWaypointType
    {
        Line,
        Bezier
    }
}
