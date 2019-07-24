using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayWin : WindowBase {

     OVRInputModule ovr_inputmodule;

    Button btn_last_song;
    Button btn_nex_song;
    Button btn_playpause;
    Image progressbar;
    Button btn_next_effect;
    Toggle toggle_lyric;
    Text text_lyric;

    public override void Init()
    {
        base.Init();

        win_root = UIManager.Instance.UIMain.transform.Find("PlayPanel");

        btn_last_song = win_root.Find("btn_last").GetComponent<Button>();
        btn_nex_song = win_root.Find("btn_next").GetComponent<Button>();
        btn_playpause = win_root.Find("btn_play").GetComponent<Button>();
        btn_next_effect = win_root.Find("btn_next_effect").GetComponent<Button>();
        progressbar = win_root.Find("progressbar").GetComponent<Image>();
        toggle_lyric = win_root.Find("toggle_lyric").GetComponent<Toggle>();
        text_lyric = win_root.Find("text_lyric").GetComponent<Text>();


        ovr_inputmodule = GameObject.FindObjectOfType<OVRInputModule>();

        btn_nex_song.onClick.AddListener(() => {
            MusicPlayer.Instance.NextSong();
        });
        btn_last_song.onClick.AddListener(() => {
            MusicPlayer.Instance.LastSong();
        });
        btn_playpause.onClick.AddListener(() => {
            bool isplay = MusicPlayer.Instance.PlayPause();
            btn_playpause.GetComponentInChildren<Text>().text = isplay ? "暂停" : "播放";
        });
        btn_next_effect.onClick.AddListener(()=> {
            EffectManager.Instance.NextEffect();
        });
        toggle_lyric.onValueChanged.AddListener((v) => {
            text_lyric.gameObject.SetActive(v);
            DataManager.Instance.Data_Save.isShowLyric = v;
            DataManager.Instance.SaveData();
        });

        LyricsManager.Instance.lyric_line_update_event = (str) => {
            text_lyric.text = str;
        };

        //init ui
        toggle_lyric.isOn = DataManager.Instance.Data_Save.isShowLyric;
    }

    public override void Update()
    {
        base.Update();

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.F))
        {
            Transform rayTransform = ovr_inputmodule.rayTransform;
            if (rayTransform != null)
            {
                Ray ray = new Ray(rayTransform.position, ovr_inputmodule.rayTransform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100) && hit.collider.name == "progress_collider")
                {
                    Vector3 local_hit_pos = hit.collider.transform.InverseTransformPoint(hit.point);
                    //Debug.Log(local_hit_pos);
                    float x = local_hit_pos.x + 0.5f;
                    x = Mathf.Clamp(x, 0, 1f);
                    MusicPlayer.Instance.PlayProgress = x;
                }
            }
        }

        progressbar.fillAmount = MusicPlayer.Instance.PlayProgress;
    }
}
