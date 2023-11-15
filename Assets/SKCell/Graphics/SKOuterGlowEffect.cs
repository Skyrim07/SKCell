using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("SKCell/Effects/SKOuterGlowEffect")]
    public class SKOuterGlowEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;

        public float _RimStrength = 1f;
        public float _RimPower = 1f;
        public float _RimSize = 1f;


        public Color colorTint = Color.yellow;
        public Color colorGlow = Color.white;

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
            alphaShader = Shader.Find("SKCell/OuterGlow");
            GetComponent<MeshRenderer>().material = _Material;
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetColor("_Color", colorTint);
            _material.SetColor("_RimColor", colorGlow);

            _material.SetFloat("_RimStrength", _RimStrength);
            _material.SetFloat("_RimPower", _RimPower);
            _material.SetFloat("_RimSize", _RimSize);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}