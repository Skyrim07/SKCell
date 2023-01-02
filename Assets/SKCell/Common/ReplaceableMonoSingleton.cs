using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceableMonoSingleton<T> : MonoBehaviour where T : Component
{
    protected static T mInstance;
    public float InitializationTime;

    public static T instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<T>();
                if (mInstance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    mInstance = obj.AddComponent<T>();
                }
            }

            return mInstance;
        }
    }

    protected virtual void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        InitializationTime = Time.time;

        DontDestroyOnLoad(this.gameObject);
        T[] check = FindObjectsOfType<T>();
        foreach (T searched in check)
        {
            if (searched != this)
            {
                if (searched.GetComponent<ReplaceableMonoSingleton<T>>().InitializationTime < InitializationTime)
                {
                    Destroy(searched.gameObject);
                }
            }
        }

        if (mInstance == null)
        {
            mInstance = this as T;
        }
    }
}
