using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailExpand : MonoBehaviour {

    public float fadestart_distance = 1f;
    public float fadeend_distance = 2f;

    public float move_speed = 0.3f;
    Vector3 start_position;

    Vector3 local_dir;

    float fade_progress = 1f;

    TrailRenderer trail;

    public Gradient[] gradients;

	// Use this for initialization
	void Start () {
        local_dir = transform.InverseTransformVector((transform.position - transform.parent.position)).normalized;
        start_position = transform.position;
        trail = GetComponent<TrailRenderer>();
        trail.colorGradient = gradients[Random.Range(0, gradients.Length)];
    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(local_dir * Time.deltaTime * move_speed,Space.Self);
        float distance = Vector3.Distance(transform.position, transform.parent.position);

        if (distance >= fadestart_distance)
            fade_progress = Mathf.Lerp(1f, 0f, (distance - fadestart_distance) / (fadeend_distance - fadestart_distance));

        if (fade_progress <= 0.001f) Destroy(gameObject);

        //律动
        transform.position = new Vector3(transform.position.x, start_position.y + MusicPlayer.Instance.SamplesSum * 1.2f * fade_progress, transform.position.z);

        //color
        trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, fade_progress);
        trail.endColor = new Color(trail.endColor.r, trail.endColor.g, trail.endColor.b, fade_progress);


    }
}
