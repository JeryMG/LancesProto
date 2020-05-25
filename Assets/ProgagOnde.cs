using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgagOnde : Vivant, IClochePropag
{
    public GameObject Onde;
    public Animator gongWaveAnimator;
    public Animator animAnimator;
    public bool go;


    private void Update()
    {
        if (go)
        {
            propagOnde();
        }
    }

    public void propagOnde()
    {
        Onde.gameObject.SetActive(true);

        gongWaveAnimator.SetTrigger("Elargi");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Ennemi_Cloche", transform.position);
        go = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WaveOri") || other.gameObject.CompareTag("Wave"))
        {
            go = true;
        }
    }
}
