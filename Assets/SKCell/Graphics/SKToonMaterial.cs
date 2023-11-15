using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("SKCell/Effects/SKToonMaterial")]
    public class SKToonMaterial : PostEffectsBase
    {
        public bool updateOnPlay = true;
        [Range(1, 10)]
        public int _Steps = 3;

        public Color colorTint = Color.yellow;
        public Color colorSpecular = Color.white;
        public float _SpecularScale = 3;

        public Color colorOutline = Color.white;
        public float _OutlineWidth = 0.5f;

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
            alphaShader = Shader.Find("SKCell/Toon_0");

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if(mr)
                mr.material = _Material;
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetColor("_Color", colorTint);
            _material.SetColor("_Specular", colorSpecular);
            _material.SetColor("_OutlineColor", colorOutline);

            _material.SetFloat("_Steps", _Steps);
            _material.SetFloat("_SpecularScale", _SpecularScale);
            _material.SetFloat("_OutlineWidth", _OutlineWidth);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}