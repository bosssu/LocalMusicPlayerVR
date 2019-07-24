using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoSingleton<PlayerInputManager> {

    protected override void Init()
    {
        base.Init();
    }

    void Update () {

        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            UIManager.Instance.OnShowHideClick();
        }

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            HandEffectManager.Instance.NextEffect();
        }

        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            UIManager.Instance.OnInteractiveUIShowHideClick();
        }

        if (OVRInput.GetUp(OVRInput.Button.Four))
        {
            EffectManager.Instance.NextEffect();
        }

        if (Input.GetKeyDown(KeyCode.B))
            HandEffectManager.Instance.NextEffect();
        if (Input.GetKeyDown(KeyCode.N))
            EffectManager.Instance.NextEffect();
        if (Input.GetKeyDown(KeyCode.H))
            UIManager.Instance.OnShowHideClick();


        if (Input.GetKeyDown(KeyCode.F1))
            UIManager.Instance.OnInteractiveUIShowHideClick();

        if (Input.GetKeyDown(KeyCode.R))
        {
            EffectManager.Instance.ReLoadEffect();
            EnvManager.Instance.ResetEnvTrans();
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
