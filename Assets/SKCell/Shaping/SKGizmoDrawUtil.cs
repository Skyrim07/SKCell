#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SKCell
{
    public struct SKGizmoDrawScope : IDisposable
    {
        private Color m_originalColor;
        private Matrix4x4 m_originalMatrix;

        public SKGizmoDrawScope(Color color)
        {
            m_originalColor = Gizmos.color;
            m_originalMatrix = Gizmos.matrix;
            Gizmos.color = color;
        }

        public SKGizmoDrawScope(Color color, Matrix4x4 matrix)
        {
            m_originalColor = Gizmos.color;
            m_originalMatrix = Gizmos.matrix;
            Gizmos.color = color;
            Gizmos.matrix = matrix;
        }

        public void Dispose()
        {
            Gizmos.color = m_originalColor;
            Gizmos.matrix = m_originalMatrix;
        }
    }
    public static class SKGizmoDrawUtil
    {
        private static readonly float ARROW_HEAD_LENGTH = 0.3f;
        private static readonly float ARROW_HEAD_ANGLE = 20.0f;
        private static readonly float PATH_POINT_RADIUS = 0.2f;

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            using (new SKGizmoDrawScope(color))
            {
                Gizmos.DrawLine(start, end);
            }
        }

        public static void DrawArrow(Vector3 start, Vector3 end, Color color)
        {
            using (new SKGizmoDrawScope(color))
            {
                Vector3 direction = end - start;
                Gizmos.DrawLine(start, end);
                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + ARROW_HEAD_ANGLE, 0) *
                                new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - ARROW_HEAD_ANGLE, 0) *
                               new Vector3(0, 0, 1);
                Gizmos.DrawRay(end, right * ARROW_HEAD_LENGTH);
                Gizmos.DrawRay(end, left * ARROW_HEAD_LENGTH);
            }
        }

        public static void DrawPath(Vector3[] path, Color color)
        {
            using (new SKGizmoDrawScope(color))
            {
                if (path.Length == 0)
                {
                    return;
                }

                Vector3 start = path[0];
                DrawSphere(start, PATH_POINT_RADIUS, color);
                if (path.Length > 1)
                {
                    for (int i = 1; i < path.Length; ++i)
                    {
                        Vector3 end = path[i];
                        DrawLine(start, end, color);
                        DrawSphere(end, PATH_POINT_RADIUS, color);
                        start = end;
                    }
                }
            }
        }

        public static void DrawBox(Vector3 center, Vector3 extends, Quaternion rotation, Color color)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            using (new SKGizmoDrawScope(color, matrix))
            {
                Gizmos.DrawWireCube(Vector3.zero, extends);
            }
        }

        public static void DrawSphere(Vector3 center, float radius, Color color)
        {
            using (new SKGizmoDrawScope(color))
            {
                Gizmos.DrawWireSphere(center, radius);
            }
        }

        public static void DrawCircle(Vector3 pos, Vector3 normal, float radius, Color color)
        {
            using (new Handles.DrawingScope(color))
            {
                Handles.DrawWireDisc(pos, normal, radius);
            }
        }

        public static void DrawSector(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
        {
            using (new Handles.DrawingScope(color))
            {
                Handles.DrawWireArc(center, normal, from, angle, radius);
            }
        }

        public static void DrawFan(Vector3 center, Vector3 up, Vector3 forward, float angle, float radius, Color color)
        {
            using (new Handles.DrawingScope(color))
            {
                var halfAngle = angle / 2;
                var right = Vector3.Cross(up, forward);
                var edgeRight = center
                                + forward * Mathf.Cos(halfAngle / 180f * Mathf.PI) * radius
                                + right * Mathf.Sin(halfAngle / 180f * Mathf.PI) * radius;
                var edgeLeft = center
                               + forward * Mathf.Cos(halfAngle / 180f * Mathf.PI) * radius
                               - right * Mathf.Sin(halfAngle / 180f * Mathf.PI) * radius;
                Handles.DrawLine(center, edgeRight);
                Handles.DrawLine(center, edgeLeft);
                Handles.DrawWireArc(center, up, forward, halfAngle, radius);
                Handles.DrawWireArc(center, up, forward, -halfAngle, radius);
            }
        }

        public static void DrawCapsule(Vector3 center, float radius, float height, Quaternion rotation, Color color)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            using (new Handles.DrawingScope(color, matrix))
            {
                var pointOffset = (height - (radius * 2)) / 2;
                Handles.DrawLine(Vector3.zero, Vector3.up * height / 2);
                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
                Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
                //draw front
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);
            }
        }

        public static void DrawPolyLineUnityGizmos(Matrix4x4 trans, List<Vector2> polyLine,
                                                   float baseHeightOffset, float polyAreaHeight,
                                                   float boxWidth, Color showColor, bool isClosed)
        {
            var curTrans = trans;
            Gizmos.color = showColor;
            for (int i = 0, n = polyLine.Count; i < n; i++)
            {
                if ((i == n - 1))
                {
                    if (n <= 2)
                    {
                        continue;
                    }
                    if (!isClosed)
                    {
                        continue;
                    }
                }
                var p1 = polyLine[i].XZ();
                var p2 = polyLine[(i + 1) % n].XZ();
                var curHeight = Mathf.Max(polyAreaHeight, 0.1f);
                p1.y = p2.y = baseHeightOffset;
                var curCenter = (p1 + p2) * 0.5f;
                curCenter.y += curHeight / 2;
                var dirHor = (p2 - p1).normalized;
                if (dirHor.magnitude == 0)
                {
                    continue;
                }
                var faceDir = Vector3.Cross(Vector3.up, (p2 - p1).normalized).normalized;
                if (faceDir.magnitude == 0)
                {
                    continue;
                }
                Quaternion rot = Quaternion.LookRotation(faceDir);
                Vector3 curSize = new Vector3((p2 - p1).MagnitudeXZ(), curHeight, boxWidth);
                var localT = Matrix4x4.TRS(curCenter, rot, curSize);
                Gizmos.matrix = curTrans * localT;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }
    }
}
#endif
