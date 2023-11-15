using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKDitherAlphaEffect")]
    public class SKDitherAlphaEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;

        public float alpha = 0.5f;

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
            alphaShader = Shader.Find("SKCell/Dither");
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr)
                mr.material = _Material;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
                sr.material = _Material;
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetFloat("_Alpha", alpha);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}