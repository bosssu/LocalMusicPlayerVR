using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum AudiofileInfoSortType
{
    Album,
    Singer
}

public class MainWinListViewItem
{
    public string object_name;
    public string show_name;
}

public class MainWin : WindowBase {

    Transform scrollview;

    Transform info_panel;
    Transform template;
    Transform view;
    Transform leftbar;

    Toggle toggle_singer;
    Toggle toggle_album;
    Toggle toggle_default;
    InputField input_serach;

    List<MainWinListViewItem> temp_items = new List<MainWinListViewItem>();

    public override void Init()
    {
        base.Init();

        win_root = UIManager.Instance.UIMain.transform.Find("MainPanel");

        scrollview = win_root.Find("ScrollView");
        view = scrollview.Find("view");
        info_panel = win_root.Find("InfoPanel");
        template = view.Find("template");
        leftbar = win_root.Find("LeftBar");
        toggle_singer = leftbar.Find("toggle_singer").GetComponentInChildren<Toggle>();
        toggle_album = leftbar.Find("toggle_album").GetComponentInChildren<Toggle>();
        toggle_default = leftbar.Find("toggle_default").GetComponentInChildren<Toggle>();
        input_serach = leftbar.Find("input_serach").GetComponentInChildren<InputField>();

        toggle_singer.onValueChanged.AddListener((value) => {
            temp_items = new List<MainWinListViewItem>();
            foreach (var item in DataManager.Instance.AllAudioFileInfomation)
            {
                if (!IsContainAudiofileInfoName(ref temp_items, item.info.author))
                    temp_items.Add(new MainWinListViewItem() { show_name = item.info.author, object_name = item.info.author });
            }
            //歌手列表
            CreateList(temp_items, (temp)=> {

                temp_items.Clear();
                foreach (var item in DataManager.Instance.AllAudioFileInfomation)
                {
                    if (item.info.author == temp.name)
                        temp_items.Add(new MainWinListViewItem() { object_name = item.info.author, show_name = item.info.author });
                }
                //歌手歌单列表
                CreateList(temp_items, (t) => {
                    temp_items.Clear();
                    foreach (var item in DataManager.Instance.AllAudioFileInfomation)
                    {
                        if (item.info.author == t.name)
                            temp_items.Add(new MainWinListViewItem() { object_name = item.info.title, show_name = item.info.title });
                    }
                    CreateList(temp_items, (song) => {
                        MusicPlayer.Instance.PlayMusic(DataManager.Instance.Dic_AudioInfo[song.name]);
                    });
                });

            });
        });

        toggle_album.onValueChanged.AddListener((value) => {
            temp_items = new List<MainWinListViewItem>();
            foreach (var item in DataManager.Instance.AllAudioFileInfomation)
            {
                if (!IsContainAudiofileInfoName(ref temp_items, item.info.album))
                    temp_items.Add(new MainWinListViewItem() { show_name = item.info.album, object_name = item.info.album });
            }
            //专辑列表
            CreateList(temp_items, (temp) => {

                temp_items.Clear();
                foreach (var item in DataManager.Instance.AllAudioFileInfomation)
                {
                    if (item.info.album == temp.name)
                        temp_items.Add(new MainWinListViewItem() { object_name = item.info.album, show_name = item.info.album });
                }
                //专辑歌单列表
                CreateList(temp_items, (t) => {
                    temp_items.Clear();
                    foreach (var item in DataManager.Instance.AllAudioFileInfomation)
                    {
                        if (item.info.album == t.name)
                            temp_items.Add(new MainWinListViewItem() { object_name = item.info.title, show_name = item.info.title });
                    }
                    //
                    CreateList(temp_items, (song) => {
                        MusicPlayer.Instance.PlayMusic(DataManager.Instance.Dic_AudioInfo[song.name]);
                    });
                });

            });
        });

        template.gameObject.SetActive(false);


        toggle_default.onValueChanged.AddListener((value) => {
            //初始化列表
            temp_items = new List<MainWinListViewItem>();
            foreach (var audioinfo in DataManager.Instance.AllAudioFileInfomation)
            {
                temp_items.Add(new MainWinListViewItem() { object_name = audioinfo.info.title, show_name = audioinfo.info.title });
            }
            CreateList(temp_items, (temp) => {
                AudioFileInfoX t = DataManager.Instance.Dic_AudioInfo[temp.name];
                MusicPlayer.Instance.PlayMusic(t);
            });
        });

        //input_serach.onEndEdit.AddListener((str) => {

        //    if (this.temp_items == null || temp_items.Count == 0) return;

        //    List<MainWinListViewItem> serach_items = new List<MainWinListViewItem>();
        //    foreach (var showinfo in temp_items)
        //    {
        //        if(showinfo.show_name.ToLower().Contains(str.ToLower()))
        //            serach_items.Add(showinfo);
        //    }
        //    CreateList(serach_items, (temp) => {
        //        AudioFileInfoX t = DataManager.Instance.Dic_AudioInfo[temp.name];
        //        MusicPlayer.Instance.PlayMusic(t);
        //    });
        //});

        toggle_default.isOn = true;

    }

    public void CreateList(List<MainWinListViewItem> itemstr,Action<GameObject> callback)
    {

        Utils.DestroyChildObjects(view, new List<string>() { "template" });

        if (itemstr != null && itemstr.Count > 0)
        {
            foreach (var str in itemstr)
            {
                GameObject temp = GameObject.Instantiate<GameObject>(template.gameObject, view);

                temp.GetComponentInChildren<Text>().text = str.show_name;
                temp.transform.localPosition = Vector3.zero;
                temp.name = str.object_name;
                temp.SetActive(true);

                temp.GetComponentInChildren<Button>().onClick.AddListener(() => {

                    if (callback != null) callback(temp);

                });
            }
        }
    }

    public void SetInfoPanel(string info,Texture2D texture)
    {
        info_panel.GetComponentInChildren<Text>().text = info;
        Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        //info_panel.GetComponent<Image>().sprite = sp;
        info_panel.Find("Image").GetComponent<Image>().sprite = sp;
    }

    private bool IsContainAudiofileInfoName(ref List<MainWinListViewItem> list, string name)
    {
        foreach (var item in list)
        {
            if (item.show_name == name)
                return true;
        }

        return false;
    }


}
