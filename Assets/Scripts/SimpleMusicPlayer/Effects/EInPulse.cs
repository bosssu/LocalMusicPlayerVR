using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EInPulseInfo : EffectInfoBase
{
    public Gradient cube_color;
}

public class EInPulse : EffectBase {

    float rotatespeed = 2;
    float multper = 1.4f;

    const string trail_object_path = "simple_music_player/prefabs/audio_effects/InPulseObject";

    GameObject traiobject;
    TrailRenderer trail;
    Transform trai2;
    Vector3 trail_start_pos;
    Color trail_begin_color;

    public override void Init()
    {
        base.Init();

        traiobject = Res.LoadObj(trail_object_path);
        traiobject.transform.SetParent(effect_root);
        traiobject.transform.localPosition = new Vector3(0,0.2f,0);
        trail = traiobject.transform.Find("trail").GetComponent<TrailRenderer>() ;
        trai2 = traiobject.transform.Find("trail2");
        trail_start_pos = trail.transform.position;
        trail_begin_color = trail.startColor;

    }

    public override void Update(float[] samples,float sum)
    {
        base.Update(samples,sum);

        trail.transform.position = new Vector3(trail.transform.position.x, trail_start_pos.y +  sum * multper,trail.transform.position.z);
        //trai2.position = new Vector3(trai2.position.x, trail_start_pos.y + sum * multper * 0.2f, trai2.position.z);
        trail.startColor = trail_begin_color * EffectManager.Instance.effect_config_data.e_in_pulse.Evaluate(sum * 5);

    }

    public override void Reset()
    {
        base.Reset();

        GameObject.Destroy(traiobject);

    }
}
