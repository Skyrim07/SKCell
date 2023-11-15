using UnityEngine;
using UnityEngine.UI;
namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/SKBackBlur")]
    public class SKBackBlur : PostEffectsBase
    {
        #region Properties
        [Tooltip("Do not enable this unless you want this to update dynamically.")]
        public bool updateOnPlay = false;

        public Color additiveColor = Color.black;
        public Color multiplicativeColor = Color.white;

        [Range(0, 20)]
        public float blur = 5;

        #endregion

        #region References
        private SpriteRenderer sprite;
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
            alphaShader = Shader.Find("SKCell/TintedUIBlur");
            sprite = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();

            if (sprite != null)
                sprite.material = _Material;

            if (image != null)
                image.material = _Material;
        }
        private void OnDisable()
        {
            if (sprite != null)
                sprite.material = null;

            if (image != null)
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
            _Material.SetColor("_AdditiveColor", additiveColor);
            _Material.SetColor("_MultiplyColor", multiplicativeColor);
            _Material.SetFloat("_Size", blur);
        }
    }
}