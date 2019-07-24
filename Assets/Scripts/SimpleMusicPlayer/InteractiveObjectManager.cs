using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectManager : Singleton<InteractiveObjectManager> {

    const string interactive_object_data_path = "simple_music_player/prefabs/interactiveobject_data";
    const string interactive_objects_basepath = "simple_music_player/prefabs/interactive_objects";

    Transform player;

    InteractiveEnvObjectScriptObject _interactive_object_data;

    public List<GameObject> _interactive_objects;
    Dictionary<GameObject, EnviromentInteractiveObjectInfo> _dic_t;

    public Transform Player
    { get { return player; } }

    Dictionary<int, EnviromentInteractiveObjectInfo> _dic_envinfo;
    public Dictionary<int, EnviromentInteractiveObjectInfo> _DIC_Envinfo
    {
        get { return _dic_envinfo; }
    }

    public InteractiveEnvObjectScriptObject InteractiveEnvObjectScriptObject
    {
        get { return _interactive_object_data; }
    }

    public override void Init()
    {
        base.Init();

        _interactive_objects = new List<GameObject>();
        _dic_t = new Dictionary<GameObject, EnviromentInteractiveObjectInfo>();

        _interactive_object_data = Resources.Load<InteractiveEnvObjectScriptObject>(interactive_object_data_path);

        _dic_envinfo = new Dictionary<int, EnviromentInteractiveObjectInfo>();
        foreach (var item in _interactive_object_data.envs)
            if (!_dic_envinfo.ContainsKey(item.id))
                _dic_envinfo.Add(item.id, item);

        foreach (var item in _interactive_object_data.toys)
            if (!_dic_envinfo.ContainsKey(item.id))
                _dic_envinfo.Add(item.id, item);

        foreach (var item in _interactive_object_data.others)
            if (!_dic_envinfo.ContainsKey(item.id))
                _dic_envinfo.Add(item.id, item);

        foreach (var item in _interactive_object_data.audio_response)
            if (!_dic_envinfo.ContainsKey(item.id))
                _dic_envinfo.Add(item.id, item);

        foreach (var item in _interactive_object_data.global_env)
            if (!_dic_envinfo.ContainsKey(item.id))
                _dic_envinfo.Add(item.id, item);

        player = GameObject.Find("PlayerController").transform;

        InitObjects();

    }

    private void InitObjects()
    {
        LoadPlace();
    }

    public GameObject CreateObjectWithID(int id)
    {
        if (_dic_envinfo.ContainsKey(id))
        {
            string s_str = "";
            EnviromentObjectType obj_type = _dic_envinfo[id].type;
            switch (obj_type)
            {
                case EnviromentObjectType.Env:
                    s_str = "/env/";
                    break;
                case EnviromentObjectType.Toy:
                    s_str = "/toy/";
                    break;
                case EnviromentObjectType.Other:
                    s_str = "/other/";
                    break;
                case EnviromentObjectType.Audioresponse:
                    s_str = "/audioresponse/";
                    break;
                case EnviromentObjectType.GlobalEnv:
                    s_str = "/global_env/";
                    break;
                default:
                    break;
            }
            string obj_path = string.Format("{0}{1}{2}", interactive_objects_basepath, s_str, _dic_envinfo[id].asset_name);
            GameObject obj = Res.LoadObj(obj_path);
            MeshRenderer[] renders = obj.GetComponentsInChildren<MeshRenderer>();
            //foreach (var item in renders)
            //    item.material.shader = Shader.Find("Custom/ToyCubeOutline");

            if(obj_type == EnviromentObjectType.GlobalEnv)
                obj.transform.SetParent(EnvManager.Instance.Global_env_object_root);
            else
                obj.transform.SetParent(EnvManager.Instance.Env_Object_Root);

            _dic_t.Add(obj, _dic_envinfo[id]);

            return obj;
        }

        return null;
    }

    public void SavePlace()
    {
        if (_interactive_objects.Count > 0)
        {
            DataManager.Instance.Data_Save.env_objects.Clear();

            foreach (var item in _interactive_objects)
            {
                if (_dic_t.ContainsKey(item))
                {
                    EnvObject o = new EnvObject();
                    o.position = item.transform.position;
                    o.local_scale = item.transform.localScale;
                    o.rotation = item.transform.rotation;

                    o.info = _dic_t[item];

                    DataManager.Instance.Data_Save.env_objects.Add(o);

                }
            }

            DataManager.Instance.SaveData();
        }
    }

    public void LoadPlace()
    {
        if (_interactive_objects.Count > 0)
        {
            for (int i = 0; i < _interactive_objects.Count; i++)
            {
                GameObject.Destroy(_interactive_objects[i]);
            }

            _dic_t.Clear();
            _interactive_objects.Clear();

        }

        List<EnvObject> objects = DataManager.Instance.Data_Save.env_objects;
        if (objects.Count > 0)
        {
            foreach (var item in objects)
            {
                GameObject o = CreateObjectWithID(item.info.id);
                o.transform.position = item.position;
                o.transform.rotation = item.rotation;
                o.transform.localScale = item.local_scale;

                _interactive_objects.Add(o);
            }
        }
    }


    public override void UnInit()
    {
        base.UnInit();
    }
}
