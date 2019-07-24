using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//scale
[RequireComponent(typeof(OVRGrabber))]
public class OVRGrabberInteractiveEx : MonoBehaviour {

    public OVRInput.Controller controller;

    OVRGrabber grabber;

    Vector3 start_scale;

    float scale_multper = 1f;
    public float scale_speed = 5f;

    public bool is_start_scale;

    Vector3 hand_object_start_grab_localoffset;

    GameObject grabable_touched;

	void Start () {
        grabber = GetComponent<OVRGrabber>();
    }

    private void Update()
    {
        OVRGrabbable grabbedObject = grabber.grabbedObject;
        if (grabbedObject != null)
        {
            Vector2 thumbstick = Vector2.zero;
            if (Application.platform == RuntimePlatform.Android)
            {
                thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);
            }
            else
            {
                thumbstick.y =Input.GetAxis("Vertical");
            }

            if (!is_start_scale)
            {
                is_start_scale = true;

                start_scale = grabbedObject.transform.localScale;
                hand_object_start_grab_localoffset = transform.InverseTransformVector(grabbedObject.transform.position - transform.position);
                scale_multper = 1f;

            }
            else
            {
                scale_multper += scale_speed * Time.deltaTime * thumbstick.y * .1f;
                scale_multper = Mathf.Clamp(scale_multper, .1f, 10f);
                Vector3 target_scale = start_scale * scale_multper;
                grabbedObject.transform.localScale = target_scale;
                grabbedObject.transform.position = transform.position + transform.TransformVector(hand_object_start_grab_localoffset) * scale_multper;
            }

        }
        else
        {
            is_start_scale = false;
        }


        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            if (grabbedObject != null)
            {
                if (grabbedObject.gameObject != null)
                {
                    List<GameObject> _interactive_objects = InteractiveObjectManager.Instance._interactive_objects;
                    if (_interactive_objects.Contains(grabbedObject.gameObject))
                        _interactive_objects.Remove(grabbedObject.gameObject);
                    Destroy(grabbedObject.gameObject);
                }
            }
        }

    }

    //void OnTriggerEnter(Collider otherCollider)
    //{
    //    OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>();
    //    if (grabbable == null) return;

    //    grabable_touched = grabbable.gameObject;

    //}

    //void OnTriggerExit(Collider otherCollider)
    //{
    //    OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>();
    //    if (grabbable == null) return;

    //    grabable_touched = null;
    //}
}
