using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="myassets/create handeffect_assets")]
public class HandEffectSerialObject : ScriptableObject {

    public List<HandEffectObject> effectObjects;
	
}

[System.Serializable]
public class HandEffectObject
{
    public string name;
    public GameObject left_hand_effect_prefab;
    public GameObject right_hand_effect_prefab;
}
