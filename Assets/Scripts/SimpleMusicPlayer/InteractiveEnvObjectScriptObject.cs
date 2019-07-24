using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="myassets/create interactive assets")]
public class InteractiveEnvObjectScriptObject : ScriptableObject {

    public List<EnviromentInteractiveObjectInfo> envs;
    public List<EnviromentInteractiveObjectInfo> toys;
    public List<EnviromentInteractiveObjectInfo> others;
    public List<EnviromentInteractiveObjectInfo> audio_response;
    public List<EnviromentInteractiveObjectInfo> global_env;
}

[System.Serializable]
public class EnviromentInteractiveObjectInfo
{
    public string name;
    public string asset_name;
    public EnviromentObjectType type;
    public int id;
    public string texture_name;
    public Vector3 localposition;
    public Quaternion rotation;
}

public enum EnviromentObjectType
{
    Env,Toy,Other,Audioresponse,GlobalEnv
}
