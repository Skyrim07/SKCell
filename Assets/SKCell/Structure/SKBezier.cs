using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [System.Serializable] 
    public class SKBezier
    {
        public Vector3 p0, p1, p2, p3;

        public Vector3 Sample(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        public Vector3[] Path(int segments)
        {
            Vector3[] path = new Vector3[segments+1];
            float incr = 1.0f / segments;
            for(int i = 0; i <= segments; i++)
            {
                path[i] = Sample(incr * i);
            }
            return path;
        }
        public Vector3[] Path(int segments, Vector3 origin)
        {
            Vector3[] path = new Vector3[segments + 1];
            float incr = 1.0f / segments;
            for (int i = 0; i <= segments; i++)
            {
                path[i] = Sample(incr * i)+origin;
            }
            return path;
        }
        public float Length(int pointCount = 15)
        {
            if (pointCount < 2)
            {
                return 0;
            }

            float length = 0.0f;
            Vector3 lastPoint = Sample(0.0f / (float)pointCount);
            for (int i = 1; i <= pointCount; i++)
            {
                Vector3 point = Sample((float)i / (float)pointCount);
                length += Vector3.Distance(point, lastPoint);
                lastPoint = point;
            }
            return length;
        }
        public Vector3 Tangent(float t)
        {
            float u = 1 - t;
            float uu = u * u;
            float tu = t * u;
            float tt = t * t;

            Vector3 P = p0 * 3 * uu * (-1.0f);
            P += p1 * 3 * (uu - 2 * tu);
            P += p2 * 3 * (2 * tu - tt);
            P += p3 * 3 * tt;

            return P.normalized;
        }
        public int[] SegmentIndices(int segment)
        {
            List<int> indices = new List<int>();    
            for(int i = 0; i <= segment; i++)
            {
                if(i==0||i==segment)
                    indices.Add(i);
                else
                {
                    indices.Add(i);
                    indices.Add(i);
                }
            }
            return indices.ToArray();
        }
    }
}
