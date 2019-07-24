using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoggerManager : Singleton<LoggerManager> {

    RectTransform rt;
    Text text;

    public override void Init()
    {
        base.Init();

        Application.logMessageReceived += MyLogCallback;

        rt = DebugUIBuilder.instance.AddLabel("");
        DebugUIBuilder.instance.AddButton("clear", () => {
            text.text = "";
        });

        rt.sizeDelta = new Vector2(1000,2000);
        text = rt.GetComponent<Text>();
        text.fontSize = 14;
        text.alignment = TextAnchor.UpperLeft;

        DebugUIBuilder.instance.Show();

        //LogError("zhangsan");
        //Log("zhangsan");
        //Debug.LogError("fdfdfdfd");
        //Debug.Log("fxxxxxxxx");
    }

    public Vector2 Size
    {
        set {
            this.rt.sizeDelta = value;
        }
    }

    public void Log(string msg)
    {
        string str = string.Format("\n{0}", msg);
        LL(str);
    }

    public void LogError(string mag)
    {
        string msg = string.Format("\n<color=#f00>{0}</color>",mag);
        LL(msg);
    }

    private void LL(string str)
    {
        this.text.text += str;
        if (text.text.Length > 1000)
            text.text = "";
    }

    void MyLogCallback(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Assert:
                //message += "      receive an assert log" + ",condition=" + condition + ",stackTrace=" + stackTrace;
                break;
            case LogType.Error:
                LogError(string.Format("condition:{0}", condition));
                break;
            case LogType.Exception:
                //message += "      receive an Exception log" + ",condition=" + condition + ",stackTrace=" + stackTrace;
                break;
            case LogType.Log:
                Log(string.Format("condition:{0}", condition));
                break;
            case LogType.Warning:
                //message += "      receive an Warning log" + ",condition=" + condition + ",stackTrace=" + stackTrace;
                break;
        }

    }

    public override void UnInit()
    {
        base.UnInit();
        Application.logMessageReceived -= MyLogCallback;
    }

}
