using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EnvStyle
{
    public string tube_image_name;
}

[System.Serializable]
public class EnvObject
{
    public EnviromentInteractiveObjectInfo info;
    public Vector3 position;
    public Vector3 local_scale;
    public Quaternion rotation;
}

[System.Serializable]
public class MusicPlaySaveData
{
    public float volume = 1;
    public int effect_index = 1;
    public int handeffect_index = 0;
    public EnvStyle style= new EnvStyle {  tube_image_name = "no"};
    public MusicPlayer.PlayMode playmode = MusicPlayer.PlayMode.AUTO_NEXT;
    public float amp_mulpter = 1;
    public bool isRandom = true;
    public bool isShowLyric = false;
    public bool isTubeAutoRotate = false;
    public List<EnvObject> env_objects = new List<EnvObject>();
}

public class StyleData
{
    public List<string> tube_images;
}

public class AudioFileInfoX : IComparable<AudioFileInfoX>
{
    public string path;
    public AudioFileInfo info;

    public int CompareTo(AudioFileInfoX other)
    {
        return this.info.title.CompareTo(other.info.title);
    }
}

public class DataManager : Singleton<DataManager> {

    const string key_player_data_save = "key_player_data_save";

#if UNITY_EDITOR
    public const string song_dir = "D:/EXTERNAL_ASSETS/AUDIO";
    public const string otherdata_base_dir = "D:/EXTERNAL_ASSETS/SimplerMusicPlayer";
#else
        public const string song_dir = "/storage/emulated/0/Music/";
        public const string otherdata_base_dir = "/storage/emulated/0/SimplerMusicPlayer";
#endif

    Texture2D loadedtexture;
    string last_loadtexture_path;

    Texture2D default_song_texture;

    MusicPlaySaveData data_save = new MusicPlaySaveData();
    public MusicPlaySaveData Data_Save
    {
        get {
            return this.data_save;
        }
    }

    StyleData styledata;
    public StyleData Style_Data
    {
        get { return this.styledata; }
    }

    List<string> audio_files;

    List<AudioFileInfoX> _all_audiofile_infomation;
    public List<AudioFileInfoX> AllAudioFileInfomation
    {
        get {
            return this._all_audiofile_infomation;
        }
    }

    Dictionary<string, AudioFileInfoX> dic_audioinfo;
    public Dictionary<string, AudioFileInfoX> Dic_AudioInfo
    {
        get { return dic_audioinfo; }
    }

    public override void Init()
    {
        base.Init();

        InitDirStruction();

        LoadData();

        default_song_texture = Resources.Load<Texture2D>("simple_music_player/texture/default_logo");

        dic_audioinfo = new Dictionary<string, AudioFileInfoX>();
        _all_audiofile_infomation = new List<AudioFileInfoX>();
        if (Directory.Exists(song_dir))
        {
            audio_files = new List<string>();
            string[] allfiles = Directory.GetFiles(song_dir);
            if (allfiles.Length > 0)
            {
                foreach (var p in allfiles)
                {
                    if (p.EndsWith(".mp3", System.StringComparison.OrdinalIgnoreCase))
                        audio_files.Add(p);
                }
            }
        }

        foreach (var filepath in audio_files)
        {
            AudioFileInfo t_info = LoadAudioConfigFile(filepath);
            if (t_info != null)
            {
                AudioFileInfoX x_info = new AudioFileInfoX() { info = t_info, path = filepath };
                if (!dic_audioinfo.ContainsKey(x_info.info.title))
                {
                    dic_audioinfo.Add(x_info.info.title, x_info);
                }

                _all_audiofile_infomation.Add(x_info);

            }
        }

        //排序
        _all_audiofile_infomation.Sort();

    }


    //创建目录结构
    private void InitDirStruction()
    {
        string tube_img_dir = otherdata_base_dir + "/styles/textures/tube";

        Utils.CreateDirIfNotExist(otherdata_base_dir);

        this.styledata = new StyleData();
        if (Directory.Exists(tube_img_dir))
        {
            string[] files = Directory.GetFiles(tube_img_dir);
            if (files.Length > 0)
            {
                styledata.tube_images = new List<string>();
                foreach (var item in files)
                {
                    styledata.tube_images.Add(item);
                }
            }
        }
        else
        {
            Utils.CreateDirIfNotExist(otherdata_base_dir + "/styles/textures/tube");
        }

    }

    public void LoadAudioTexture(string audiofile_path,System.Action<Texture2D> callback)
    {
        string temp = Path.GetFileNameWithoutExtension(audiofile_path);
        string temp1 = Path.GetDirectoryName(audiofile_path);
        string song_img_path = string.Format("{0}/{1}.jpg", temp1, temp);
        Debug.Log(song_img_path);

        if (last_loadtexture_path != song_img_path)
        {
            MusicPlayerManager.Instance.LoadTextureWWW(song_img_path, (tex,err) =>
            {
                if (string.IsNullOrEmpty(err))
                {
                    this.loadedtexture = tex;
                    if (callback != null) callback(this.loadedtexture);
                    last_loadtexture_path = song_img_path;
                }
                else
                {
                    Debug.LogError(err);
                    if (callback != null) callback(this.default_song_texture);
                }
            });
        }
        else
        {
            if (this.loadedtexture != null)
            {
                if (callback != null) callback(this.loadedtexture);
            }
            else
            {
                if (callback != null) callback(this.default_song_texture);
            }
        }  

    }

    private AudioFileInfo LoadAudioConfigFile(string filepath)
    {
        string filename_noextension = Path.GetFileNameWithoutExtension(filepath);
        string config_file_path = string.Format("{0}/{1}.json", DataManager.song_dir, filename_noextension);
        //Debug.Log(config_file_path);
        if (File.Exists(config_file_path))
        {
            string str = File.ReadAllText(config_file_path);
            return JsonUtility.FromJson<AudioFileInfo>(str);
        }
        else
        {
            Debug.Log("json文件不存在");
        }

        return null;
    }

    #region 数据管理

    public void ClearData()
    {
        PlayerPrefs.DeleteKey(key_player_data_save);
    }

    public void SaveData()
    {
        string js = JsonUtility.ToJson(data_save);
        PlayerPrefs.SetString(key_player_data_save, js);
        //Debug.Log("save:" + js);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey(key_player_data_save))
        {
            string js = PlayerPrefs.GetString(key_player_data_save);
            this.data_save = JsonUtility.FromJson<MusicPlaySaveData>(js);
            //Debug.Log("load:" + js);
        }
    }

    #endregion

    public override void UnInit()
    {
        base.UnInit();
    }
}
