using UnityEngine;
using System.Collections;

namespace SKCell
{
    [ExecuteInEditMode]
    public class PostEffectsBase : MonoBehaviour
    {

        // Called when start
        protected void CheckResources()
        {
            bool isSupported = CheckSupport();

            if (isSupported == false)
            {
                NotSupported();
            }
        }

        // Called in CheckResources to check support on this platform
        protected bool CheckSupport()
        {
            return true;
        }

        // Called when the platform doesn't support this effect
        protected void NotSupported()
        {
            enabled = false;
        }

        protected virtual void Start()
        {
            CheckResources();
        }

        // Called when need to create the material used by this effect
        protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
        {
            if (shader == null)
            {
                return null;
            }

            if (shader.isSupported && material && material.shader == shader)
                return material;

            if (!shader.isSupported)
            {
                return null;
            }
            else
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.DontSave;
                if (material)
                    return material;
                else
                    return null;
            }
        }
    }
}
