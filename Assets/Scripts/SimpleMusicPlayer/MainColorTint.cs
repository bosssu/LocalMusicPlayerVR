using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainColorTintData
{
    public Color colortint;
    public float ilu;
}

/// <summary>
/// 贴图主色调及明度计算工具
/// Created by 杜子兮 2016.1.23
/// duzixi.com All Rights Reserved
/// </summary>
public class MainColorTint 
{
    // 计算主色调
    public static MainColorTintData ComputeMainColor(Texture2D img)
    {
        float r = 0;
        float g = 0;
        float b = 0;

        int width = img.width;
        int height = img.height;
        Color[] colors = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                colors[i * height + j] = img.GetPixel(i, j);
                r += colors[i * height + j].r;
                g += colors[i * height + j].g;
                b += colors[i * height + j].b;
            }
        }

        r /= width * height;
        g /= width * height;
        b /= width * height;

        // 计算明度
        float v = Mathf.Max(Mathf.Max(r, g), b);

        MainColorTintData dat = new MainColorTintData();
        dat.colortint = new Color(r, g, b);
        dat.ilu = v;

        return dat;
    }
}