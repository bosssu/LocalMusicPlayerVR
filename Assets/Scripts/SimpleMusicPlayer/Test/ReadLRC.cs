using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class ReadLRC : MonoBehaviour
{
    public string basepath = "D:/EXTERNAL_ASSETS/AUDIO";

    private Text lrcText;
    float offest = 0f;
    private void Start()
    {
        lrcText = GetComponent<Text>();
        GetLrcFile(basepath + "/魏新雨 - 桃花鱼.lrc");
    }
    public List<string> GetLyricListAndTimeList(string lyricText, out List<float> timeA, out List<string> titleA)
    {
        List<string> lyricArray = new List<string>();
        timeA = new List<float>();
        titleA = new List<string>();
        string[] lineArray = lyricText.Split('\n');//根据分隔出行
        for (int i = 0; i < lineArray.Length; i++)
        {
            string lineStr = lineArray[i];
            if (lineStr.Contains("ti") || lineStr.Contains("ar") || lineStr.Contains("al") || lineStr.Contains("by") || lineStr.Contains("offset"))
            {//标题
                string[] array = lineStr.Split('[', ':', ']');
                float f;
                int value = array.Length - 2;
                string value_str = array[value];
                if (value_str != null)
                {
                    if (!float.TryParse(value_str, out f))
                    {
                        titleA.Add(value_str);
                    }
                    else
                    {
                        if (lineStr.Contains("offset"))
                            this.offest = f;
                    }
                }
            }
            else
            {//歌词
                string[] contentArray = lineStr.Split('[', ']');
                for (int j = contentArray.Length - 1; j >= 0; j--)
                {
                    string subStr = contentArray[j];
                    string newSubStr = subStr.Replace(":", "");
                    float temp;
                    if (float.TryParse(newSubStr, out temp))
                    {
                        string[] time = subStr.Split(':');
                        float min;
                        float.TryParse(time[0], out min);
                        float sec;
                        float.TryParse(time[1], out sec);
                        subStr = string.Format("{0}", (sec + 60 * min));
                    }
                    float num = 0f;
                    if (float.TryParse(subStr, out num))
                    {
                        timeA.Add(num);
                        if (float.TryParse(contentArray[contentArray.Length - 1], out num))
                        {
                            lyricArray.Add("");
                        }
                        else
                        {
                            lyricArray.Add(contentArray[contentArray.Length - 1]);
                        }
                    }
                }
            }
        }
        return lyricArray;
    }
    public List<string> SortLyricListAndTimeList(List<string> lyricA, List<float> timeA, out List<float> timeArray)
    {
        for (int i = 0; i < timeA.Count - 1; i++)
        {
            for (int j = 0; j < timeA.Count - 1 - i; j++)
            {
                if (timeA[j] > timeA[j + 1])
                {
                    float temp = timeA[j];
                    timeA[j] = timeA[j + 1];
                    timeA[j + 1] = temp;

                    string tempLyric = lyricA[j];
                    lyricA[j] = lyricA[j + 1];
                    lyricA[j + 1] = tempLyric;
                }
            }
        }
        timeArray = timeA;
        return lyricA;
    }

    //读取lrc歌词文件
    public void GetLrcFile(string file)
    {
        StreamReader sr = new StreamReader(file, Encoding.UTF8);
        string str = sr.ReadToEnd();
        List<float> a = new List<float>();  //取得了时间点 
        List<string> b = new List<string>();  //多少行标题
        List<float> c = new List<float>();
        List<string> d = new List<string>();//歌词
        List<string> e = new List<string>();
        d = GetLyricListAndTimeList(str, out a, out b);  //输出了时间点和 歌词list
        e = SortLyricListAndTimeList(d, a, out c);//得到了每行歌词  和时间点
                                                  //这里很乱，我是先达到具体目的，优化以后再考虑
        StartCoroutine(ShowLrc(e, c));
    }
    IEnumerator ShowLrc(List<string> lrc, List<float> t)
    {
        lrcText.text = lrc[0];
        yield return new WaitForSeconds(t[0]);
        for (int i = 1; i < lrc.Count; i++)
        {
            float ts = t[i] - t[i - 1] - offest;  //偏移量
            yield return new WaitForSeconds(ts);
            lrcText.text = lrc[i];
        }
    }

}
