using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace SKCell
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("SKCell/Effects/SKImageProcessing")]
    public class SKImageProcessing : PostEffectsBase
    {
        #region Properties
        [Tooltip("Do not enable this unless you want this to update dynamically.")]
        public bool updateOnPlay = true;

        [SKFolder("Blend Mode")]
        public BlendMode srcBlend = BlendMode.SrcAlpha;
        public BlendMode dstBlend = BlendMode.OneMinusSrcAlpha;

        [SKFolder("Alpha Fade")]
        [Range(0, 1)]
        public float leftX = 0;
        [Range(0, 1)]
        public float rightX = 0;
        [Range(0, 1)]
        public float topY = 0;
        [Range(0, 1)]
        public float bottomY = 0;
        [Range(-2, 0)]
        public float alphaSmooth = 0;
        public Texture2D alphaMask;

        [SKFolder("Color Properties")]
        [Range(0, 1)]
        public float colorShift = 1;
        [Range(0, 5)]
        public float brightness = 1;
        [Range(0, 5)]
        public float saturation = 1;
        [Range(0, 5)]
        public float contrast = 1;

        [SKFolder("Outline and Rim Light")]
        public bool active = false;

        public Color rimColor = Color.white;
        [Range(0,2)]
        public float rimAlphaThreshold = 1f;
        [Range(0,1)]
        public float baseAlphaThreshold = 0.2f;
        [Range(0,5)]
        public float dampRate = 2f;

        #endregion

        #region References
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
        protected override void Start()
        {
            base.Start();
            UpdateFields();
        }
        private void OnEnable()
        {
            alphaShader = Shader.Find("SKCell/ImageProcessing");
            image = GetComponent<Image>();

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
            UpdateFields();
        }
        private void UpdateFields()
        {
            _Material.SetInt("_SrcBlendMode", (int)srcBlend);
            _Material.SetInt("_DstBlendMode", (int)dstBlend);

            _Material.SetFloat("_AlphaLX", leftX * 2);
            _Material.SetFloat("_AlphaRX", ((1 - rightX) - 0.5f) * 2);
            _Material.SetFloat("_AlphaTY", ((1 - topY) - 0.5f) * 2);
            _Material.SetFloat("_AlphaBY", bottomY * 2);
            _Material.SetFloat("_AlphaPower", alphaSmooth);
            _Material.SetFloat("_Brightness", brightness);
            _Material.SetFloat("_Saturation", saturation);
            _Material.SetFloat("_Contrast", contrast);
            _Material.SetFloat("_Hue", colorShift);

            _Material.SetInt("_ShowOutline", SKUtils.BoolToInt(active));
            _Material.SetFloat("_EdgeAlphaThreshold", rimAlphaThreshold);
            _Material.SetFloat("_BaseAlphaThreshold", baseAlphaThreshold);
            _Material.SetFloat("_EdgeDampRate", dampRate);
            _Material.SetColor("_EdgeColor", rimColor);
        }
    }
}