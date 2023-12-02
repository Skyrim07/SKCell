using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/Effects/Light2D")]
    public sealed class SKLight2D : MonoBehaviour
    {
        public int rayCount = 10;
        [Range(0.2f, 50f)]
        public float radius = 1;
        [Range(0.1f, 10f)]
        public float fadePower = 4;
        public Color color;

        public LayerMask layerMask;

        private List<Vector2> hits = new List<Vector2>();
        private Material m_Material;
        private MeshFilter mf;
        private Mesh mesh;
        private Vector2 origin;
        private void OnEnable()
        {
            m_Material = GetComponent<MeshRenderer>().sharedMaterial;
            mf = GetComponent<MeshFilter>();
            mesh = new Mesh();
            origin = Vector2.zero;
        }

        private void Update()
        {
            hits.Clear();
            for (int i = 0; i < rayCount; i++)
            {
                float angle = Mathf.Deg2Rad * 360 * ((float)i / rayCount);
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, layerMask);
                if (hit.collider != null)
                {
                    hits.Add((hit.point - (Vector2)transform.position) / transform.localScale.x);
                }
                else
                {
                    hits.Add((origin + dir * radius) / transform.localScale.x);
                }
            }
            BuildMesh();

            m_Material.SetVector("origin", origin);
            m_Material.SetFloat("radius", radius);
            m_Material.SetFloat("_FadePower", fadePower);
            m_Material.SetVector("_Color", color);
        }

        private void BuildMesh()
        {
            float z = transform.position.z;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            vertices.Add(origin);
            for (int i = 0; i < hits.Count; i++)
            {
                vertices.Add(new Vector3(hits[i].x, hits[i].y, 0));
            }
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i == vertices.Count - 2 ? 1 : i + 2);
                triangles.Add(i + 1);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mf.mesh = mesh;

        }
    }
}