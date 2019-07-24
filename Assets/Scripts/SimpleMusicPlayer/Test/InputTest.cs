using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {

    public bool one;
    public bool two;
    public bool three;
    public bool four;
    public bool pri_index_trigger;
    public bool sec_index_trigger;
    public bool pri_thump;
    public bool sec_thump;
    public bool pri_grip;
    public bool sec_grip;

    public float pri_index_trigger_1d;
    public Vector2 pri_thumbstic_2d;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        one = OVRInput.Get(OVRInput.Button.One);
        two = OVRInput.Get(OVRInput.Button.Two);
        three = OVRInput.Get(OVRInput.Button.Three);
        four = OVRInput.Get(OVRInput.Button.Four);

        pri_index_trigger = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
             sec_index_trigger = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        pri_thump = OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
        sec_thump = OVRInput.Get(OVRInput.Button.SecondaryThumbstick);
        pri_grip = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        sec_grip = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);

        pri_index_trigger_1d = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        pri_thumbstic_2d = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

    }
}
