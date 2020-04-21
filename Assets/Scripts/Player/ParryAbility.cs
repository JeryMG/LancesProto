using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hunter), typeof(PlayerInputs))]
public class ParryAbility : MonoBehaviour
{
    private PlayerInputs playerInputs;
    private bool inputPressed;
    private GameObject _player;
    private bool go;
    float countdown;

    public List<GameObject> incomings;
    
    void Start()
    {
        playerInputs = FindObjectOfType<PlayerInputs>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        incomings.RemoveAll(item => item == null);
        
        if (!inputPressed && playerInputs.Parry)
        {
            Debug.Log("Update Input");
            inputPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if (inputPressed && incomings.Count != 0)
        {
            Debug.Log("Fixed Update : Input consumed");
            for (int i = 0; i < incomings.Count; i++)
            {
                Destroy(incomings[i]);
                //son parade
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event2D/Joueur/Parade");
            }
            incomings.Clear();
            inputPressed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            print("projectile incoming");
            incomings.Add(other.gameObject);
        }
    }
}
