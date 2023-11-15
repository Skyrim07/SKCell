using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKLightCastEffect")]
    public class SKLightCastEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;
        [Range(-1f, 1f)]
        public float _Distance = 5f;
        public float _NearSmoothDistance = 1f;
        public float _Pow = 3f;
        public float _Intensity = 1f;
        public Vector4 direction = new Vector2(1f,1f);

        public Color colorTint = Color.yellow;
        public Color colorHighlight = Color.white;

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
  
            alphaShader = Shader.Find("SKCell/LightCast");

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if(mr)
                mr.material = _Material;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
                sr.material = _Material;
            _material.SetFloat("_HighlightThreshold", 0.1f);
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetColor("_Color", colorTint);
            _material.SetFloat("_Distance", _Distance);
            _material.SetFloat("_Intensity", _Intensity);
            _material.SetFloat("_Pow", _Pow);
            _material.SetFloat("_NearSmoothDistance", _NearSmoothDistance);
            _material.SetVector("_LightPos", direction);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}