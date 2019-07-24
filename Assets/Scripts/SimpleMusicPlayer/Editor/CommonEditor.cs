using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CommonEditor : Editor  {


    [MenuItem("工具/清除pref")]
    public static void ClearPref()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("清除完毕");
    }

}
