using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : Singleton<LocalizationManager> {


    const string localization_datafile_path = "simple_music_player/prefabs/localization_info_data";

    Dictionary<string, Dictionary<string,string>> dict_localization_info;
    public Dictionary<string, Dictionary<string, string>> Dict_Localization_Info
    {
        get { return this.dict_localization_info; }
    }

    string current_localization = "";
    public string CurrentLocalization
    {
        set { this.current_localization = value; }
    }

    public bool IsCurrentValueExistKey(string key)
    {
        return  dict_localization_info.ContainsKey(current_localization)
            &&  dict_localization_info[current_localization].ContainsKey(key);
    }

    public string GetCurrentValueWithKey(string key)
    {
        if(IsCurrentValueExistKey(key))
            return this.dict_localization_info[current_localization][key];
        return null;
    }

    public override void Init()
    {
        base.Init();

        List<AppLocalizationInfo> info_list = Resources.Load<LocalizationSerialObject>(localization_datafile_path).localizationInfos;
        dict_localization_info = new Dictionary<string, Dictionary<string, string>>();

        if (info_list.Count > 0)
        {
            foreach (var item in info_list)
            {
                Dictionary<string, string> dict_key_value = new Dictionary<string, string>();
                if (!dict_localization_info.ContainsKey(item.name))
                    dict_localization_info.Add(item.name, dict_key_value);

                foreach (var key_value in item.key_value_list)
                {
                    if (!dict_key_value.ContainsKey(key_value.key))
                        dict_key_value.Add(key_value.key, key_value.value);
                }
            }
        }

    }



}
