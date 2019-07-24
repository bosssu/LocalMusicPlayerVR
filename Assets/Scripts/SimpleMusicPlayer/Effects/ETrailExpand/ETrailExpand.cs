using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETrailExpand : EffectBase {

    const string cube_path = "simple_music_player/prefabs/audio_effects/InPulseObjects";

    Transform trai_root;
    GameObject trail_template;

    public override void Init()
    {
        base.Init();

        trai_root = Res.LoadObj(cube_path).transform;
        trai_root.SetParent(EnvManager.Instance.Env_Object_Root);
        trail_template = trai_root.Find("trail").gameObject;

    }

    public override void Reset()
    {
        base.Reset();

        GameObject.Destroy(trai_root.gameObject);
    }
}
