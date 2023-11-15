using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKWireframeEffect")]
    public class SKWireframeEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;

        public float width = 0.05f;


        public Color _FrontColor = Color.white;
        public Color _BackColor = Color.gray;

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
            alphaShader = Shader.Find("SKCell/Wireframe_1");
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
            _material.SetColor("_FrontColor", _FrontColor);
            _material.SetColor("_BackColor", _BackColor);

            _material.SetFloat("_WireframeVal", width);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}