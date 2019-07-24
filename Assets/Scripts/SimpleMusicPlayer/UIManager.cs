using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    OVRCameraRig cam_rig;

    RectTransform lable_Rt;
    RectTransform playmode_Rt;
    RectTransform random_Rt;
    Text lable_text;
    bool is_show_mai_ui;
    bool is_show_interactive;
    AudioFileInfo info;
    GameObject guide_img;

    GameObject ui_helper;
    LaserPointer lp;
    LineRenderer lr;

    GameObject ui_main;
    OVRRaycaster raycaster;

    OVRRaycaster interactive_raycaster;

    GameObject ui_interactive;


    MainWin main_win;
    SettingWin set_win;
    PlayWin play_win;
    EnvWin env_win;
    InteractiveMainWin interactive_main_win;

    float ui_main_start_distance;

    public GameObject UIMain
    {
        get { return ui_main; }
    }

    public GameObject UI_Interactive
    {
        get { return ui_interactive; }
    }

    public override void Init()
    {
        base.Init();

        ui_main = GameObject.Find("UIMain");
        ui_interactive = GameObject.Find("UIInteractive");
        guide_img = ui_main.transform.Find("guide_img").gameObject;

        is_show_mai_ui = true;
        is_show_interactive = false;
        ui_main.SetActive(is_show_mai_ui);
        ui_interactive.SetActive(is_show_interactive);

        ui_helper = Res.LoadObj("simple_music_player/prefabs/UIHelpers");
        lp = ui_helper.GetComponentInChildren<LaserPointer>();
        lr = ui_helper.GetComponentInChildren<LineRenderer>();

        interactive_raycaster = Utils.GetSafeComponet<OVRRaycaster>(ui_interactive);
        interactive_raycaster.pointer = lp.gameObject;

        cam_rig = GameObject.FindObjectOfType<OVRCameraRig>();
        ui_main_start_distance = Vector2.Distance(new Vector2(cam_rig.centerEyeAnchor.position.x, cam_rig.centerEyeAnchor.position.z),
            new Vector2(ui_main.transform.position.x, ui_main.transform.position.z));
        raycaster = Utils.GetSafeComponet<OVRRaycaster>(ui_main);
        raycaster.pointer = lp.gameObject;

        lp.laserBeamBehavior = LaserPointer.LaserBeamBehavior.OnWhenHitTarget;

        main_win = new MainWin();main_win.Init();
        set_win = new SettingWin();set_win.Init();
        play_win = new PlayWin();play_win.Init();
        env_win = new EnvWin();env_win.Init();

        interactive_main_win = new InteractiveMainWin();interactive_main_win.Init();

        ui_main.transform.Find("Btn_Set").GetComponent<Button>().onClick.AddListener(() => {
            set_win.Open();
        });
        ui_main.transform.Find("Btn_envset").GetComponent<Button>().onClick.AddListener(() => {
            env_win.Open();
        });
        ui_main.transform.Find("Btn_guide").GetComponent<Button>().onClick.AddListener(() => {
            guide_img.SetActive(!guide_img.activeInHierarchy);
        });

    }

    public void Update()
    {
        set_win.Update();
        main_win.Update();
        play_win.Update();
    }

    public void OnShowHideClick()
    {
        is_show_mai_ui = !is_show_mai_ui;
        ui_main.SetActive(is_show_mai_ui);
        if (is_show_mai_ui)
        {
            Vector3 p = cam_rig.centerEyeAnchor.position + cam_rig.centerEyeAnchor.forward * ui_main_start_distance;
            Vector3 ui_main_new_pos = new Vector3(p.x, ui_main.transform.position.y, p.z);
            ui_main.transform.rotation = Quaternion.LookRotation(cam_rig.centerEyeAnchor.forward);
            ui_main.transform.position = ui_main_new_pos;
        }
    }

    public void OnInteractiveUIShowHideClick()
    {
        is_show_interactive = !is_show_interactive;
        ui_interactive.SetActive(is_show_interactive);
        if (is_show_interactive)
        {
            Vector3 p = cam_rig.centerEyeAnchor.position + cam_rig.centerEyeAnchor.forward * 0.6f;
            ui_interactive.transform.rotation = Quaternion.LookRotation(cam_rig.centerEyeAnchor.forward);
            ui_interactive.transform.position = p;
        }
    }

    public void OnSongPlay(AudioFileInfoX info)
    {
        Debug.Log(info.info.title);
        string detail_info = string.Format("artist：{0}\nsinger：{1}\nalbum：{2}", info.info.title, info.info.author, info.info.album);
        DataManager.Instance.LoadAudioTexture(info.path, (tex) => {
            main_win.SetInfoPanel(detail_info,tex);
        });

    }

    public override void UnInit()
    {
        base.UnInit();
    }
}
