using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonoInstance : MonoSingleton<MonoInstance> {


    public void LoadPage(string url,Action<string> callback)
    {
        StartCoroutine(LoadWebPage(url, callback));
    }

    IEnumerator LoadWebPage(string url, Action<string> callback)
    {
        WWW ww = new WWW(url);
        yield return ww;
        if (string.IsNullOrEmpty(ww.error))
        {
            if (callback != null) callback(ww.text);
        }
    }
}
