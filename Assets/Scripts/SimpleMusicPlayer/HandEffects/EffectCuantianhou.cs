using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCuantianhou : MonoBehaviour {

    public float speed = 2f;

    Rigidbody rgd;

    public Transform forward_achor;

    bool istart;

	// Use this for initialization
	void Start () {
        rgd = GetComponentInChildren<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
        if (rgd != null)
        {
            if (istart)
            {
                rgd.velocity = forward_achor.up * speed;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + "enter");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.name);
        if (other.name == "huochai")
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                istart = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.name + "exi");
    }
}
