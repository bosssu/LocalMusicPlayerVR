using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EffectYanhua : HandEffectBase {

    public Transform ball_spawn_point;
    public GameObject template;
    public Transform bar;
    public AudioClip clip;
    AudioSource audiosrc;

    public float speed_mulpter = 1;
    float sample_sum;

    public override void Init()
    {
        base.Init();

        audiosrc = GetComponent<AudioSource>();

    }

    protected override void OnSamplesUpdate(float[] samples, float sum)
    {
        base.OnSamplesUpdate(samples, sum);

        sample_sum = sum;

    }

    public override void Update()
    {
        base.Update();

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Transform ball = MusicPlayerManager.Instance.SpawnPool.Spawn(template.transform,ball_spawn_point.position,Quaternion.identity);
            Rigidbody rid = ball.GetComponent<Rigidbody>();
            rid.velocity = bar.up * (1 + sample_sum) * speed_mulpter;
            audiosrc.PlayOneShot(clip,(0.5f + sample_sum));
        }

    }
}
