﻿using UnityEngine;
using System.Collections;
using System;

public class Singleton<T> where T : class, new() {

    private static T s_instance;

    public static T Instance
    {
        get
        {
            if (Singleton<T>.s_instance == null)
            {
                Singleton<T>.CreateInstance();
            }
            return Singleton<T>.s_instance;
        }
    }

    public static void CreateInstance()
    {
        if (Singleton<T>.s_instance == null)
        {
            Singleton<T>.s_instance = Activator.CreateInstance<T>();
            (Singleton<T>.s_instance as Singleton<T>).Init();
        }
    }

    public static void DestroyInstance()
    {
        if (Singleton<T>.s_instance != null)
        {
            (Singleton<T>.s_instance as Singleton<T>).UnInit();
			Singleton<T>.s_instance = null;
        }
    }

    public static bool HasInstance()
    {
        return Singleton<T>.s_instance != null;
    }

    public virtual void Init()
    {
    }

    public virtual void UnInit()
    {
    }
}
