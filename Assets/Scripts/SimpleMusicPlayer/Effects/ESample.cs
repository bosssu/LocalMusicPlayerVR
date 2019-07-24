using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESampleInfo : EffectInfoBase
{
    public Gradient cube_color;
}

public class ESample : EffectBase {

    const string cube_path = "simple_music_player/prefabs/audio_effects/cube";

    List<Transform> objects;

    float radius = 2;

    Gradient cube_color;

    public override void Init()
    {
        base.Init();

        Vector3 player = EffectManager.Instance.player;
        EffectSerialObject effect_config_data = EffectManager.Instance.effect_config_data;
        cube_color = effect_config_data.cube_color;

        objects = new List<Transform>();
        for (int i = 0; i < MusicPlayer.samples_count; i++)
        {
            float base_angle = 360f / MusicPlayer.samples_count;
            Quaternion rot = Quaternion.AngleAxis(base_angle * i, Vector3.up);
            Vector3 dir = rot * Vector3.right;
            Vector3 center_pos = new Vector3(player.x, 0, player.z);
            Vector3 pos = center_pos + dir * radius;

            GameObject g = Res.LoadObj(cube_path);
            g.transform.SetParent(effect_root);
            g.transform.position = pos;
            g.transform.LookAt(new Vector3(player.x, g.transform.position.y, player.z));
            objects.Add(g.transform);
        }

    }

    public override void Update(float[] samples,float sum)
    {
        base.Update(samples,sum);

        for (int i = 0; i < samples.Length; i++)
        {
            Transform trans = objects[i];
            float y = samples[i] * 0.25f * (50 + i * i * 0.5f);
            //y *= MusicPlayerManager.Instance.Amp_Mulpter;
            if (y >= 1.5f) y = 1.5f;
            trans.localScale = new Vector3(trans.localScale.x, y, trans.localScale.z);
            trans.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", cube_color.Evaluate(samples[i] * 500));
        }
    }

    public override void Reset()
    {
        base.Reset();
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject.Destroy(objects[i].gameObject);
        }
        objects.Clear();
    }
}
