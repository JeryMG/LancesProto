using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputs))]
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
    [SerializeField] private Vector3 hitBoxSize;

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
    [SerializeField] public List<Vector3> lieuxDeTp;
    private float dashSpeed = 1f;
    
    [Header("KaK Variables")] 
    [SerializeField] float KakDamage = 2f;
    [SerializeField] float shakeMag = 2f;
    [SerializeField] float shakeLength = 0.7f;

    [Header("Random")]
    private PlayerInputs _playerInputs;
    private CameraTop90 playerCamera;
    private Rigidbody rb;
    [SerializeField] private Vector3 distanceKnockBack = new Vector3(1.5f,0,1.5f);
    public bool aiming;

    private void Awake()
    {
        currentState = states.blinker;
        _playerInputs = GetComponent<PlayerInputs>();
        playerCamera = FindObjectOfType<CameraTop90>();
        rb = GetComponent<Rigidbody>();
        lancesRestantes = nbrLances;

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
            ColorChange();
        }
        
        //Viser, lancé et melee
        if (lancesRestantes > 0)
        {
            if (_playerInputs.AimWeapon && currentState == states.blinker)
            {
                aiming = true;
                Lance newLance = Instantiate(lancePrefab, Hand.position, Hand.rotation, Hand);
                currentState = states.hunter;
                lanceEquiped = newLance;
            }

            if (_playerInputs.FireWeapon && currentState == states.hunter)
            {
                //Son Lancé
                FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/Lancé",
                    transform.position);

                
                aiming = false;
                currentState = states.blinker;
                lanceEquiped.transform.parent = null;
                lanceEquiped.stop = false;
                lanceEquiped.StayImmobile(false);
                lancesRestantes--;
            }

            if (_playerInputs.Melee && currentState == states.blinker)
            {
                changeColor = true;
                Collider[] _hitbox = Physics.OverlapBox(hitBox.position, hitBoxSize / 2);
                foreach (Collider hits in _hitbox)
                {
                    Enemi hitable = hits.GetComponent<Enemi>();
                    Rigidbody hitBody = hits.GetComponent<Rigidbody>();
                    NavMeshAgent pathfinder = hits.GetComponent<NavMeshAgent>();
                    
                    if (hitable != null)
                    {
                        Debug.Log("hitted");
                        hitable.StartCoroutine("stun", 0.7f);
                        
                        hitable.TakeDamage(KakDamage);
                        Vector3 direction = hitBody.position - transform.position;
                        playerCamera.Shake(direction, shakeMag, shakeLength);
                        direction.y = hitable.transform.position.y;
                        hitable.transform.position += distanceKnockBack;
                        
                        hitable.UpdatePath();
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

                
                lieuxDeTp[lieuxDeTp.Count - 1] = new Vector3(lieuxDeTp[lieuxDeTp.Count - 1].x, transform.position.y, lieuxDeTp[lieuxDeTp.Count - 1].z);
                transform.position = lieuxDeTp[lieuxDeTp.Count - 1];
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
    }

    private void ColorChange()
    {
        timer += Time.deltaTime;
        skinMat.color = Color.green;
        if (timer >= 1)
        {
            skinMat.color = playerColor;
            changeColor = false;
            timer = 0;
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Enemi _enemi = other.gameObject.GetComponent<Enemi>();
        
        if (other.gameObject.GetComponent<Lance>() != null)
        {
            Destroy(other.gameObject);
            //Son de lance Ramassée
            FMODUnity.RuntimeManager.PlayOneShot("event:/Event3D/Joueur3D/ObjetRamassé",
                transform.position);

            
            lieuxDeTp.Remove(lieuxDeTp[lieuxDeTp.Count - 1]);
            lieuxDeTp.RemoveAll(item => item == null);
            lancesRestantes++;
        }

        if (_enemi != null)
        {
            Debug.Log("mes hp : " + health);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(hitBox.position, hitBoxSize);
    }
}
