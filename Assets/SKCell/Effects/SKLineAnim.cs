using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKLineAnim")]
    public class SKLineAnim : PostEffectsBase
    {
        [SKFolder("Attributes")]
        public float _Speed = 1.0f;
        public float _Amplitude = 1.0f, _Frequency = 1.0f, _Thickness = 0.2f, _Phase, _Length = 1.0f;
        [SKFolder("Colors")]
        public Color _BaseColor = Color.white;
        public Color _TipColor = Color.white, _Color = Color.white;

        #region References
        private SpriteRenderer sr;
        private Image image;
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
        #endregion
        private void OnEnable()
        {
            alphaShader = Shader.Find("SKCell/Curve");
            sr = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();

            if (sr)
                sr.material = _Material;
            if (image)
                image.material = _Material;
        }
        private void OnDisable()
        {
            image.material = null;
        }

        private void Update()
        {
            _materal.SetFloat("_Speed", _Speed);
            _materal.SetFloat("_Amplitude", _Amplitude);
            _materal.SetFloat("_Frequency", _Frequency);
            _materal.SetFloat("_Thickness", _Thickness);
            _materal.SetFloat("_Phase", _Phase);
            _materal.SetFloat("_Length", _Length);
            _materal.SetColor("_BaseColor", _BaseColor);
            _materal.SetColor("_TipColor", _TipColor);
            _materal.SetColor("_Color", _Color);
        }
    }
}
