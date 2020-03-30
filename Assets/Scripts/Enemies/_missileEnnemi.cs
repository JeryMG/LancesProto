using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _missileEnnemi : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 2f;

    private Rigidbody flecheBody;

    private void Start()
    {
        Hunter target = FindObjectOfType<Hunter>();
        Destroy(gameObject,2f);
        flecheBody = GetComponent<Rigidbody>();
        transform.LookAt(target.transform);
        flecheBody.AddForce(speed * transform.forward,ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vivant playerEntity = other.gameObject.GetComponent<Vivant>();
        
        if (playerEntity != null)
        {
            playerEntity.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
