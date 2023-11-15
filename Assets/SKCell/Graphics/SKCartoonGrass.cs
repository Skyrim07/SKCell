using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("SKCell/Effects/SKCartoonGrass")]
    public class SKCartoonGrass : PostEffectsBase
    {
        public bool updateOnPlay = true;

        [SKFolder("Colors")]
        public Color _TopColor = new Color(.51f, 1f, .47f);
        public Color _BottomColor = new Color(.04f, .31f, .13f);
        public Color _TintColor = new Color(.77f, 1.0f, .467f);
        public Color _TintColor2 = new Color(.21f, .42f, .51f);
        [Range(0f,1f)] public float _Tint = 0.53f;
        [Range(0f,1f)] public float _Tint2 = 0.77f;

        [SKFolder("Geometry")]
        [Range(0f,1f)] public float _Bend = 0.2f;
        public float _BladeWidth = 0.05f;
        public float _BladeWidthRandom = 0.02f;
        public float _BladeHeight = 0.4f;
        public float _BladeHeightRandom = 0.22f;
        public float _BladeCurve = 2f;
        public float _BladeForward = 0.06f;
        public float _BladeForwardRandom = 0.09f;

        [SKFolder("Wind")]
        public Vector4 _WindFrequency = new Vector4(.08f, .08f, 0,0);
        public float _WindStrength = 0.5f;

        private Shader alphaShader;
        private Material _material;
        public Material _Material
        {
            get
            {
                _material = CheckShaderAndCreateMaterial(alphaShader, _material);
                return _material;
            }
        }
        private void OnEnable()
        {
            alphaShader = Shader.Find("SKCell/Grass_2");
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr)
                mr.material = _Material;
            Texture2D tintTex1 = Resources.Load<Texture2D>("SKCell/Textures/noise");
            Texture2D tintTex2 = Resources.Load<Texture2D>("SKCell/Textures/PerlinNoise");
            Texture2D windMap = Resources.Load<Texture2D>("SKCell/Textures/noiseRG");

            _material.SetTexture("_TintTex", tintTex1);
            _material.SetTextureScale("_TintTex", Vector2.one*.5f);
            _material.SetTexture("_TintTex2", tintTex2);
            _material.SetTexture("_WindDistortionMap", windMap);
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetColor("_TopColor", _TopColor);
            _material.SetColor("_BottomColor", _BottomColor);
            _material.SetColor("_TintColor", _TintColor);
            _material.SetColor("_TintColor2", _TintColor2);

            _material.SetFloat("_Tint", _Tint);
            _material.SetFloat("_Tint2", _Tint2);
            _material.SetFloat("_Bend", _Bend);
            _material.SetFloat("_BladeWidth", _BladeWidth);
            _material.SetFloat("_BladeWidthRandom", _BladeWidthRandom);
            _material.SetFloat("_BladeHeight", _BladeHeight);
            _material.SetFloat("_BladeHeightRandom", _BladeHeightRandom);
            _material.SetFloat("_BladeCurve", _BladeCurve);
            _material.SetFloat("_BladeForward", _BladeForward);
            _material.SetFloat("_BladeForwardRandom", _BladeForwardRandom);

            _material.SetVector("_WindFrequency", _WindFrequency);
            _material.SetFloat("_WindStrength", _WindStrength);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}