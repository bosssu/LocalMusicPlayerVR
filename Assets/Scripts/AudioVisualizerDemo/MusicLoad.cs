using System.Collections;
using System.IO;
//using NAudio.Wave;
using UnityEngine;
public class MusicLoad : MonoBehaviour
{
    protected void OnGUI()
    {
        if (GUILayout.Button("www"))
        {
            StartCoroutine(LoadMusic(@"D:\EXTERNAL_ASSETS\AUDIO\coop.mp3"));
        }
    }
    //public AudioSource Source; private IEnumerator LoadMusic(string filepath, string savepath)
    //{
    //    //var stream = File.Open(filepath, FileMode.Open);
    //    //var reader = new Mp3FileReader(stream);
    //    //WaveFileWriter.CreateWaveFile(savepath, reader);
    //    var www = new WWW("file://" + savepath);
    //    yield return www;
    //    var clip = www.audioClip;
    //    Source.clip = clip;
    //    Source.Play();
    //}

    public AudioSource Source;
    private IEnumerator LoadMusic(string filepath)
    {
        //var stream = File.Open(filepath, FileMode.Open);
        //var reader = new Mp3FileReader(stream);
        //WaveFileWriter.CreateWaveFile(savepath, reader);
        var www = new WWW("file://" + filepath);
        yield return www;
        var clip = www.GetAudioClip();
        Source.clip = clip;
        Source.Play();
    }
}