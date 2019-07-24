using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

    public Space space = Space.World;

    public enum Axis { x,y,z}

    public Axis _axis = Axis.y;

    public float _rotateSpeed = 5;

	void Update () {

        float rotate_amount = _rotateSpeed * Time.deltaTime;

        switch (_axis)
        {
            case Axis.x:
                transform.Rotate(new Vector3(rotate_amount, 0, 0), space);
                break;
            case Axis.y:
                transform.Rotate(new Vector3(0, rotate_amount, 0), space);
                break;
            case Axis.z:
                transform.Rotate(new Vector3(0, 0, rotate_amount), space);
                break;
            default:
                break;
        }
	}
}
