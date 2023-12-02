using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace SKCell
{
    public class SKTimeManager : SKMonoSingleton<SKTimeManager>
    {
        public static bool locked = false;
        private float _fixedDeltaTime;
        private const float _defaultRestoreSpeed = 1f;
        private float _restoreSpeed;
        private bool _restore = false;

        protected override void Awake()
        {
            base.Awake();
            _fixedDeltaTime = Time.fixedDeltaTime;
            _restoreSpeed = _defaultRestoreSpeed;
        }

        void Update()
        {
            if (locked)
                return;

            if (_restore)
            {
                if (Time.timeScale < 1f)
                {
                    Time.timeScale += Time.unscaledDeltaTime * _restoreSpeed;
                }
                else
                {
                    Time.timeScale = 1f;
                    _restore = false;
                }

                SyncFixedDeltaTime();
            }
        }

        private void SyncFixedDeltaTime()
        {
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        public void StartSlowMotion(float targetTimeScale, float duration, float restoreSpeed = 0f)
        {
            if (restoreSpeed > 0f)
                _restoreSpeed = restoreSpeed;
            else
                _restoreSpeed = _defaultRestoreSpeed;

            Assert.IsTrue(duration > 0f);
            Assert.IsTrue(targetTimeScale > 0f && targetTimeScale < 1f);

            StopCoroutine(RestoreTimeOnDelay(duration));
            StartCoroutine(RestoreTimeOnDelay(duration));

            Time.timeScale = targetTimeScale;
            SyncFixedDeltaTime();
        }

        private IEnumerator RestoreTimeOnDelay(float delay)
        {
            _restore = true;
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}