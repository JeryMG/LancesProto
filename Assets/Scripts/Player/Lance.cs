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
    public bool returning;
    private Transform PlayerTransform;


    public float vitesseReturn;
    //private bool isFlying = false;

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lanceBody = GetComponent<Rigidbody>();
        _inputs = FindObjectOfType<PlayerInputs>();
        StayImmobile(true);
        stop = true;
        _player = FindObjectOfType<Hunter>();
    }

    private void Update()
    {
        if (!stop && !returning)
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

            // if (isFlying && _inputs.blink)
            // {
            //     GameSystem.Instance.StartCoroutine(GameSystem.Instance.SlowMotion());
            // }
        }
        
        returningToPlayer();

        if (_inputs.LanceReturn)
        {
            // son attraction
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event2D/Joueur/Attraction");
            returning = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag!= "Player" /*&& !_player.aiming*/)
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

        if (other.gameObject.CompareTag("Enemy"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Ennemi",transform.position);
        }

        if (other.gameObject.CompareTag("Vide"))
        {
            Destroy(gameObject,timerDestruction);
        }

        if (other.gameObject.CompareTag("Mur"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Mur",transform.position);
        }
        
        if (other.gameObject.CompareTag("Ground"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Sol",transform.position);
        }

        Vivant enemiBody = other.gameObject.GetComponent<Vivant>();
        if (enemiBody != null && other.gameObject.CompareTag("Enemy"))
        {
            enemiBody.TakeDamage(lanceDamage);
            if (!returning)
            {
                transform.parent = enemiBody.transform;
            }
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
        //isFlying = true;
    }

    public void returningToPlayer()
    {
        if (returning)
        {
            StayImmobile(false);
            Vector3 destination = PlayerTransform.position - transform.position;
            transform.position += destination * vitesseReturn * Time.deltaTime;
        }
    }

    public void StayImmobile(bool yes)
    {
        if (yes)
        {
            lanceBody.useGravity = false;
            lanceBody.isKinematic = true;
            //isFlying = true;
        }
        if (!yes)
        {
            lanceBody.useGravity = true;
            lanceBody.isKinematic = false;
        }
    }
}
