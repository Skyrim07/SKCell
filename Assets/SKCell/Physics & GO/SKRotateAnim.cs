using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/Physics & GO/SKRotateAnim")]
    public class SKRotateAnim : MonoBehaviour
    {
        public string uid = string.Empty;
        [HideInInspector]
        public bool active = true;

        [Header("Settings")]
        public float speed = 1;
        public RotationOrientation orientation = RotationOrientation.Clockwise;
        public RotationMode mode = RotationMode.Linear;
        public bool xAxis = false, yAxis = false, zAxis = true;

        [Header("Speed Randomization")]
        public bool enableSpeedRandomization=false;
        public float minRandomSpeed = 0.8f, maxRandomSpeed = 1.2f;

        [Header("Orientation Randomization")]
        public bool enableOrientationRandomization = false;

        private float accSpeed = 0;
        private void Start()
        {
            if (enableSpeedRandomization)
                speed *= Random.Range(minRandomSpeed, maxRandomSpeed);

            if (enableOrientationRandomization)
                orientation = (RotationOrientation)Random.Range(0, 2);

            if (orientation == RotationOrientation.Clockwise)
                speed = -speed;
        }
        private void FixedUpdate()
        {
            if (!active)
            {
                return;
            }
            
            accSpeed += speed;
            transform.Rotate(new Vector3(xAxis ? speed : 0, yAxis ? speed : 0, zAxis ? speed : 0));
            if (mode == RotationMode.Pingpong)
            {
                if (Mathf.Abs(accSpeed) >= 180)
                {
                    speed = -speed;
                }
            }
        }
    }

    public enum RotationOrientation
    {
        Clockwise,
        CounterClockwise
    }
    public enum RotationMode
    {
        Linear,
        Pingpong
    }
}
