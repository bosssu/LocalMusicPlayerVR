using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Effect_Auto_Recyle : MonoBehaviour {
    // If true, deactivate the object instead of destroying it
    public bool OnlyDeactivate;

    public float recycle_dur = 2f;

    void OnEnable()
    {
        AudioSource src = GetComponent<AudioSource>();
        if (src.clip != null) recycle_dur = src.clip.length;

        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        ParticleSystem ps = this.GetComponent<ParticleSystem>();

        while (true && ps != null)
        {
            yield return new WaitForSeconds(recycle_dur);
            if (!ps.IsAlive(true))
            {
                if (OnlyDeactivate)
                {
#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
#else
                    this.gameObject.SetActive(false);
#endif
                }
                else
                    MusicPlayerManager.Instance.SpawnPool.Despawn(this.transform);
                break;
            }
        }
    }
}
