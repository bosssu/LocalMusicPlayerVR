using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEffectBase : MonoBehaviour {

    protected Vector3 start_postion;
    protected Vector3 start_localscale;
    protected Quaternion start_rotation;

    MusicPlayer instance;

    private void Awake()
    {
        instance = MusicPlayer.Instance;
        start_postion = transform.position;
        start_localscale = transform.localScale;
        start_rotation = transform.rotation;

        Init();
    }

    public virtual void Init()
    {
        instance.on_samples_update_event += OnSamplesUpdate;

    }

    public virtual void Update() { }

    protected virtual void OnSamplesUpdate(float[] samples,float sum)
    {

    }

    private void OnDestroy()
    {
        instance.on_samples_update_event -= OnSamplesUpdate;
    }

    public virtual void Reset()
    {

    }
}
