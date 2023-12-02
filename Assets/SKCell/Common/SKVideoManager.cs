using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SKCell {
    public class SKVideoManager : SKSingleton<SKVideoManager>
    {
        /// <summary>
        /// Plays video in scene. Uses resources path.
        /// </summary>
        /// <param name="path">Path inside the resources folder. No suffixes.</param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        public static VideoPlayer PlayVideo(string path, bool isLoop=false)
        {
            string videoPath = path;
            if (!string.IsNullOrEmpty(path))
            {
                videoPath = path;
            }
            GameObject videoCameraGo = new GameObject("VideoCamera");
            Camera VideoCamera = videoCameraGo.AddComponent<Camera>();
            VideoCamera.depth = 5;
            var videoPlayer = VideoCamera.gameObject.AddComponent<VideoPlayer>();
            videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
            videoPlayer.targetCameraAlpha = 1f;
            videoPlayer.clip = Resources.Load<VideoClip>(path);
            videoPlayer.loopPointReached += EndReached;
            videoPlayer.isLooping = isLoop;
            videoPlayer.Play();

            return videoPlayer;
        }

        /// <summary>
        /// Plays video on a raw image. Uses resources path.
        /// </summary>
        /// <param name="path">Path inside the resources folder. No suffixes.</param>
        /// <param name="rawImage"></param>
        public static void PlayVideoInUI(string path, RawImage rawImage)
        {
            if (rawImage == null)
            {
                return;
            }
            RenderTexture re = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            string videoPath = path;
            if (!string.IsNullOrEmpty(path))
            {
                videoPath = path;
            }
            rawImage.texture = re;
            VideoPlayer videoPlayer = rawImage.gameObject.GetOrAddComponent<UnityEngine.Video.VideoPlayer>();
            videoPlayer.clip = Resources.Load<VideoClip>(path);
            videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
            videoPlayer.isLooping = false;
            videoPlayer.targetTexture = re;
            videoPlayer.loopPointReached += EndReached;
            videoPlayer.Play();
            rawImage.gameObject.SetActive(true);
        }
        private static void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            vp.Stop();
            GameObject.Destroy(vp.gameObject);
        }
    }
}
