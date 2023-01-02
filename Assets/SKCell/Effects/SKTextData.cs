using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SKCell
{
    public class SKTextData
    {
        public SKTextCharData[] charData;

        public void ConstructCharData(TMP_Text text)
        {
            TMP_TextInfo info = text.textInfo;
            int len = info.meshInfo[0].vertexCount/4;
            charData = new SKTextCharData[len];
            int count = 0;
            SKTextCharData data = new SKTextCharData();
            data.vertices = new Vector3[4];
            data.oVertices = new Vector3[4];
            data.colors32 = new Color32[4];
            data.oColors32 = new Color32[4];
            data.tColors32 = new Color32[4];

            for (int i = 0; i < info.meshInfo[0].vertexCount; i++)
            {
                data.vertices[i % 4] = info.meshInfo[0].vertices[i];
                data.oVertices[i % 4] = info.meshInfo[0].vertices[i];

                //accomodate for update text in animator
                data.colors32[i % 4] = text.color.ToColor32();
                data.oColors32[i % 4] = text.color.ToColor32();

                data.tColors32[i % 4] = new Color32(255,255,255,255);
                if (i % 4 == 3) //new character every 4 vertices
                {
                    data.center = new Vector3((info.meshInfo[0].vertices[i - 1].x + info.meshInfo[0].vertices[i - 3].x) / 2,
                    (info.meshInfo[0].vertices[i - 1].y + info.meshInfo[0].vertices[i - 3].y) / 2);
                    data.index = count;
                    charData[count++] = data;
                    data = new SKTextCharData();
                    data.vertices = new Vector3[4];
                    data.oVertices = new Vector3[4];
                    data.colors32 = new Color32[4];
                    data.oColors32 = new Color32[4];
                    data.tColors32 = new Color32[4];
                }
            }
        }
    }

    public class SKTextCharData
    {
        public int index = 0;
        public Vector3[] vertices; //0: bottom left, 1: upper left, 2: upper right, 3: bottom right
        public Vector3[] oVertices;// original vertices
        public Color32[] colors32; //0: bottom left, 1: upper left, 2: upper right, 3: bottom right
        public Color32[] oColors32; // original colors
        public Color32[] tColors32; // typewriter colors
        public Vector3 center; //center pos for vertices
        public float scale= 1.0f;
        public float angle_rad = 0.0f;
        public Vector3 rot_center = -Vector3.one; //0,0: bottom left, 1,0: upper left, 1,1: upper right, 0,1: bottom right
        public Vector3 translation = Vector3.zero;
        public Vector3 shake_translation = Vector3.zero;
    }
}
