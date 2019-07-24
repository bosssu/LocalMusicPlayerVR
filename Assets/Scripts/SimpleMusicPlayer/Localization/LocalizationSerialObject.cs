using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="myassets/create localization data file")]
public class LocalizationSerialObject : ScriptableObject {

    public List<AppLocalizationInfo> localizationInfos;

}

[System.Serializable]
public class AppLocalizationInfo
{
    public string name;
    public List<LocalizationKeyValue> key_value_list;
}

[System.Serializable]
public class LocalizationKeyValue
{
    public string key;
    public string value;
}
