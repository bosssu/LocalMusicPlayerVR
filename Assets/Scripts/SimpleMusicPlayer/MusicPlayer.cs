using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AudioFileInfo
{
    public string album;
    public string author;
    public string title;
}

public class MusicPlayer : MonoSingleton<MusicPlayer> {

    public const int samples_count = 128;

    public enum PlayMode
    {
        AUTO_NEXT,  //自动下一曲
        SINGLE_LOOP,//单曲循环
        ENTIRE_LOOP//列表循环
    }

    public System.Action<float[],float> on_samples_update_event;
    public System.Action<AudioFileInfoX> on_song_play_event;

    List<AudioFileInfoX> audio_files;

    float[] samples = new float[samples_count];
    int current_index;
    float progress;
    //Stack<int> record;
    List<int> random_play_list;

    bool isPlaying;

    float samples_sum;
    public float SamplesSum
    {
        get { return this.samples_sum; }
    }

    bool _is_random_play;
    public bool IsRandomPlay
    {
        get {
            return _is_random_play;
        }
        set {
            _is_random_play = value;
        }
    }

    PlayMode _playmode;
    public PlayMode _PlayMode
    {
        get {
            return this._playmode;
        }
        set {
            if (value == PlayMode.SINGLE_LOOP)
            {
               
            }
            else
            {

            }
            this._playmode = value;
        }
    }

    public float PlayProgress
    {
        get {
            if(this.audioSource.clip != null)
                return this.audioSource.time / this.audioSource.clip.length;

            return 0;
        }
        set {
            if (this.audioSource.clip != null)
            {
                this.audioSource.time = value * this.audioSource.clip.length;
                LyricsManager.Instance.LrcTimeSet(this.audioSource.time);
            }
             
        }
    }

    private AudioSource audioSource;
    public AudioSource AudioSouce
    {
        get
        {
            return this.audioSource;
        }
        set {
            audioSource = value;
        }
    }

    AudioFileInfoX _current_audio_file_info;
    public AudioFileInfoX CurrentAudioFileInfo
    {
        get {
            return this._current_audio_file_info;
        }
    }

    protected override void Init()
    {
        base.Init();
        //record = new Stack<int>();
        random_play_list = new List<int>();
        audioSource = Utils.GetSafeComponet<AudioSource>(gameObject);
        audioSource.loop = false;
        audio_files = DataManager.Instance.AllAudioFileInfomation;
    }

    void Update()
    {
        if (isPlaying)
        {
            if (PlayProgress >= 0.99f)
            {
                audioSource.Stop();
                audioSource.time = 0;
                isPlaying = false;

                switch (_playmode)
                {
                    case PlayMode.AUTO_NEXT:
                        NextSong();
                        break;
                    case PlayMode.SINGLE_LOOP:
                        PlayMusic(_current_audio_file_info);
                        break;
                    default:
                        break;
                }
            }

            audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
            samples_sum = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= MusicPlayerManager.Instance.Amp_Mulpter;
                samples_sum += samples[i];
            }
            if (on_samples_update_event != null) on_samples_update_event(this.samples,samples_sum);
        }

        if (Input.GetKeyDown(KeyCode.X))
            audioSource.time = audioSource.clip.length - 5;
    }

    public bool PlayPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            return false;
        }
        else
        {
            audioSource.UnPause();
            return true;
        }
    }

    public void NextSong()
    {
        if (audio_files.Count == 0) return;

        if (_is_random_play)
        {
            current_index = RandomIndex();
            if (current_index != -1)
            {
                MusicPlayerManager.Instance.StartCoroutine(LoadAndPlayLocal(audio_files[current_index]));
            }
        }
        else
        {
            current_index++;
            if (current_index >= audio_files.Count) current_index = 0;
            MusicPlayerManager.Instance.StartCoroutine(LoadAndPlayLocal(audio_files[current_index]));
        }

        //record.Push(current_index);
    }

    public void LastSong()
    {
        if (audio_files.Count == 0) return;

        if (_is_random_play)
        {
            current_index = RandomIndex();
            if (current_index != -1)
            {
                MusicPlayerManager.Instance.StartCoroutine(LoadAndPlayLocal(audio_files[current_index]));
            }
        }
        else
        {
            current_index--;
            if (current_index < 0) current_index = audio_files.Count - 1;
            MusicPlayerManager.Instance.StartCoroutine(LoadAndPlayLocal(audio_files[current_index]));
        }

        //if (record.Count > 0)
        //{
        //    current_index = record.Pop();
        //    MusicPlayerManager.Instance.StartCoroutine(LoadAndPlayLocal(audio_files[current_index]));
        //}
        //else
        //{
        //}


    }

    public void PlayMusic(AudioFileInfoX info)
    {
        for (int i = 0; i < audio_files.Count; i++)
        {
            if (audio_files[i] == info)
            {
                current_index = i;
                //record.Push(current_index);
                MusicPlayerManager.Instance.StartCoroutine(LoadAndPlayLocal(info));
            }
        }
    }

    int ran_index;
    private int RandomIndex()
    {
        if (ran_index >= random_play_list.Count)
        {
            random_play_list = Utils.GetRandomNoSame(0, audio_files.Count, audio_files.Count);
            ran_index = 0;
        }
        ran_index++;
        return random_play_list[ran_index - 1];
    }

    //private IEnumerator LoadAndPlayOnline(string url)
    //{
    //    if (audioSource.isPlaying) audioSource.Stop();
    //    this.PlayProgress = 0;

    //    WWW www = new WWW(url);
    //    while (!www.isDone)
    //    {
    //        this.progress = www.progress;
    //    }
    //    yield return www;
    //    if (string.IsNullOrEmpty(www.error))
    //    {
    //        var audio = www.GetAudioClip(false,true);
    //        audioSource.clip = audio;
    //        audioSource.Play();
    //        isPlaying = true;
    //        if (on_song_play_event != null) on_song_play_event(audio_files[current_index].path);
    //        _current_audio_file_info = DataManager.Instance.Dic_AudioInfo[audio_files[current_index].path];
    //        Resources.UnloadUnusedAssets();
    //    }
    //}

    private void StartPlayLyric(string mp3path,float time)
    {
        string lrcname = Path.GetFileNameWithoutExtension(mp3path);
        LyricsManager.Instance.StartLyric(lrcname,time);
    }

    private IEnumerator LoadAndPlayLocal(AudioFileInfoX info)
    {
        if (audioSource.isPlaying) audioSource.Stop();
        this.PlayProgress = 0;

        string url = string.Format("file:///{0}", info.path);
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            this.progress = www.progress;
        }
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            var audio = www.GetAudioClip(false, true);
            audioSource.clip = audio;
            audioSource.Play();       
            isPlaying = true;
            if (on_song_play_event != null) on_song_play_event(info);
            StartPlayLyric(info.path, 0);
            _current_audio_file_info = info;
            Resources.UnloadUnusedAssets();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

}
