using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    public class SKPostProcessManager : Singleton<SKPostProcessManager>
    {
        public Camera cam;
        public List<SKPostProcessMask_Saturation> sat_masks = new List<SKPostProcessMask_Saturation>();
        public Texture2D tex_sat = new Texture2D(Screen.width, Screen.height);
        private Color[] colors = new Color[Screen.height*Screen.width];


        private Vector4[] pos_sat = new Vector4[16];
        private float[] radius_sat = new float[16];
        private float[] blur_sat = new float[16];
        private float[] value_sat = new float[16];
        private float[] shape_sat = new float[16];

        public void UpdatePPMask(Material mat)
        {
            pos_sat = new Vector4[16];
            radius_sat = new float[16];
            blur_sat = new float[16];
            value_sat = new float[16];
            for (int i = 0; i < sat_masks.Count; i++)
            {
                if (sat_masks[i] == null)
                    continue;

                PPMask mask = sat_masks[i].mask;

                Vector3 pos = sat_masks[i].transform.position;
                Vector4 wPos = new Vector4(pos.x, pos.y, pos.z, 1);
                wPos = cam.projectionMatrix * wPos;

                //Screen Pos
                Vector4 sPos = (wPos / wPos.w);
                sPos /= 2f;
                sPos.x += .5f;
                sPos.y+= .5f;

                //update arrays
                pos_sat[i] = sPos;
                radius_sat[i] = mask.radius;
                blur_sat[i] = mask.blur;
                value_sat[i] = 1-sat_masks[i].saturation;
                shape_sat[i] = (int)mask.shape;
            }
            mat.SetVectorArray("pos_sat", pos_sat);
            mat.SetFloatArray("radius_sat", radius_sat);
            mat.SetFloatArray("blur_sat", blur_sat);
            mat.SetFloatArray("value_sat", value_sat);
            mat.SetFloatArray("shape_sat", shape_sat);
        }
    }
    /*
             /// <summary>
        /// Generate texture for each mask effect. Send to SKPostProcessCamera for processing.
        /// </summary>
        public void GenerateMaskTextures()
        {
            if (cam == null)
            {
                CommonUtils.EditorLogWarning("SKPostProcessManager: No active PP camera. Add the SKPostProcessingCamera component first.");
                return;
            }

            //saturation
            tex_sat = new Texture2D(Screen.width, Screen.height);
      
            //convert sprites to screen space
            for (int i = 0; i < sat_masks.Count; i++)
            {
                //Texture2D tex = sat_masks[i].maskSprite.sprite.texture;

                Vector3 pos = sat_masks[i].transform.position;
                Vector4 wPos = new Vector4(pos.x, pos.y, pos.z,1);
                wPos = cam.projectionMatrix*wPos;

                //Screen Pos
                Vector4 sPos = (wPos / wPos.w);
                //WriteToTexture(tex_sat, sPos, new Vector2(128, 128));
                WriteToTexture(tex_sat, new Vector2(.5f,.5f), new Vector2(.1f,.1f));
                //Debug.Log(sPos);
            }
        }
        private void WriteToTexture(Texture2D tex, Vector2 pos01, Vector2 size01)
        {
            pos01.x *= tex.width;
            size01.x *= tex.width;
            pos01.y *= tex.height;
            size01.y *= tex.height;

            size01.x /= (float)tex.width / tex.height;

            Vector2Int startPos = new Vector2Int(((int)(pos01 - size01/2f).x), (int)(pos01 - size01/2f).y);
            for (int i = 0; i < size01.x ; i++)
            {
                for (int j = 0; j < size01.y; j++)
                {
                    Vector2Int p = startPos + new Vector2Int(i, j);
                    if(p.x<0||p.x>tex.width || p.y < 0 || p.y > tex.height)
                    {
                        continue;
                    }
                    colors[i * Screen.width + j] = Color.black;
                }
            }

            tex.SetPixels(startPos.x, startPos.y, (int)size01.x, (int)size01.y, colors);
            tex.Apply();
        } 
     
     
     */

    [System.Serializable]
    public struct PPMask
    {
        public PPMaskShape shape;
        [Range(0.0f,2.0f)]
        public float radius;
        [Range(0.0f, 1.0f)]
        public float blur;
    }

    public enum PPMaskShape
    {
        Circle,
        Rect,
    }
}