using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveMainWin : WindowBase {

    Toggle toggle_env;
    Toggle toggle_toy;
    Toggle toggle_other;
    Toggle toggle_audioresponse;
    Toggle toggle_global_env;
    Button Btn_Save;
    Button Btn_Load;

    Transform InfoPanel;

    Transform ListView;
    Transform view;
    GameObject template;

    Dictionary<int, GameObject> dic_global_env;

    public override void Init()
    {
        base.Init();

        dic_global_env = new Dictionary<int, GameObject>();

        win_root = UIManager.Instance.UI_Interactive.transform;

        toggle_env = win_root.Find("MainPanel/ToggleGroup/toggle_env").GetComponent<Toggle>();
        toggle_toy = win_root.Find("MainPanel/ToggleGroup/toggle_toy").GetComponent<Toggle>();
        toggle_other = win_root.Find("MainPanel/ToggleGroup/toggle_other").GetComponent<Toggle>();
        toggle_audioresponse = win_root.Find("MainPanel/ToggleGroup/toggle_audioresponse").GetComponent<Toggle>();
        toggle_global_env = win_root.Find("MainPanel/ToggleGroup/toggle_global_env").GetComponent<Toggle>();
        Btn_Save = win_root.Find("MainPanel/Btn_Save").GetComponent<Button>();
        Btn_Load = win_root.Find("MainPanel/Btn_Load").GetComponent<Button>();

        InfoPanel = win_root.Find("MainPanel/InfoPanel");

        ListView = win_root.Find("MainPanel/ListView");
        view = ListView.Find("ScrollView/view");
        template = view.Find("template").gameObject;
        template.SetActive(false);

        //event
        toggle_env.onValueChanged.AddListener((v) => {
            if (v == true)
            {
                List<EnviromentInteractiveObjectInfo> infos = InteractiveObjectManager.Instance.InteractiveEnvObjectScriptObject.envs;
                OnToggleChecked(infos);
            }
        });

        toggle_toy.onValueChanged.AddListener((v) => {
            if (v == true)
            {
                List<EnviromentInteractiveObjectInfo> infos = InteractiveObjectManager.Instance.InteractiveEnvObjectScriptObject.toys;
                OnToggleChecked(infos);
            }
        });

        toggle_other.onValueChanged.AddListener((v) => {
            if (v == true)
            {
                List<EnviromentInteractiveObjectInfo> infos = InteractiveObjectManager.Instance.InteractiveEnvObjectScriptObject.others;
                OnToggleChecked(infos);
            }
        });

        toggle_audioresponse.onValueChanged.AddListener((v) => {
            if (v == true)
            {
                List<EnviromentInteractiveObjectInfo> infos = InteractiveObjectManager.Instance.InteractiveEnvObjectScriptObject.audio_response;
                OnToggleChecked(infos);
            }
        });

        toggle_global_env.onValueChanged.AddListener((v) => {
            if (v == true)
            {
                List<EnviromentInteractiveObjectInfo> infos = InteractiveObjectManager.Instance.InteractiveEnvObjectScriptObject.global_env;
                OnToggleChecked(infos);
            }
        });

        Btn_Save.onClick.AddListener(() => {
            InteractiveObjectManager.Instance.SavePlace();
        });

        Btn_Load.onClick.AddListener(() => {
            InteractiveObjectManager.Instance.LoadPlace();
        });

        //default active
        toggle_env.isOn = true;

    }

    private void OnToggleChecked(List<EnviromentInteractiveObjectInfo> infos)
    {
        Utils.DestroyChildObjects(view,new List<string> { "template" });

        if (infos.Count > 0)
        {

            foreach (var inf in infos)
            {
                Transform item = GameObject.Instantiate<GameObject>(template, view).transform;
                item.gameObject.SetActive(true);

                item.name = inf.id.ToString();
                item.GetComponentInChildren<Text>().text = inf.name;
                item.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    int id = int.Parse(item.name);
                    if (id > 5000 && id <= 6000)
                    {
                        if (dic_global_env.ContainsKey(id))
                        {
                            GameObject.Destroy(dic_global_env[id]);
                            dic_global_env.Remove(id);
                        }
                        else
                        {
                            GameObject o = InteractiveObjectManager.Instance.CreateObjectWithID(id);
                            o.name = id.ToString();
                            EnviromentInteractiveObjectInfo info = InteractiveObjectManager.Instance._DIC_Envinfo[id];
                            o.transform.localPosition = info.localposition;
                            dic_global_env.Add(id, o);
                        }
                    }
                    else
                    {
                        GameObject o = InteractiveObjectManager.Instance.CreateObjectWithID(id);
                        Transform player = InteractiveObjectManager.Instance.Player;
                        o.transform.position = player.position + player.forward * .6f;

                        InteractiveObjectManager.Instance._interactive_objects.Add(o);
                    }
                });
            }
        }
    }

}
