using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputs))]
public class Hunter : Vivant
{
    public enum states
    {
        hunter,
        blinker
    }

    public float cdLances = 15f;
    
    public Transform Hand;

    public Lance lancePrefab;
    public int lancesRestantes;
    public states currentState;
    private Color playerColor;
    private bool changeColor;
    private Material skinMat;
    [SerializeField] private float speed = 5f;

    [Header("Hunter Variables")]
    private int nbrLances = 3;
    private Lance lanceEquiped;
    public bool lanceReturning;

    [Header("Blink Variables")] 
    [SerializeField] public List<Transform> lieuxDeTp;

    [Header("Animation")]
    public TrailRenderer _trail;

    [Header("Random")]
    private PlayerInputs _playerInputs;
    private Rigidbody rb;
    private Respawner respawner;
    private bool dejaJouee;
    private float yMax;
    private bool grounded;


    private void Awake()
    {
        currentState = states.blinker;
        _playerInputs = GetComponent<PlayerInputs>();
        respawner = FindObjectOfType<Respawner>();
        rb = GetComponent<Rigidbody>();
        lancesRestantes = nbrLances;

        skinMat= GetComponent<Renderer>().material;
        playerColor = skinMat.color;

        OnDeath += playDeathSound;
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    private void Update()
    {
        ClampHauteurJoueur();
        
        //Viser, lancé et melee
        if (lancesRestantes > 0 && !lanceReturning)
        {
            if (_playerInputs.AimWeapon && currentState == states.blinker)
            {
                //Son mélée
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event2D/Lance/Visee");
                
                Lance newLance = Instantiate(lancePrefab, Hand.position, Hand.rotation, Hand);
                currentState = states.hunter;
                lanceEquiped = newLance;
            }

            if (_playerInputs.FireWeapon && currentState == states.hunter)
            {
                //Son Lancé
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/Lance",transform.position);
                
                currentState = states.blinker;
                lanceEquiped.transform.parent = null;
                lanceEquiped.stop = false;
                lanceEquiped.StayImmobile(false);
                lancesRestantes--;
            }
        }

        if (lancesRestantes < 3)
        {
            if (_playerInputs.blink && currentState == states.blinker)
            {
                // Vector3 direction = lieuxDeTp[0] - transform.position;
                // direction = new Vector3(direction.x, 0, direction.z);
                
                //Son de teleportation
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/Téléportation",
                    transform.position);
                
                //tp
                lieuxDeTp[lieuxDeTp.Count - 1].position = new Vector3(lieuxDeTp[lieuxDeTp.Count - 1].position.x, /*transform.position.y*/ lieuxDeTp[lieuxDeTp.Count - 1].position.y + 1, lieuxDeTp[lieuxDeTp.Count - 1].position.z);
                transform.position = lieuxDeTp[lieuxDeTp.Count - 1].position;
                

                StartCoroutine(trail());
            }

            if (_playerInputs.LanceReturn && currentState == states.blinker)
            {
                lieuxDeTp.Clear();
                lanceReturning = true;
            }
        }

        if (lancesRestantes >= 3)
        {
            lancesRestantes = 3;
            lanceReturning = false;
        }
    }
    
    private void Move()
    {
        Vector3 movement = new Vector3(_playerInputs.Horizontal, 0, _playerInputs.Vertical);
        Vector3 Velocity = movement.normalized * speed;
        rb.MovePosition(rb.position + Velocity * Time.deltaTime);
    }

    private void ClampHauteurJoueur()
    {
        if (transform.position.y >= yMax + 1)
        {
            transform.position = new Vector3(transform.position.x, yMax, transform.position.z);
        }
    }

    private void playDeathSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Event2D/Joueur/Mort");
    }

    private void OnCollisionEnter(Collision other)
    {
        Vivant _enemi = other.gameObject.GetComponent<Vivant>();

        if (other.gameObject.GetComponent<Lance>() != null)
        {
            if (other.gameObject.GetComponent<Lance>().returning)
            {
                lanceReturning = false;
            }
            Destroy(other.gameObject);
            //Son de lance Ramassée
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/ObjetRamassé",
                transform.position);


            if (lieuxDeTp.Count != 0)
            {
                lieuxDeTp.Remove(lieuxDeTp[lieuxDeTp.Count - 1]);
            }
            lieuxDeTp.RemoveAll(item => item == null);
            lancesRestantes++;
        }

        if (_enemi != null)
        {
            Debug.Log("mes hp : " + health);
        }

        if (other.gameObject.CompareTag("Vide"))
        {
            rb.velocity = Vector3.zero;
            transform.position = respawner.transform.position;
        }

        if (other.transform.CompareTag("Ground") && !grounded)
        {
            grounded = true;
            yMax = transform.position.y;
        }
    }

    IEnumerator trail()
    {
        _trail.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _trail.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, rayonLance);
    }
}
