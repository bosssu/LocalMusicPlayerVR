using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrail : HandEffectBase {


    public float sample_mulpter = 3f;
    public Gradient gradient_color;
    TrailRenderer trail;
    ParticleSystem particle_system;
    ParticleSystem.MainModule main;

    public override void Init()
    {
        base.Init();

        particle_system = GetComponentInChildren<ParticleSystem>();
        main = particle_system.main;
        trail = GetComponentInChildren<TrailRenderer>();

        trail.Clear();

    }

    protected override void OnSamplesUpdate(float[] samples,float sum)
    {
        base.OnSamplesUpdate(samples,sum);

        Color color = gradient_color.Evaluate(sum * sample_mulpter);
        main.startColor = color;
        GetComponentInChildren<MeshRenderer>().material.color = color;
        transform.localScale = start_localscale * (1 + sum * sample_mulpter);
    }
}
