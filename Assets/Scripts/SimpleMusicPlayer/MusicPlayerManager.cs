using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicPlayerManager : MonoSingleton<MusicPlayerManager> {

    float amp_mulpter = 0.2f;

    SpawnPool _pool;
    public SpawnPool SpawnPool
    {
        get {
            if (_pool == null)
                _pool = gameObject.AddComponent<SpawnPool>();
            return _pool;
        }
    }

    public float Amp_Mulpter
    {
        set { amp_mulpter = value; }
        get { return amp_mulpter; }
    }

    protected override void Init()
    {
        base.Init();

        //LocalizationManager.Instance.CurrentLocalization = "Zh-CN";

        MusicPlayer.Instance.IsRandomPlay = false;
        MusicPlayer.Instance._PlayMode = MusicPlayer.PlayMode.AUTO_NEXT;

        MusicPlayer.Instance.on_song_play_event += (audio_info) => {
            EnvManager.Instance.OnMusicLoad(audio_info.path);
        };
        MusicPlayer.Instance.on_song_play_event += (audio_info) => {
            UIManager.Instance.OnSongPlay(audio_info);
        };

        EffectManager.CreateInstance();

        PlayerInputManager.GetInstance();

        StartCoroutine(DelayInit());

    }

    IEnumerator DelayInit()
    {
        yield return new WaitForSeconds(.5f);
        HandEffectManager.CreateInstance();
    }

    private void Update()
    {
        UIManager.Instance.Update();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MusicPlayer.Instance.LastSong();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MusicPlayer.Instance.NextSong();
        if (Input.GetKeyDown(KeyCode.A))
            EffectManager.Instance.NextEffect();
    }

    #region Util Func

    public void LoadTextureWWW(string path, Action<Texture2D,string> callback)
    {
        StartCoroutine(LoadTexture(path, callback));
    }

    private IEnumerator LoadTexture(string path,Action<Texture2D,string> callback )
    {
        string url = "file://" + path;
        WWW www = new WWW(url);
        yield return www;
        if (callback != null) callback(www.texture, www.error);
    }

    #endregion

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
