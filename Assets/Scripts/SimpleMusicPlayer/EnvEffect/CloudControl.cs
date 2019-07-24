using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudControl : MonoBehaviour {

    public float max_offset_y = 0.5f;
    public float max_speed = 1;
    public float max_rotate_speed_y = .1f;
    Material mat;

    Vector2 start_position;
    float y;
    Vector3 move_speed;

	void Start () {
        start_position = transform.position;
        Invoke("StartFloat", Random.Range(0f, 2f));
        Invoke("StartMove", Random.Range(0f, 2f));
        mat = GetComponent<MeshRenderer>().material;

        //EnvManager.Instance.on_maincolor_tint_changed = (tint) => {
        //    mat.SetColor("_EmissionColor", tint.colortint);
        //};

    }

    void Update () {
        //cloud float
        transform.position = new Vector3(transform.position.x, start_position.y + Mathf.Sin(Time.time * 0.1f) * y, transform.position.z);
        transform.Rotate(new Vector3(0, Random.Range(-max_rotate_speed_y, max_rotate_speed_y) * 0.3f, 0));

        //move
        Vector3 speed_offset = new Vector3(Mathf.Sin(Time.time * 0.2f + Mathf.Cos(Time.time * 0.05f)), 0, Mathf.Sin(Time.time * 0.2f));
        transform.Translate((move_speed + speed_offset * .01f) * Time.deltaTime );

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "cloud_col")
        {
            Vector3 v = Random.insideUnitSphere * 2;
            Vector3 dir = (new Vector3(v.x - transform.position.x, transform.position.y, v.z - transform.position.z)).normalized;
            move_speed = dir * Random.Range(0f, 1f) * max_speed;
        }
    }

    void StartMove()
    {
        Vector2 v = Random.insideUnitCircle;
        move_speed = new Vector3(v.x, 0, v.y) * max_speed;
    }

    void StartFloat()
    {
        y = max_offset_y;
    }

}
