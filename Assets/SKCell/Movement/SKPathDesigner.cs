using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [SelectionBase]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("SKCell/Movement/SKPathDesigner")]
    /// <summary>
    /// Translates this object along a path.
    /// </summary>
    public class SKPathDesigner : MonoBehaviour
    {
        #region Fields
        public TranslateMode translateMode;
        [Range(0f, 1f)] [Tooltip("0-1 value representing the position along the whole path.")]
        public float normalizedTime = 0.0f;
        public float speed = 1.0f;
        [Range(3, 20)] [Tooltip("Segment count of each bezier curve. Default is 8.")]
        public int bezierSegments = 8;
        [Range(0.001f, 2f)] [Tooltip("SqrMagnitude of distance from object to waypoint to be judged as 'reached the point'.")]
        public float onPointThreshold = 0.5f;
        public bool startOnAwake = true;

        [HideInInspector]
        public List<SKTranslatorWaypoint> waypoints = new List<SKTranslatorWaypoint>();
        [HideInInspector] public float selfWaitTime = 0;
        private List<float> distances = new List<float>(); // Distance between waypoints
        private float totalDistance; // Sum of length of paths

        private Vector3 oPos;
        private bool isPlaying = false, isWaiting = false;
        private float waitTimer;
        private int moveDir = 1;
        [HideInInspector]
        public int curWaypoint = -1, nextWaypoint = -1;
        #endregion
        private void Start()
        {
            oPos = transform.position;
            UpdateDistances();
            UpdateBezier();
            if (startOnAwake)
            {
                StartPath();
            }
        }

        private void FixedUpdate()
        {
            CheckWait();
            if (isWaiting)
            {
                waitTimer -= Time.fixedDeltaTime;
                if (waitTimer <= 0)
                {
                    isWaiting = false;
                }
                return;
            }
            if (isPlaying)
            {
                normalizedTime += speed * moveDir * 0.3f * Time.fixedDeltaTime;
                if(translateMode == TranslateMode.OneTime)
                {
                    if (normalizedTime >= 1.0f)
                    {
                        normalizedTime = 1.0f;
                        isPlaying = false;
                    }
                }
                else if(translateMode == TranslateMode.PingPong)
                {
                    if (normalizedTime >= 1.0f)
                        moveDir = -1;
                    if (normalizedTime <= 0.0f)
                        moveDir = 1;
                }
                else if (translateMode == TranslateMode.Repeat)
                {
                    if (normalizedTime >= 1.0f)
                    {
                        normalizedTime = 0.0f;
                    }
                }
                Vector3 diff = transform.position - oPos;
                Vector3 nPos =  GetNormalizedWPosition();
                transform.position = nPos - diff;
            }
        }
        private void CheckWait()
        {
            Vector3 targetPos = nextWaypoint == -1 ? oPos : (waypoints[nextWaypoint].localPosition + oPos);
            if ((transform.position - targetPos).sqrMagnitude < onPointThreshold)
            {
                isWaiting = true;
                if (nextWaypoint == -1)
                {
                    waitTimer = selfWaitTime;
                }
                else{
                    waitTimer = waypoints[nextWaypoint].stayTime;
                }
                curWaypoint = nextWaypoint;
                if (translateMode == TranslateMode.PingPong)
                {
                    if(curWaypoint == waypoints.Count - 1)
                    {
                        nextWaypoint = curWaypoint - 1;
                    }
                    else if (curWaypoint == -1)
                    {
                        nextWaypoint = 0;
                    }
                    else
                    {
                        if (moveDir == 1)
                        {
                            nextWaypoint = curWaypoint + 1;
                        }
                        else
                        {
                            nextWaypoint = curWaypoint - 1;
                        }
                    }
                }
                else
                {
                    if (moveDir == 1)
                    {
                        nextWaypoint = curWaypoint == waypoints.Count - 1 ? -1 : curWaypoint + 1;
                    }
                    else
                    {
                        nextWaypoint = curWaypoint == -1 ? 0 : curWaypoint - 1;
                    }
                }
            }
        }
        /// <summary>
        /// Start movement along the path. Will restart if called during a movement.
        /// </summary>
        public void StartPath()
        {
            if (waypoints.Count == 0)
                return;

            moveDir = 1;
            curWaypoint = -1;
            nextWaypoint = -1;
            isPlaying = true;
        }

        /// <summary>
        /// Pause the ongoing movement.
        /// </summary>
        public void PausePath()
        {
            isPlaying = false;
        }
        /// <summary>
        /// Resume the ongoing movement.
        /// </summary>
        public void ResumePath()
        {
            isPlaying = true;
        }
        public SKTranslatorWaypoint GetLastWayPoint()
        {
            return waypoints.Count > 0 ? waypoints[waypoints.Count-1] : null;
        }
        public void AddWaypoint(SKTranslatorWaypoint waypoint)
        {
            waypoints.Add(waypoint);
            UpdateDistances();
            UpdateBezier();
        }
        public void DeleteWaypoint(SKTranslatorWaypoint waypoint)
        {
            waypoints.Remove(waypoint);
            UpdateDistances();
            UpdateBezier();
        }
        public void DeleteWaypoint(int i)
        {
            waypoints.RemoveAt(i);
            UpdateDistances();
            UpdateBezier();
        }

        /// <summary>
        /// Get the world position of a waypoint given an index;
        /// </summary>
        /// <param name="waypoint"></param>
        /// <returns></returns>
        public Vector3 GetWaypointWPos(int waypointIndex, bool includeInitialWaypoint)
        {
            if(!includeInitialWaypoint)
                return oPos + waypoints[waypointIndex].localPosition;
            else
            {
                if (waypointIndex == 0) return oPos;
                else
                    return oPos+ waypoints[waypointIndex-1].localPosition;
            }
        }

        /// <summary>
        /// Get the world position evaluated at time t (0-1) on the path.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 Evaluate(float t)
        {
            return GetNormalizedWPosition(t);
        }
        public Vector3 GetNormalizedWPosition()
        {
            return GetNormalizedWPosition(normalizedTime);
        }
        /// <summary>
        /// Get the world position evaluated at time t (0-1) on the path.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetNormalizedWPosition(float t)
        {
            if (waypoints.Count == 0)
                return transform.position;
            if (t <= 0)
                return transform.position;
            else if (t >= 1)
                return transform.position + waypoints[waypoints.Count - 1].localPosition;

            float curPercentage = 0.0f;
            for (int i = 0; i < distances.Count; i++)
            {
                float perc = distances[i] / totalDistance;
                //normalized time in this interval
                if (curPercentage + perc >= t)
                {
                    float p = (t - curPercentage) / perc;
                    float l = SKCurveSampler.SampleCurve(waypoints[i].curve, p);
                    if (i != 0)
                    {
                        if (waypoints[i].type == SKTranslatorWaypointType.Line)
                            return Vector3.Lerp(waypoints[i - 1].localPosition + transform.position, waypoints[i].localPosition + transform.position, l);
                        else if (waypoints[i].type == SKTranslatorWaypointType.Bezier)
                            return waypoints[i].bezier.Sample(l) + waypoints[i].localPosition + transform.position;
                    }
                    else
                    {
                        if (waypoints[i].type == SKTranslatorWaypointType.Line)
                            return Vector3.Lerp(transform.position, waypoints[i].localPosition + transform.position, l);
                        else if (waypoints[i].type == SKTranslatorWaypointType.Bezier)
                            return waypoints[i].bezier.Sample(l) + waypoints[i].localPosition + transform.position;
                    }
                }

                curPercentage += perc;
            }
            return transform.position;
        }

        public void UpdateDistances()
        {
            distances.Clear();
            totalDistance = 0.0f;
            for(int i = 0; i < waypoints.Count; i++)
            {
                Vector3 diff;
                float dist = 0;
                if (waypoints[i].type== SKTranslatorWaypointType.Line)
                {
                    diff = i == 0 ? (waypoints[i].localPosition) : (waypoints[i].localPosition - waypoints[i - 1].localPosition);
                    dist = diff.magnitude;
                }
                else if (waypoints[i].type == SKTranslatorWaypointType.Bezier)
                {
                    dist = waypoints[i].bezier.Length();
                }
                totalDistance += dist;
                distances.Add(dist);
            }
        }

        public void UpdateBezier()
        {
            for(int i = 0; i < waypoints.Count; i++)
            {
                SKTranslatorWaypoint point = waypoints[i];
                if (i == 0)
                    point.bezier.p0 = -point.localPosition;
                else
                    point.bezier.p0 = waypoints[i - 1].localPosition-point.localPosition;
                point.bezier.p3 = Vector3.zero;
            }
        }
    }

    public enum TranslateMode
    {
        OneTime,
        PingPong,
        Repeat,
    }
}