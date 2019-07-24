using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yanhuaball : MonoBehaviour {

    public float max_dur = 3f;
    public float min_dur = 1f;
    TrailRenderer trail;
    Rigidbody rgd;

    public Transform firework_object;

	void Awake () {

        rgd = GetComponentInChildren<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();

	}

    private void OnEnable()
    {
        trail.Clear();
        Invoke("OnOpenFirework", Random.Range(min_dur, max_dur));
    }

    private void Update()
    {
        if(rgd.velocity.y < 0.5f)
            rgd.velocity = Vector3.Lerp(rgd.velocity, Vector3.zero, .2f);
    }

    private void OnOpenFirework()
    {
        //Transform firwork_obj = MusicPlayerManager.Instance.SpawnPool.Spawn(firework_object, transform.position, Quaternion.identity);
        //GameObject o = Instantiate<GameObject>(firework_object.gameObject, transform.position, Quaternion.identity);
        //o.GetComponent<ParticleSystem>().Play();

        Invoke("OnRecyle", .7f);
      
    }

    private void OnRecyle()
    {
        MusicPlayerManager.Instance.SpawnPool.Despawn(transform);
    }

}
