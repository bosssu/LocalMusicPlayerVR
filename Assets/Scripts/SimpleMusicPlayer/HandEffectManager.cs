using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEffectManager : Singleton<HandEffectManager> {

    const string hand_effect_config_file_path = "simple_music_player/prefabs/hand_effect_config_data";

    OVRCameraRig rig;

    public Transform l_controller_root;
    public Transform r_controller_root;

    int index;
    List<HandEffectObject> effects;
    GameObject lcontroller_current_effect;
    GameObject rcontroller_current_effect;

    public override void Init()
    {
        base.Init();
        rig = GameObject.FindObjectOfType<OVRCameraRig>();
        this.l_controller_root = rig.leftControllerAnchor;
        this.r_controller_root = rig.rightControllerAnchor;

        effects = Resources.Load<HandEffectSerialObject>(hand_effect_config_file_path).effectObjects;

        MusicPlayer.Instance.on_samples_update_event += OnSampleUpdate;

        LoadEffect(DataManager.Instance.Data_Save.handeffect_index);
    }

    public void NextEffect()
    {
        if (effects.Count > 0)
        {
            ClearLastEffect();

            if (index >= effects.Count) index = 0;
            LoadEffect(this.index);
            index++;

        }
    }

    private void LoadEffect(int index)
    {
        if (effects.Count > 0)
        {
            ClearLastEffect();

            lcontroller_current_effect = GameObject.Instantiate<GameObject>(effects[index].left_hand_effect_prefab, l_controller_root);
            lcontroller_current_effect.transform.localPosition = Vector3.zero;
            lcontroller_current_effect.transform.localRotation = Quaternion.identity;
            //lcontroller_current_effect.transform.localScale = Vector3.one;
            rcontroller_current_effect = GameObject.Instantiate<GameObject>(effects[index].right_hand_effect_prefab, r_controller_root);
            rcontroller_current_effect.transform.localPosition = Vector3.zero;
            rcontroller_current_effect.transform.localRotation = Quaternion.identity;
            //rcontroller_current_effect.transform.localScale = Vector3.one;

            DataManager.Instance.Data_Save.handeffect_index = index;
            DataManager.Instance.SaveData();    

        }
        else
        {
            Debug.Log("hand effect count is 0");
        }

    }

    private void ClearLastEffect()
    {
        if (lcontroller_current_effect != null) GameObject.Destroy(lcontroller_current_effect);
        if (rcontroller_current_effect != null) GameObject.Destroy(rcontroller_current_effect);
    }

    private void OnSampleUpdate(float[] samples,float sum)
    {

    }

    public override void UnInit()
    {
        base.UnInit();

        MusicPlayer.Instance.on_samples_update_event -= OnSampleUpdate;
    }

}
