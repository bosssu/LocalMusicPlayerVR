using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EPulseInfo : EffectInfoBase
{
    public Gradient cube_color;
}

public class EPulse : EffectBase {

    float radius = 2;

    Gradient line_color;
    LineRenderer linerender;
    List<Vector3> start_points;
    Vector3[] current_points;

    public override void Init()
    {
        base.Init();

        Vector3 player = EffectManager.Instance.player;
        EffectSerialObject effect_config_data = EffectManager.Instance.effect_config_data;
        line_color = effect_config_data.linecolor;

        linerender = new GameObject("pulse_line").AddComponent<LineRenderer>(); ;
        linerender.transform.position = player;
        linerender.transform.SetParent(effect_root);
        linerender.widthMultiplier = 0.1f;
        linerender.loop = true;
        linerender.material = Resources.Load<Material>("simple_music_player/materials/linerender");

        start_points = new List<Vector3>();
        current_points = new Vector3[MusicPlayer.samples_count];
        for (int i = 0; i < MusicPlayer.samples_count; i++)
        {
            float base_angle = 360f / MusicPlayer.samples_count;
            Quaternion rot = Quaternion.AngleAxis(base_angle * i, Vector3.up);
            Vector3 dir = rot * Vector3.right;
            Vector3 center_pos = new Vector3(player.x, .1f, player.z);
            Vector3 pos = center_pos + dir * radius;

            start_points.Add(pos);

        }

        linerender.positionCount = start_points.Count;
        linerender.SetPositions(start_points.ToArray());
        linerender.startColor = line_color.Evaluate(0);
        linerender.endColor = line_color.Evaluate(1);

    }

    public override void Update(float[] samples,float sum)
    {
        base.Update(samples,sum);

        for (int i = 0; i < samples.Length; i++)
        {
            float y = samples[i] * 0.25f * (50 + i * i * 0.5f);
            if (y >= 1.5f) y = 1.5f;
            Vector3 pos = new Vector3(start_points[i].x, y + 0.1f, start_points[i].z);
            current_points[i] = pos;
        }

        linerender.SetPositions(current_points);
        linerender.material.SetColor("_EmissionColor", line_color.Evaluate(sum * 5));
    }

    public override void Reset()
    {
        base.Reset();

        GameObject.Destroy(linerender.gameObject);
        current_points = null;
        start_points.Clear();
    }
}
