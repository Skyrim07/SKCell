using UnityEngine;
using UnityEngine.SceneManagement;
namespace SKCell
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        static DontDestroyOnLoad inst = null;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLevelLoaded;

        }
        private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
        {
            if (inst == null)
                inst = this;
            if (inst != this)
                Destroy(gameObject);
        }

    }
}