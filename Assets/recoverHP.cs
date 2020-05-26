using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recoverHP : MonoBehaviour
{
    [SerializeField] private float amount = 6;

    private void OnTriggerEnter(Collider other)
    {
        Hunter entityPlayer = other.GetComponent<Hunter>();
        if (other.gameObject.CompareTag("Player") && entityPlayer != null)
        {
            entityPlayer.RecoverHP(amount);
            Destroy(gameObject);
        }
    }
}
