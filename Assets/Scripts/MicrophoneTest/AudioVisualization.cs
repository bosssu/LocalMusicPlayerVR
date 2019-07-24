using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
{
    public static float volume;
    private AudioClip micRecord;
    private Vector3 start_pos;
    string device;
    /// <summary>
    /// 拖尾的移动速度   要和摄像机的移动速度一致
    /// </summary>
    private int speed;
    private float x;
    void Start()
    {
        //初始化速度的值
        speed = 5;
        device = Microphone.devices[0];//获取设备麦克风
        micRecord = Microphone.Start(device, true, 999, 44100);//44100音频采样率   固定格式
        start_pos = transform.position;
    }
    void Update()
    {
        volume = GetMaxVolume();
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        //要加粒子特效  产生拖尾
        //transform.Translate(Vector3.right * speed * Time.deltaTime);
        transform.position = new Vector3(start_pos.x + Mathf.Sin(Time.time * Mathf.PI * 2), transform.position.y, transform.position.z);
        x = gameObject.transform.position.x;
        //print(volume);
        //处理峰值
        if (volume > 0.9f)
        {
            volume = volume * speed * Time.deltaTime;
            gameObject.transform.position = new Vector3(x, start_pos.y + volume * 10, transform.position.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(x, start_pos.y + volume * 10, transform.position.z);
        }
    }
    //每一振处理那一帧接收的音频文件
    float GetMaxVolume()
    {
        float maxVolume = 0f;
        //剪切音频
        float[] volumeData = new float[128];
        int offset = Microphone.GetPosition(device) - 128 + 1;
        if (offset < 0)
        {
            return 0;
        }
        micRecord.GetData(volumeData, offset);

        for (int i = 0; i < 128; i++)
        {
            float tempMax = volumeData[i];//修改音量的敏感值
            if (maxVolume < tempMax)
            {
                maxVolume = tempMax;
            }
        }
        return maxVolume;
    }
}
