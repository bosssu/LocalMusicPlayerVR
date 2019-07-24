using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBase {

    protected Transform win_root;

    public virtual void Init() { }

    public virtual void Open() {
        win_root.gameObject.SetActive(true);
    }

    public virtual void Update() { }

    public virtual void Close() {
        win_root.gameObject.SetActive(false);
    }

}
