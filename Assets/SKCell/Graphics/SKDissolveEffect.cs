using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKDissolveEffect")]
    public class SKDissolveEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;
        [Range(-1f, 1f)]
        public float dissolveAmount = 0f;
        public Vector2 dissolveSize = new Vector2(0.7f, 0.7f);

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
            Texture2D noise = Resources.Load<Texture2D>("SKCell/Textures/Noise_Random");
            alphaShader = Shader.Find("SKCell/Dissolve_0");

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if(mr)
                mr.material = _Material;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
                sr.material = _Material;
            _material.SetTexture("_Noise", noise);
            _material.SetFloat("_HighlightThreshold", 0.1f);
            UpdateParamaters();
        }

        private void UpdateParamaters()
        {
            _material.SetTextureScale("_Noise", dissolveSize);
            _material.SetColor("_Color", colorTint);
            _material.SetColor("_HighlightColor", colorHighlight);

            _material.SetFloat("_Threshold", dissolveAmount);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}