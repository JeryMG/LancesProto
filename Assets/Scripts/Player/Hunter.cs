using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Rigidbody))]
public class Hunter : Vivant
{
    public enum states
    {
        hunter,
        blinker
    }
    
    float timer = 0f;
    
    public Transform Hand;
    public Transform hitBox;
    [SerializeField] private Vector3 hitBoxSize = new Vector3(3, 1.5f, 2);

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

    [Header("Blink Variables")] 
    [SerializeField] public List<Transform> lieuxDeTp;

    [Header("KaK Variables")] 
    [SerializeField] float KakDamage = 2f;

    [Header("Animation")] 
    public PlayAnimHeros animPerso;
    public TrailRenderer _trail;

    [Header("Random")]
    private PlayerInputs _playerInputs;
    private CameraTop90 playerCamera;
    private Rigidbody rb;
    private Respawner respawner;
    [SerializeField] private Vector3 distanceKnockBack = new Vector3(1.5f,0,1.5f);
    private bool dejaJouee;

    private void Awake()
    {
        currentState = states.blinker;
        _playerInputs = GetComponent<PlayerInputs>();
        playerCamera = FindObjectOfType<CameraTop90>();
        respawner = FindObjectOfType<Respawner>();
        rb = GetComponent<Rigidbody>();
        lancesRestantes = nbrLances;

        animPerso = GetComponentInChildren<PlayAnimHeros>();
        
        skinMat= GetComponent<Renderer>().material;
        playerColor = skinMat.color;
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    private void Update()
    {
        if (changeColor)
        {
            ColorChange(Color.green);
        }
        
        //Viser, lancé et melee
        if (lancesRestantes > 0)
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
                
                animPerso.AnimLancer();
            }

            if (_playerInputs.Melee && currentState == states.blinker)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/CAC_Swift", transform.position);

                changeColor = true;
                //playerCamera.ShakeIt();

                Collider[] _hitbox = Physics.OverlapBox(hitBox.position, hitBoxSize / 2);
                foreach (Collider hits in _hitbox)
                {
                    _E_Cac hitable = hits.GetComponent<_E_Cac>();
                    //Rigidbody hitBody = hits.GetComponent<Rigidbody>();
                    NavMeshAgent pathfinder = hits.GetComponent<NavMeshAgent>();
                    
                    if (hitable != null)
                    {
                        //Son CAC quand il touche
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/CAC", transform.position);

                        Debug.Log("hitted");
                        //hitable.StartCoroutine("stun", 0.7f);
                        
                        hitable.TakeDamage(KakDamage);
                        Vector3 direction = hitable.transform.position - transform.position;
                        //playerCamera.Shake(direction, shakeMag, shakeLength);
                        direction.y = hitable.transform.position.y;
                        hitable.transform.position += distanceKnockBack;
                        
                        //hitable.UpdatePath();
                    }
                }
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
                lieuxDeTp[lieuxDeTp.Count - 1].position = new Vector3(lieuxDeTp[lieuxDeTp.Count - 1].position.x, transform.position.y, lieuxDeTp[lieuxDeTp.Count - 1].position.z);
                transform.position = lieuxDeTp[lieuxDeTp.Count - 1].position;

                StartCoroutine(trail());
            }

            if (_playerInputs.LanceReturn && currentState == states.blinker)
            {
                lancesRestantes = 3;
                lieuxDeTp.Clear();
            }
        }

        if (lancesRestantes > 3)
        {
            lancesRestantes = 3;
        }
    }

    private void Move()
    {
        Vector3 movement = new Vector3(_playerInputs.Horizontal, 0, _playerInputs.Vertical);
        Vector3 Velocity = movement.normalized * speed;
        rb.MovePosition(rb.position + Velocity * Time.deltaTime);
        if (movement != Vector3.zero && !dejaJouee)
        {
            animPerso.AnimCourse();
            dejaJouee = true;
        }
        if(movement == Vector3.zero && dejaJouee)
        {
            animPerso.AnimPause();
            dejaJouee = false;
        }
    }

    private void ColorChange(Color newColor)
    {
        timer += Time.deltaTime;
        skinMat.color = newColor;
        if (timer >= 0.5)
        {
            skinMat.color = playerColor;
            changeColor = false;
            timer = 0;
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Vivant _enemi = other.gameObject.GetComponent<Vivant>();

        if (other.gameObject.GetComponent<Lance>() != null)
        {
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
            transform.position = respawner.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            ColorChange(Color.red);
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
        //Gizmos.DrawWireCube(hitBox.position, hitBoxSize);
    }
}
