﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Lance : MonoBehaviour
{
    [SerializeField]private float lanceSpeed = 12f;
    private PlayerInputs _inputs;
    public bool stop;
    private Rigidbody lanceBody;
    private Hunter _player;
    public float lanceDamage = 5f;
    [SerializeField] private float timerDestruction = 5f;

    private void Start()
    {
        lanceBody = GetComponent<Rigidbody>();
        _inputs = FindObjectOfType<PlayerInputs>();
        StayImmobile(true);
        stop = true;
        _player = FindObjectOfType<Hunter>();
    }

    private void Update()
    {
        if (!stop)
        {
            shooting();
            
            if (_inputs.blink)
            {
                lanceBody.velocity = Vector3.zero;
                stop = true;
                StayImmobile(true);
                if (!_player.lieuxDeTp.Contains(transform))
                {
                    _player.lieuxDeTp.Add(transform);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag!= "Player" && !_player.aiming)
        {
            lanceBody.velocity = Vector3.zero;
            stop = true;
            StayImmobile(true);
            Debug.Log("cible ajoutée");
            if (!_player.lieuxDeTp.Contains(transform))
            {
                _player.lieuxDeTp.Add(transform);
            }
        }

        if (other.gameObject.CompareTag("Vide"))
        {
            Destroy(gameObject,timerDestruction);
        }

        Enemi enemiBody = other.gameObject.GetComponent<Enemi>();
        if (enemiBody != null)
        {
            enemiBody.TakeDamage(lanceDamage);
            transform.parent = enemiBody.transform;
        }
    }

    private void OnDestroy()
    {
        if (_player.lieuxDeTp.Contains(transform))
        {
            _player.lieuxDeTp.Remove(transform);
            _player.lancesRestantes++;
        }
    }

    public void shooting()
    {
        transform.Translate(Vector3.forward * lanceSpeed * Time.deltaTime);
    }

    public void StayImmobile(bool yes)
    {
        if (yes)
        {
            lanceBody.useGravity = false;
            lanceBody.isKinematic = true;
        }
        if (!yes)
        {
            lanceBody.useGravity = true;
            lanceBody.isKinematic = false;
        }
    }
}
