using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerManager : MonoSingleton<PlayerManager> {

    const string video_basepath = "shuimo/video";

    VideoPlayer player;

    public RenderTexture _RenderTexture
    {
        get {
            return this.player.targetTexture;
        }
    }

    protected override void Init()
    {
        base.Init();
        player = gameObject.AddComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.waitForFirstFrame = true;
        player.isLooping = true;
        player.source = VideoSource.VideoClip;
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = new RenderTexture(1280, 720, 24);
        player.targetTexture.wrapMode = TextureWrapMode.Repeat;
    }

    public void PlayVideo(string name,VideoSource videosource)
    {
        Stop();
        player.targetTexture.Release();

        this.player.source = videosource;

        if (videosource == VideoSource.VideoClip)
        {
            string path = video_basepath + "/" + name;
            VideoClip clip = Resources.Load<VideoClip>(path);
            if (clip == null) return;

            Debug.Log("videopath: " + path);
            player.clip = clip;
        }
        else if (videosource == VideoSource.Url)
        {
            string url = name;
            //Debug.Log("video url" + url);
            player.url = url;
        }

        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        //准备（需要等准备完成才能播放)
        Debug.Log("开始准备");
        player.Prepare();

        while (!player.isPrepared)
        {
            yield return null;
        }


        //视频已经准备好
        Debug.Log("准备好了");
        player.Play();

    }

    public void PauseVideo()
    {
        if(player.isPlaying)
            player.Pause();
    }

    public void Stop()
    {
        if (player.isPlaying)
        {
            player.frame = 0;
            player.Stop();
        }

    }

}
