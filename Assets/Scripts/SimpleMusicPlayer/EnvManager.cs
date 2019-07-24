using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class EnvManager : Singleton<EnvManager> {

    public Action<MainColorTintData> on_maincolor_tint_changed;

    GameObject env_root;
    GameObject tube_inside;
    GameObject tube_outside;
    GameObject g_tube;
    GameObject qua;
    GameObject env_objct;
    GameObject global_effect_anchor;
    Light light;

    List<string> tube_images;

    Color light_start_color;

    public string tube_image_name;

    MainColorTintData main_color_tint;
    public MainColorTintData Main_Color_Tint
    {
        get { return main_color_tint; }
    }

    public Transform Env_Object_Root
    {
        get { return env_objct.transform; }
    }

    public Transform Global_env_object_root
    {
        get { return global_effect_anchor.transform; }
    }

    public bool EnableTubeRotate
    {
        set {
            tube_inside.GetComponentInChildren<AutoRotate>().enabled = value;
            tube_outside.GetComponentInChildren<AutoRotate>().enabled = value;
        }
    }

    public override void Init()
    {
        base.Init();

        env_root = GameObject.Find("Enviroment");
        tube_inside = env_root.transform.Find("tube_inside").gameObject;
        tube_outside = env_root.transform.Find("tube_outside").gameObject;
        g_tube = env_root.transform.Find("g_tube").gameObject;
        qua = env_root.transform.Find("qua").gameObject;
        env_objct = env_root.transform.Find("env_objct").gameObject;
        global_effect_anchor = env_root.transform.Find("global_effect_anchor").gameObject;
        light = env_root.transform.Find("point light").GetComponent<Light>();
        light_start_color = light.color;

        EnableTubeRotate = DataManager.Instance.Data_Save.isTubeAutoRotate;

        tube_images = DataManager.Instance.Style_Data.tube_images;

        //根据存档初始化环境
        tube_image_name = DataManager.Instance.Data_Save.style.tube_image_name;
        if (tube_image_name != "no")
        {
            ChageTubeTexture(tube_image_name);
        }
    }

    public void ResetEnvTrans()
    {
        Vector3 player_postion = GameObject.Find("PlayerController").transform.position;
        env_root.transform.position = new Vector3(player_postion.x, env_root.transform.position.y, player_postion.z);
    }

    public void OnMusicLoad(string fullpath)
    {
        DataManager.Instance.LoadAudioTexture(fullpath, (tex) => {
            Material tube_inside_mat = tube_inside.GetComponent<MeshRenderer>().material;
            tube_inside_mat.mainTexture = tex;

            main_color_tint = MainColorTint.ComputeMainColor(tex);
            tube_inside_mat.color = main_color_tint.colortint ;
            light.color = light_start_color * 0.5f + main_color_tint.colortint * 0.5f;
            if (on_maincolor_tint_changed != null) on_maincolor_tint_changed(main_color_tint);
            float tile_x = GetTubeTiling(tex.width, tex.height);
            Vector4 v4 = tube_inside_mat.GetVector("_MainTex_ST");
            tube_inside_mat.SetVector("_MainTex_ST", new Vector4(tile_x, v4.y, v4.z, v4.w));

            qua.GetComponent<MeshRenderer>().material.mainTexture = tex;
        });

    }

    private float GetTubeTiling(int tex_width, int tex_height)
    {
        float radio = tex_height / (float)tex_width;
        return -8.37f * radio;
    }

    public void ClearSave()
    {
        DataManager.Instance.Data_Save.style = new EnvStyle();
        DataManager.Instance.SaveData();
    }

    public void ChangeTubeOutSideTexture(Texture tex)
    {
        Material tube_out_mat = tube_outside.GetComponentInChildren<MeshRenderer>().material;
        tube_out_mat.mainTexture = tex;

        float tile_x = GetTubeTiling(tex.width, tex.height);
        Vector4 v4 = tube_out_mat.GetVector("_MainTex_ST");
        tube_out_mat.SetVector("_MainTex_ST", new Vector4(tile_x, v4.y, v4.z, v4.w));

    }

    public void ChangeTubeTexture(int tube_img_index)
    {
        string filename = tube_images[tube_img_index];
        ChageTubeTexture( filename);
    }

    private void ChageTubeTexture(string filename)
    {
        if (filename.EndsWith(".jpg") || filename.EndsWith(".png"))
        {
            MusicPlayerManager.Instance.LoadTextureWWW(filename, (tex, err) =>
            {
                if (string.IsNullOrEmpty(err))
                {
                    EnvManager.Instance.ChangeTubeOutSideTexture(tex);
                }
            });
        }
        else if (filename.EndsWith(".mp4") || filename.EndsWith(".mov"))
        {
            PlayerManager.Instance.PlayVideo(filename, UnityEngine.Video.VideoSource.Url);
            EnvManager.Instance.ChangeTubeOutSideTexture(PlayerManager.Instance._RenderTexture);
        }
    }

    public override void UnInit()
    {
        base.UnInit();
    }
}
