using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    [AddComponentMenu("SKCell/UI/SKUIAnimation")]
    public class SKUIAnimation : MonoBehaviour
    {
        public bool enterOnStart = false;
        [HideInInspector] public bool isOn = false;

        [Header("Preset")]
        [Tooltip("Select 'None' if you want to use animations of your own.")]
        public SKUIAnimPreset preset;

        private Animator anim;

        private void Awake()
        {
            this.anim = GetComponent<Animator>();
            //Load animator
            RuntimeAnimatorController anim = null;
            if (preset != SKUIAnimPreset.CustomTwoState)
            {
                string path = SKAssetLibrary.UI_ANIM_PRESET_DIR_PATH.Substring(SKAssetLibrary.UI_ANIM_PRESET_DIR_PATH.IndexOf("SKCell")) + $"/{preset}";
                anim = Resources.Load<RuntimeAnimatorController>(path);
                this.anim.runtimeAnimatorController = anim;
            }

            SetState(enterOnStart);
        }

        public void SetState(bool appear)
        {
            if(anim)
                anim.SetBool("Appear", appear);
            isOn = appear;
        }
        public enum SKUIAnimPreset
        {
            CustomTwoState,
            InstantAppear,
            AlphaFade,
            FadeLeft,
            FadeRight,
            FadeUp,
            FadeDown,
            ScaleUp,
            ScaleDown,
            AlphaFadeSlow,
            InstantInFadeOut,
            Shine
        }
    }

}