using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPulseObjects : MonoBehaviour {

    public float invoke_repeat_dur = 1f;

    GameObject template;

    float start_distance;

	// Use this for initialization
	void Start () {
        template = transform.Find("trail").gameObject;
        InvokeRepeating("CreateTrail", 0f, invoke_repeat_dur);
        start_distance = Vector3.Distance(template.transform.position, transform.position);

    }

    void CreateTrail()
    {
        GameObject g = Instantiate<GameObject>(template, transform);
        Vector2 dir = Random.insideUnitCircle.normalized;
        g.transform.position = transform.position + new Vector3(dir.x, 0, dir.y) * start_distance;
        g.GetComponent<TrailRenderer>().Clear();
        g.SetActive(true);
    }
}
