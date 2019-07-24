using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springball : HandObjectBase {

    public Gradient gradient;
    public float force = 1f;

    MeshRenderer render;
    Rigidbody rid;

    float last_sum;

    public override void Init()
    {
        base.Init();

        render = GetComponent<MeshRenderer>();
        rid = GetComponent<Rigidbody>();

    }

    protected override void OnSamplesUpdate(float[] samples, float sum)
    {
        base.OnSamplesUpdate(samples, sum);

        transform.localScale = start_localscale * (1 + sum * 1.5f);
        render.material.color = gradient.Evaluate(sum * 4);
        Vector2 c = Random.insideUnitCircle;

        if (sum - last_sum > 0.1f)
        {
            rid.velocity += new Vector3(c.x, sum * force, c.y * 0.1f);
        }

        last_sum = sum;


    }

}
