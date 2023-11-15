using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKEdgeOutlineEffect")]
    public class SKEdgeOutlineEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;

        public float _Strength = 1.0f;

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
            alphaShader = Shader.Find("SKCell/EdgeOutline");
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
                sr.material = _Material;
            Image im = GetComponent<Image>();
            if (im)
                im.material = _Material;
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetFloat("_Strength", _Strength);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}