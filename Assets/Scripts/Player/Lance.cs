using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Lance : MonoBehaviour
{
    [Header("Lance Speed")]
    [SerializeField]private float lanceSpeed = 12f;
    private float vitesseReturn = 0f;
    [SerializeField]private float acceleration = 1.0f;
    [SerializeField]private float maxSpeed = 60.0f;

    private PlayerInputs _inputs;
    public bool stop;
    private Rigidbody lanceBody;
    private Hunter _player;
    public float lanceDamage = 5f;
    [SerializeField] private float timerDestruction = 5f;
    public bool returning;
    [SerializeField] private float distanceReturn = 3f;
    private Transform PlayerTransform;
    

    //private bool isFlying = false;

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lanceBody = GetComponent<Rigidbody>();
        _inputs = FindObjectOfType<PlayerInputs>();
        StayImmobile(true);
        stop = true;
        _player = FindObjectOfType<Hunter>();
        Destroy(gameObject,5f);
    }

    private void Update()
    {
        if (_player != null)
        {
            if (!stop && !returning)
            {
                shooting();
                if (!_player.lieuxDeTp.Contains(transform))
                {
                    _player.lieuxDeTp.Add(transform);
                }
                // if (_inputs.blink)
                // {
                //     if (!_player.lieuxDeTp.Contains(transform))
                //     {
                //         _player.lieuxDeTp.Add(transform);
                //     }
                // }
            }
        
            returningToPlayer();

            if (_inputs.LanceReturn && _player.currentState == Hunter.states.blinker)
            {
                // son attraction
                RuntimeManager.PlayOneShot("event:/Event2D/Joueur/Attraction");
                returning = true;
            }
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

        if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<E_Gong>() == null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Ennemi",transform.position);
        }

        if (other.gameObject.CompareTag("Vide"))
        {
            Destroy(gameObject,timerDestruction);
        }
        if(other.gameObject.GetComponent<E_Gong>()!=null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Armure",transform.position);
        }

        if (other.gameObject.CompareTag("Mur"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Mur",transform.position);
        }
        
        if (other.gameObject.CompareTag("Ground"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Lance3D/Sol",transform.position);
        }

        E_Gong Egong = other.gameObject.GetComponent<E_Gong>();
        Vivant enemiBody = other.gameObject.GetComponent<Vivant>();
        if (enemiBody != null && other.gameObject.CompareTag("Enemy"))
        {
            enemiBody.TakeDamage(lanceDamage);
            if (!returning)
            {
                transform.parent = enemiBody.transform;
            }
        }
        
        Lance lance = other.gameObject.GetComponent<Lance>();

        if (Egong != null)
        {
            Egong.stunned = true;
            StartCoroutine( Egong.StunXseconds(5f));
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
            this.transform.LookAt(PlayerTransform);
            vitesseReturn += acceleration * Time.deltaTime;
 
            if (vitesseReturn > maxSpeed)
                vitesseReturn = maxSpeed;

            if (Vector3.Distance(transform.position, PlayerTransform.position) < distanceReturn)
            {
                Destroy(gameObject);
                _player.lanceReturning = false;
            }
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
