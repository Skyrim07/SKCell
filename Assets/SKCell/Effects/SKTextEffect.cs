using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public class SKTextEffect
    {
        public float time = 1.0f;
        public float timePerChar = 1.0f;
        public float interval = 1.0f;
        public bool loop = false;
        public bool isTypewriter = false;
        public SKTextAnimMode mode = SKTextAnimMode.OneWay;
        public SKTextEffectType type = SKTextEffectType.None;
        public int startIndex = 0, endIndex = int.MaxValue;
        public SKCurve curve = SKCurve.LinearIn;

        public float wave_Amplitude = 0.5f;
        public float twinkle_Frequency = 0.5f;
        public float rainbow_Frequency = 0.5f;
        public float shake_Amplitude = 0.5f;
        public float shake_Frequency = 50f;
        public float scale = 1.5f;
        public Vector3 translation_delta = Vector3.zero;
        public Vector3 rot_center = -Vector3.one;
        public float angle_deg = 45;
        public Color32 color;
        public float alpha = 0.0f;
    }
    public enum SKTextEffectType
    {
        None,
        Scaling,
        Translation,
        Rotation,
        Color,
        Alpha,
        Shake,
        Wave,
        Twinkle,
        Rainbow
    }
}
