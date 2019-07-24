using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Utils  {


    public static T GetSafeComponet<T>(GameObject target) where T:Behaviour
    {
        T t = target.GetComponent<T>();
        if (t == null)
            t = target.AddComponent<T>();
        return t;
    }

    public static List<int> GetRandomNoSame(int minNum, int maxNum, int ResultCount)
    {
        List<int> numbers = new List<int>();
        List<int> finals = new List<int>();
        for (int i = minNum; i < maxNum; i++)
            numbers.Add(i);

        for (int i = 0; i < ResultCount; i++)
        {
            int index = Random.Range(0, numbers.Count);
            //Debug.Log(numbers[index]);
            finals.Add(numbers[index]);
            numbers.RemoveAt(index);
        }

        return finals;
    }

    /// <summary>
    /// 一次创建多级目录中的文件
    /// </summary>
    /// <param name="path">d:/aa/bb/c.txt</param>
    public static void CreateFileRecuIfNotExist(string path)
    {
        string dir = Path.GetDirectoryName(path);
        CreateDirRecuIfNotExist(dir);

        CreateFileIfNotExist(path);
    }

    /// <summary>
    /// 一次创建多级目录
    /// </summary>
    /// <param name="dir">d:/aa/bb</param>
    public static void CreateDirRecuIfNotExist(string dir)
    {
        if (dir.Contains("/"))
        {
            string[] p = dir.Split('/');

            string str = p[0];
            CreateDirIfNotExist(str);

            for (int i = 1; i < p.Length; i++)
            {
                if (!string.IsNullOrEmpty(p[i]))
                {
                    str += "/" + p[i];
                    CreateDirIfNotExist(str);
                }
            }
        }
    }

    public static void CreateDirIfNotExist(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static FileStream CreateFileIfNotExist(string path)
    {
        if (!File.Exists(path))
            return File.Create(path);

        return null;
    }

    public static void DestroyChildObjects(Transform parent,List<string> ignore_objects)
    {
        if (parent.childCount == 0) return;

        List<GameObject> childs = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if(!ignore_objects.Contains(child.name))
                childs.Add(child.gameObject);
        }

        for (int i = 0; i < childs.Count; i++)
            GameObject.Destroy(childs[i]);
    }

}

