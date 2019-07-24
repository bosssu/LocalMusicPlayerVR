using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationMono : MonoBehaviour {

	void Start () {

        Text text = GetComponent<Text>();
        if (!string.IsNullOrEmpty(text.text) && LocalizationManager.Instance.IsCurrentValueExistKey(text.text))
        {
            string v = LocalizationManager.Instance.GetCurrentValueWithKey(text.text);
            if (!string.IsNullOrEmpty(v))
                text.text = v;
        }

	}
	
}
