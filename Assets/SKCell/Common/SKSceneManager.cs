using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace SKCell
{
    [AddComponentMenu("SKCell/Misc/SKSceneManager")]

    public class SKSceneManager : SKMonoSingleton<SKSceneManager>
    {
        private static AsyncOperation async = null;
        private static float actualAsyncProgress = 0;

        [SKFolder("Async Settings")]
        public float asyncLoadLag = 3f;
        public float asyncAfterFullLag = 1.5f;
        public float asyncLagLowerThreshold = 0.5f, asyncLagUpperThreshold = 0.8f;

        [SKFolder("Async Events")]
        public UnityEvent onStartLoad;
        public UnityEvent onLoadingSceneLoaded,onNextSceneLoaded, onLoadingBarFull, onAwake;

        [SerializeField] SKUIPanel loadFader;

        protected override void Awake()
        {
            base.Awake();
            onAwake.Invoke();
        }
        #region Methods
        /// <summary>
        /// Instantly load the next scene.
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneSync(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        /// <summary>
        /// Go to a loading scene and then the next scene.
        /// </summary>
        /// <param name="sceneName"></param>
        public bool LoadSceneAsync(string loadingSceneName, string sceneName)
        {
            if (async != null)
                return false;
            StartCoroutine(LoadSceneAsyncCR(loadingSceneName, sceneName));
            return true;
        }
        /// <summary>
        /// Start an async process with the given slider and text presented as load progress. (e.g. Text is displayed as "99.99%" if with a precision of 2.")
        /// </summary>
        /// <param name="loadingSceneName"></param>
        /// <param name="sceneName"></param>
        /// <param name=""></param>
        public bool LoadSceneAsync(string loadingSceneName, string sceneName, SKSlider loadBar=null)
        {
            if (async != null)
                return false;
            StartCoroutine(LoadSceneAsyncCR(loadingSceneName, sceneName,loadBar));
            return true;
        }

        private IEnumerator LoadSceneAsyncCR(string loadingSceneName, string sceneName, SKSlider loadBar=null)
        {
            //Cast Fader
            loadFader.SetState(SKUIPanelState.Active);
            yield return new WaitForSeconds(0.2f);

            onStartLoad.Invoke();
            //Load the loading scene first

            SceneManager.LoadScene(loadingSceneName);
            yield return new WaitForSeconds(0.05f);
            onLoadingSceneLoaded.Invoke();

            loadFader.SetState(SKUIPanelState.Inactive);

            //Then load the next scene
            async = SceneManager.LoadSceneAsync(sceneName);
            actualAsyncProgress = 0f;
            async.allowSceneActivation = false;
            float randomThreshold = UnityEngine.Random.Range(asyncLagLowerThreshold, asyncLagUpperThreshold);
            while (async.progress < 0.9f)
            {
                actualAsyncProgress = (async.progress / 0.9f)* randomThreshold;
                UpdateDisplay(loadBar);
                yield return new WaitForSecondsRealtime(0.02f);
            }

            //Process load lag
            float timer = 0;
            while (timer<asyncLoadLag)
            {
                UpdateDisplay(loadBar);
                actualAsyncProgress = randomThreshold + (1- randomThreshold) * (timer / asyncLoadLag);
                float randomDelta = UnityEngine.Random.Range(0.05f, asyncLoadLag/2f);
                timer += randomDelta;
                yield return new WaitForSecondsRealtime(randomDelta);
            }
            actualAsyncProgress = 1;
            UpdateDisplay(loadBar);
            onLoadingBarFull.Invoke();

            //Wait for after lag
            yield return new WaitForSecondsRealtime(asyncAfterFullLag);


            //Cast fader
            loadFader.SetState(SKUIPanelState.Active);
            yield return new WaitForSeconds(0.18f);
            //Load complete
            async.allowSceneActivation = true;
            onNextSceneLoaded.Invoke();

            yield return new WaitForSeconds(0.8f);
            loadFader.SetState(SKUIPanelState.Inactive);

            async = null;
            yield return null;
        }

        private static void UpdateDisplay(SKSlider loadBar)
        {
            if (loadBar != null)
            {
                loadBar.SetValue(actualAsyncProgress);
            }
        }

        public static float GetLoadProgress()
        {
            if (async == null)
            {
                SKUtils.EditorLogWarning("GetLoadProcess can only be called under an async process.");
                return 0;
            }
            if (async.isDone)
                return 1f;
            else
                return async.progress;
        }
        #endregion

    }
}
