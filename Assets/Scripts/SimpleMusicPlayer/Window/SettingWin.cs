using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWin : WindowBase {

    Toggle toggle_rotate;
    Toggle toggle_play_loop;
    Toggle toggle_play_random;
    Slider slider_amp_mulpter;
    Slider slider_volume;
    Button btn_close;
    Button btn_clear_save;

    public override void Init()
    {
        base.Init();


        win_root = UIManager.Instance.UIMain.transform.Find("Setting");

        toggle_rotate = win_root.Find("toggle_rotate").GetComponent<Toggle>();
        toggle_play_loop = win_root.Find("toggle_play_loop").GetComponent<Toggle>();
        toggle_play_random = win_root.Find("toggle_play_random").GetComponent<Toggle>();
        slider_amp_mulpter = win_root.Find("slider_amp_mulpter").GetComponent<Slider>();
        slider_volume = win_root.Find("slider_volume").GetComponent<Slider>();
        btn_close = win_root.Find("btn_close").GetComponent<Button>();
        btn_clear_save = win_root.Find("btn_clear_save").GetComponent<Button>();


        toggle_play_loop.onValueChanged.AddListener((value) => {
            MusicPlayer.Instance._PlayMode = value ? MusicPlayer.PlayMode.SINGLE_LOOP : MusicPlayer.PlayMode.AUTO_NEXT;
            DataManager.Instance.Data_Save.playmode = MusicPlayer.Instance._PlayMode;
        });
        toggle_play_random.onValueChanged.AddListener((value) => {
            MusicPlayer.Instance.IsRandomPlay = value;
            DataManager.Instance.Data_Save.isRandom = value;
        });
        slider_amp_mulpter.onValueChanged.AddListener((value) => {
            MusicPlayerManager.Instance.Amp_Mulpter = value;
            DataManager.Instance.Data_Save.amp_mulpter = value;

        });
        slider_volume.onValueChanged.AddListener((value) => {
            MusicPlayer.Instance.AudioSouce.volume = value;
            DataManager.Instance.Data_Save.volume = value;
        });
        btn_close.onClick.AddListener(() => {
            Close();
            DataManager.Instance.SaveData();
        });
        btn_clear_save.onClick.AddListener(() => {
            EnvManager.Instance.ClearSave();
        });
        toggle_rotate.onValueChanged.AddListener((v) => {
            EnvManager.Instance.EnableTubeRotate = v;
            DataManager.Instance.Data_Save.isTubeAutoRotate = v;
        });

        //要放到后面，需要前面的事件先绑定
        toggle_play_loop.isOn = DataManager.Instance.Data_Save.playmode == MusicPlayer.PlayMode.SINGLE_LOOP ? true : false;
        toggle_play_random.isOn = DataManager.Instance.Data_Save.isRandom;
        slider_amp_mulpter.value = DataManager.Instance.Data_Save.amp_mulpter;
        slider_volume.value = DataManager.Instance.Data_Save.volume;
        toggle_rotate.isOn = DataManager.Instance.Data_Save.isTubeAutoRotate;

    }
}
