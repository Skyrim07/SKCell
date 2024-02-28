using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SKCell {
    [AddComponentMenu("SKCell/Effects/SKImageEffects")]
    [ExecuteInEditMode]
    public class SKImageEffects : PostEffectsBase
    {
        [Tooltip("Do not enable this unless you want this to update dynamically.")]
        public bool updateOnPlay = false;
        public SKImageEffectType type;

        [Range(-2f, 2f)]
        public float value = .5f;

        [SKFolder("Dissolve")]
        public Texture dissolve_NoiseTex;
        public Vector4 dissolve_st = new Vector4(1,1,0,0);
        public bool inverseColor;
        public bool useSpeed;
        public Vector2 zwSpeed = Vector2.zero;
        private Image image;
        private SpriteRenderer sr;
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
            alphaShader = Shader.Find("SKCell/ImageEffects");
            image = GetComponent<Image>();
            sr = GetComponent<SpriteRenderer>();
            if(image)
            image.material = _Material;
            if(sr)
                sr.material = _Material;

        }
        private void OnDisable()
        {
            if(image)
            image.material = null;
            if (sr)
                sr.material = null;
        }
        void Update()
        {
            if (useSpeed)
            {
                dissolve_st = new Vector4(dissolve_st.x, dissolve_st.y, dissolve_st.z + zwSpeed.x * Time.unscaledDeltaTime, dissolve_st.w + zwSpeed.y * Time.unscaledDeltaTime);
                _materal.SetVector("_NoiseTex_ST", dissolve_st);
            }

            if (Application.isPlaying)
            {
                if (!updateOnPlay)
                {
                    return;
                }
            }
            if (_materal == null)
                return;

            _materal.SetFloat("_Value", value);
            if(image)
            _materal.SetColor("_Color", image.color);
            if (sr)
                _materal.SetColor("_Color", sr.color);
            if (type== SKImageEffectType.Dissolve)
            {
                _materal.SetTexture("_NoiseTex", dissolve_NoiseTex);
                _materal.SetVector("_NoiseTex_ST", dissolve_st);
                _materal.SetInt("_InverseColor", inverseColor?1:0);
            }
        }
    }

    public enum SKImageEffectType
    {
        Dissolve,
    }
}

