using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateWithAudio : MonoBehaviour {

    public float _rotateSpeed = 5;

	void Update () {
        transform.Rotate(new Vector3(0, _rotateSpeed * Time.deltaTime * (1 + MusicPlayer.Instance.SamplesSum * 4) , 0));
	}
}
