using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchTest : MonoBehaviour {

    const string url_base = "https://music.taihe.com/song/";

    // Use this for initialization
    void Start () {
        BaiduMusicAPI.QuerySong("少司命", (c) => {
            StartCoroutine(LoadAndPlay(c.song[0].songid));
        });
	}

    IEnumerator LoadAndPlay(string id)
    {
        string url = string.Format("{0}{1}", url_base, id);
        WWW ww = new WWW(url);
        yield return ww;

        GetComponent<AudioSource>().clip = ww.GetAudioClip(false, true);
        GetComponent<AudioSource>().Play();
    }

	// Update is called once per frame
	void Update () {
		
            
	}
}
