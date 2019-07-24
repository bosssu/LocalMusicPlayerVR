using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager> {


    public Vector3 player;
    public EffectSerialObject effect_config_data;

    List<EffectBase> effect_base;

    ESample es;
    EPulse ep;

    EffectBase _currentEffect;
    int index;

    private bool is_effect_active;
    public bool Is_effect_active
    {
        get
        {
            return is_effect_active;
        }

        set
        {
            is_effect_active = value;
        }
    }

    public override void Init()
    {
        base.Init();

        effect_config_data = Resources.Load<EffectSerialObject>("simple_music_player/prefabs/effect_config_data");

        player = GameObject.Find("PlayerController").transform.position;

        MusicPlayer.Instance.on_samples_update_event += OnSampleUpdate;

        effect_base = new List<EffectBase>();
        is_effect_active = true;

        //注册
        effect_base.Add(new ESample());
        effect_base.Add(new EPulse());
        effect_base.Add(new EInPulse());
        effect_base.Add(new ETrailExpand());

        LoadEffect(DataManager.Instance.Data_Save.effect_index);

    }

    public void NextEffect()
    {
        if (effect_base.Count > 0)
        {

            if (_currentEffect != null) _currentEffect.Reset();

            if (index >= effect_base.Count) index = 0;
            LoadEffect(this.index);
            index++;

        }

    }

    public void ReLoadEffect()
    {
        LoadEffect(index);
    }

    private void LoadEffect(int index)
    {
        if (_currentEffect != null) _currentEffect.Reset();

        _currentEffect = effect_base[index];
        _currentEffect.Init();

        DataManager.Instance.Data_Save.effect_index = index;
        DataManager.Instance.SaveData();

    }

    private void OnSampleUpdate(float[] samples,float sum)
    {
        if (is_effect_active && _currentEffect != null)
        {
            _currentEffect.Update(samples,sum);
        }
    }

    public override void UnInit()
    {
        base.UnInit();

        MusicPlayer.Instance.on_samples_update_event -= OnSampleUpdate;

    }

}
