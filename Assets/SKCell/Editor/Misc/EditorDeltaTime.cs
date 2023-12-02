using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace SKCell
{
    [InitializeOnLoad]
    public static class EditorDeltaTime
    {
        private static Stopwatch stopwatch;
        private static float lastTime;

        static EditorDeltaTime()
        {
            stopwatch = new Stopwatch();
            EditorApplication.update += Update;
            stopwatch.Start();
        }

        public static float DeltaTime => lastTime;

        private static void Update()
        {
            lastTime = (float)stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();
        }
    }
}