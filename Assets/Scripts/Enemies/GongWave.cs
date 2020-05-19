﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GongWave : MonoBehaviour
{
    private Hunter _hunter;
    private Vivant targetVie;
    public GameObject GauchePart;
    public GameObject DroitePart;

    [SerializeField] private float damage = 5f;
    
    private void Start()
    {
        _hunter = FindObjectOfType<Hunter>();
        targetVie = _hunter.GetComponent<Vivant>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hikiki");    
        IClochePropag bro = other.gameObject.GetComponent<IClochePropag>();
        
        if (other.gameObject.CompareTag("Player"))
        {
            targetVie.TakeDamage(damage);
        }

        if (other.CompareTag("Enemy"))
        {
            if (bro != null)
            {
                
                Destroy(
                Instantiate(GauchePart, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
                1);
                Destroy(
                Instantiate(DroitePart, transform.position,
                Quaternion.FromToRotation(Vector3.forward, transform.position)),
                1);
        
                Debug.Log("lets go !!!!");
                bro.propagOnde();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/EnnemiDistance3D/Cloches/Ennemi_Cloche",transform.position);
            }
        }
    }
}
