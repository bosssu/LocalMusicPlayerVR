using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchController : MonoBehaviour {

    [SerializeField]
    private OVRInput.Controller m_controller;

    private void Start()
    {
        //OVRManager.InputFocusAcquired += OnInputFocusAcquired;
        //OVRManager.InputFocusLost += OnInputFocusLost;
    }

    private void OnInputFocusLost()
    {
        gameObject.SetActive(false);
    }

    private void OnInputFocusAcquired()
    {
        gameObject.SetActive(true);
    }
}
