using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SimperManager : MonoBehaviour {

    public Transform player;

    public float amp_mulpter = 0.2f;
    public float radius = 2;
    public Gradient cube_color;
    public GameObject cube_prefab;

#if UNITY_EDITOR
    const string song_dir = "D:/EXTERNAL_ASSETS/AUDIO";
#else
        const string song_dir = "/storage/emulated/0/Music/";
#endif

    string[] audio_files;

    float[] samples = new float[128];

    RectTransform lable_Rt;
    Text lable_text;
    Text progress_label;
    int index;
    AudioSource audioSource;
    float progress;
    List<Transform> objects;

    bool is_show_menu;

	void Start () {
        audioSource = GetComponent<AudioSource>();

        if (Directory.Exists(song_dir))
        {
            audio_files = Directory.GetFiles(song_dir);
        }

        InitUI();
        InitObject();


    }

    void InitObject()
    {
        objects = new List<Transform>();
        for (int i = 0; i < samples.Length; i++)
        {
            float base_angle = 360f / samples.Length;
            Quaternion rot = Quaternion.AngleAxis(base_angle * i, Vector3.up);
            Vector3 dir = rot * Vector3.right;
            Vector3 center_pos = new Vector3(player.position.x,0, player.position.z);
            Vector3 pos = center_pos + dir * radius;

            GameObject g = Instantiate<GameObject>(cube_prefab, transform);
            g.transform.position = pos;
            g.transform.LookAt(new Vector3(player.position.x, g.transform.position.y, player.position.z));
            objects.Add(g.transform);
        }
    }

    void InitUI()
    {
        lable_Rt = DebugUIBuilder.instance.AddLabel("");
        lable_Rt.sizeDelta = new Vector2(400, 200);
        lable_text = lable_Rt.GetComponent<Text>();

        DebugUIBuilder.instance.AddButton("下一首", NextSong);
        DebugUIBuilder.instance.AddButton("上一首", LastSong);

        progress_label = DebugUIBuilder.instance.AddLabel("").GetComponent<Text>();

        DebugUIBuilder.instance.Show();
        is_show_menu = true;
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            LastSong();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextSong();

        audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

        //Debug.Log(samples[0]);
        for (int i = 0; i < samples.Length; i++)
        {
            Transform trans = objects[i];
            trans.localScale = new Vector3(trans.localScale.x, samples[i] * amp_mulpter *(50 + i * i * 0.5f), trans.localScale.z);
            trans.GetComponentInChildren<MeshRenderer>().material.color = cube_color.Evaluate(samples[i] * 500);
        }

        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            is_show_menu = !is_show_menu;
            if (is_show_menu)
                DebugUIBuilder.instance.Show();
            else
                DebugUIBuilder.instance.Hide();
        }
    }

    private void NextSong()
    {
        if (index >= audio_files.Length) index = 0;
        lable_text.text = string.Format("歌曲：{0}", Path.GetFileNameWithoutExtension(audio_files[index]));
        StartCoroutine(LoadAndPlay(audio_files[index]));
        index++;
    }

    private void LastSong()
    {
        if (index < 0) index = audio_files.Length - 1;
        lable_text.text = string.Format("歌曲：{0}", Path.GetFileNameWithoutExtension(audio_files[index]));
        StartCoroutine(LoadAndPlay(audio_files[index]));
        index--;
    }

    private IEnumerator LoadAndPlay(string path)
    {
        if (audioSource.isPlaying) audioSource.Stop();
        this.progress = 0;

        string url = string.Format("file:///{0}", path);
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            this.progress = www.progress;
            progress_label.text = this.progress.ToString();
        }
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            var audio = www.GetAudioClip();
            audioSource.clip = audio;
            audioSource.Play();
        }
    }
}
