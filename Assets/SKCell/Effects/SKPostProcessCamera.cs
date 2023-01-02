using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public sealed class SKPostProcessCamera : MonoBehaviour
    {
        private Material mat;
        private Shader shader;

        //Get mask texture from manager

        private void OnEnable()
        {
            shader = Shader.Find("SKCell/SKPostProcessMask");
            mat = new Material(shader);

            SKPostProcessManager.instance.cam = GetComponent<Camera>();
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            SKPostProcessManager.instance.UpdatePPMask(mat);
            Graphics.Blit(source, destination, mat);
        }
    }
}
