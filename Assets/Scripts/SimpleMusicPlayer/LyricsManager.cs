using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LyricsManager : Singleton<LyricsManager> {

    Coroutine last_task;

    float offest = 0f;

    List<float> c;
    List<string> e;

    public bool is_task_on; //是否有歌词任务在进行

    public Action<string> lyric_line_update_event;

    public override void Init()
    {
        base.Init();
    }

    public void StartLyric(string name,float time)
    {

        is_task_on = false;

        string path = string.Format("{0}/{1}.lrc", DataManager.song_dir, name);
        Debug.Log(path);
        if (File.Exists(path))
        {
            GetLrcFile(path);
            LrcTimeSet(time);
        }
        else
        {
            Debug.LogError("lrc 歌词文件不存在" + path);
            if (lyric_line_update_event != null) lyric_line_update_event("LRC lyrics file does not exist");
        }
            
        
    }

    //读取lrc歌词文件
    public void GetLrcFile(string file)
    {
        try
        {
            if (!File.Exists(file)) return;

            StreamReader sr = new StreamReader(file, Encoding.UTF8);
            string str = sr.ReadToEnd();

            List<float> a = new List<float>();  //取得了时间点 
            List<string> b = new List<string>();  //多少行标题
            c = new List<float>();
            List<string> d = new List<string>();//歌词
            e = new List<string>();
            d = GetLyricListAndTimeList(str, out a, out b);  //输出了时间点和 歌词list
            e = SortLyricListAndTimeList(d, a, out c);//得到了每行歌词  和时间点
                                                      //这里很乱，我是先达到具体目的，优化以后再考虑

            is_task_on = true;

        }
        catch(Exception e) {
            Debug.LogError(e.Message);
            return;
        }

    }

    public void LrcTimeSet(float time)
    {
        if (e != null && e.Count > 0 && c != null && c.Count > 0)
        {
            if (last_task != null) MusicPlayerManager.Instance.StopCoroutine(last_task);
            last_task = MusicPlayerManager.Instance.StartCoroutine(ShowLrc(e, c, time));
        }
    }

    IEnumerator ShowLrc(List<string> lrc, List<float> t,float time)
    {
        if (is_task_on)
        {
            int begin_index = 0;
            for (int i = 1; i < t.Count; i++)
            {
                if (time >= t[i - 1] && time < t[i])
                {
                    begin_index = i;
                    break;
                }
            }

            if (lyric_line_update_event != null) lyric_line_update_event(lrc[begin_index]);

            //Debug.LogError(string.Format("time:{0} indextime:{1}", time, t[begin_index]));
            yield return new WaitForSeconds(t[begin_index] - time);
            for (int i = begin_index + 1; i < lrc.Count; i++)
            {
                //Debug.LogError(string.Format("time:{0} indextime:{1}", time, t[i]));
                float ts = t[i] - t[i - 1] - offest;  //偏移量
                yield return new WaitForSeconds(ts);
                if (lyric_line_update_event != null) lyric_line_update_event(lrc[i]);
            }

            is_task_on = false;
        }
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

    public override void UnInit()
    {
        base.UnInit();
    }

}
