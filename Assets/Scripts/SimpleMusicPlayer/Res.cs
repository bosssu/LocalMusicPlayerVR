using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Res
{
    public static Sprite LoadSprite(string path)
    {
        Texture2D t2d = LoadTexture(path);
        Sprite sp = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        return sp;
    }

    public static GameObject LoadObj(string path)
    {
        return GameObject.Instantiate<GameObject>(LoadObject(path) as GameObject);
    }

    public static void LoadGameObjectAsync(string path, Action<GameObject> callback)
    {
        LoadObjectAsync(path, (a) => {
            if (callback != null)
                callback(a as GameObject);
        });
    }

    public static AudioClip LoadAudioClip(string path)
    {
        return Resources.Load<AudioClip>(path);
    }

    public static void LoadAudioClipAsync(string path, Action<AudioClip> callback)
    {
        LoadObjectAsync(path, (a) => {
            if (callback != null) callback(a as AudioClip);
        });
    }

    public static Texture2D LoadTexture(string path)
    {
        return Resources.Load<Texture2D>(path);
    }

    public static UnityEngine.Object LoadObject(string path)
    {
        return Resources.Load<UnityEngine.Object>(path);
    }

    public static void LoadObjectAsync(string path, Action<UnityEngine.Object> callback)
    {
        ResourceRequest quest = Resources.LoadAsync<UnityEngine.Object>(path);
        quest.completed += (ay) =>
        {
            if (callback != null)
                callback(quest.asset);
        };
    }

}
