using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpecialEventFx : MonoBehaviour
{
    private ParticleSystem upParticles;
    private ParticleSystem moreDownParticles;
    private ParticleSystem lessDownParticles;
    // Start is called before the first frame update
    void Awake()
    {
        upParticles = this.GetComponentByPath<ParticleSystem>("FX_KD_caidai_lizi (shang)");
        moreDownParticles = this.GetComponentByPath<ParticleSystem>("FX_KD_caidai_lizi (duo）");
        lessDownParticles = this.GetComponentByPath<ParticleSystem>("FX_KD_caidai_lizi (shao)");
    }

    private void OnEnable()
    {
        StartCoroutine(StartParticles());
    }

    private void OnDisable()
    {
        upParticles.gameObject.SetActive(false);
        upParticles.gameObject.SetActive(false);
        lessDownParticles.gameObject.SetActive(false);
    }

    private IEnumerator StartParticles()
    {
        upParticles.gameObject.SetActive(true);
        upParticles.Play();
        yield return new WaitForSeconds(1.37f);
        moreDownParticles.gameObject.SetActive(true);
        moreDownParticles.Play();
        yield return new WaitForSeconds(5.2f);
        lessDownParticles.gameObject.SetActive(true);
        lessDownParticles.Play();
    }
  
}
