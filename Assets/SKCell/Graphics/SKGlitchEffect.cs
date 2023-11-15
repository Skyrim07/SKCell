using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKGlitchEffect")]
    public class SKGlitchEffect : PostEffectsBase
    {
        public bool updateOnPlay = true;

        public float splitX = 7.5f;
        public float splitY = 1f;
        public float speed = 15f;
        public float blockSize = 15f;

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
            alphaShader = Shader.Find("SKCell/BlockSplit");
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr)
                mr.material = _Material;
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
            _material.SetFloat("_MaxRGBSplitX", splitX);
            _material.SetFloat("_MaxRGBSplitY", splitY);
            _material.SetFloat("_Speed", speed);
            _material.SetFloat("_BlockSize", blockSize);
        }

        private void Update()
        {
            if (!updateOnPlay)
                return;
            UpdateParamaters();
        }
    }
}