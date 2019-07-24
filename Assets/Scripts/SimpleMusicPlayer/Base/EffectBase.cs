using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInfoBase
{
    public Transform player;
}

public class EffectBase  {

    protected Transform effect_root;

    public virtual void Init() {
        effect_root = GameObject.Find("EffectRoot").transform;
    }

    public virtual void Update(float[] samples,float sum) { }

    public virtual void Reset() {

    }
}
