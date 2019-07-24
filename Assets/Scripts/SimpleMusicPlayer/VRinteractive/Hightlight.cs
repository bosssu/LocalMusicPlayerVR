using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OVRGrabbable))]
public class Hightlight : MonoBehaviour {

    Material mat;

    Color start_color;

    private void Start()
    {
        mat = GetComponentInChildren<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<OVRGrabber>() != null)
        {
            start_color = mat.color;
            //mat.SetFloat("_OutlineWidth", 0.015f);
            mat.color *= 1.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<OVRGrabber>() != null)
        {
            //mat.SetFloat("_OutlineWidth", 0f);
            mat.color = start_color;
        }
    }
}
