using UnityEngine;
using System.Collections;
using System;

public class MonoSingleton<T> : MonoBehaviour where T : Component {

    private static T _instance;

    public static T Instance
    {
        get
        {
            return MonoSingleton<T>.GetInstance();
        }
    }

    public static T GetInstance()
    {
        if (MonoSingleton<T>._instance == null)
        {
            Type typeFromHandle = typeof(T);
            MonoSingleton<T>._instance = (T)((object)UnityEngine.Object.FindObjectOfType(typeFromHandle));
            if (MonoSingleton<T>._instance == null)
            {
                GameObject gameObject = new GameObject(typeof(T).Name);
                MonoSingleton<T>._instance = gameObject.AddComponent<T>();
            }
        }
        return MonoSingleton<T>._instance;
    }

    public static void DestroyInstance()
    {
        if (MonoSingleton<T>._instance != null)
        {
            UnityEngine.Object.Destroy(MonoSingleton<T>._instance.gameObject);
        }
		MonoSingleton<T>._instance = null;
    }
		
    protected virtual void Awake()
    {
        if (MonoSingleton<T>._instance != null && MonoSingleton<T>._instance.gameObject != base.gameObject)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(base.gameObject);
            }
        }
        else if (MonoSingleton<T>._instance == null)
        {
            MonoSingleton<T>._instance = base.GetComponent<T>();
        }
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        this.Init();
    }

    protected virtual void OnDestroy()
    {
        if (MonoSingleton<T>._instance != null && MonoSingleton<T>._instance.gameObject == base.gameObject)
        {
			MonoSingleton<T>._instance = null;
        }
    }

    public static bool HasInstance()
    {
        return MonoSingleton<T>._instance != null;
    }

    protected virtual void Init()
    {
    }
}
