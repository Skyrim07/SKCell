using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell {
    [AddComponentMenu("SKCell/Effects/SKSpriteBlur")]
    public class SKSpriteBlur : PostEffectsBase
    {
        [Tooltip("Do not enable this unless you want this to update dynamically.")]
        public bool updateOnPlay = false;

        [Range(0, 1)]
        public float blur = 0.2f;

        [Range(1, 64)]
        public int sampleCount = 16;

        private SpriteRenderer image;
        private Shader alphaShader;
        private Material _materal;
        public Material _Material
        {
            get
            {
                _materal = CheckShaderAndCreateMaterial(alphaShader, _materal);
                return _materal;
            }
        }

        private void OnEnable()
        {
            alphaShader = Shader.Find("SKCell/SpriteBlur");
            image = GetComponent<SpriteRenderer>();

            image.material = _Material;
        }
        private void OnDisable()
        {
            image.material = null;
        }
        void Update()
        {
            if (Application.isPlaying)
            {
                if (!updateOnPlay)
                {
                    return;
                }
            }
            _Material.SetFloat("_Blur", blur * 0.7f);
            _Material.SetInt("_SampleCount", sampleCount);
        }
    }
}

