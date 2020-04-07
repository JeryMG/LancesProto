using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInputs : MonoBehaviour
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    
    public float OrientationVerticale { get; private set; }
    
    public float OrientationHorizontal { get; private set; }

    public bool FireWeapon { get; private set; }
    
    public event Action OnFire = delegate {  };

    private Camera mainCamera;
    public bool AimWeapon { get; private set; }
    public bool blink { get; private set; }
    public bool Melee { get; private set; }
    public bool LanceReturn { get; private set; }
    public bool Parry { get; private set; }
    
    public bool manetteInputs;
    public bool InputsPS4;
    public bool InputsXbox;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        //player mouvement horizontal et vertical
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
        
        //Inputs Souris et clavier
        if (!manetteInputs)
        {
            AimWeapon = Input.GetMouseButton(0);
            FireWeapon = Input.GetMouseButtonUp(0);
            Melee = Input.GetKeyDown(KeyCode.Space);
            blink = Input.GetMouseButtonDown(1);
            LanceReturn = Input.GetKeyDown(KeyCode.R);
            Parry = Input.GetKeyDown(KeyCode.F);
        }
        //Inputs Manettes
        else
        {
            if (InputsPS4)
            {
                OrientationHorizontal = Input.GetAxis("Ps4Horizontale");
                OrientationVerticale = Input.GetAxis("Ps4Verticale");
                AimWeapon = Input.GetButton("TireLance");
                FireWeapon = Input.GetButtonUp("TireLance");
                blink = Input.GetButtonDown("Blink");
                Melee = Input.GetButtonDown("Melee");
                LanceReturn = Input.GetButtonDown("LanceReturn");
                Parry = Input.GetButtonDown("Parry");
            }
            if(InputsXbox)
            {
                OrientationHorizontal = Input.GetAxis("XboxHorizontale");
                OrientationVerticale = Input.GetAxis("XboxVerticale");
                AimWeapon = Input.GetKey(KeyCode.Joystick1Button5);
                FireWeapon = Input.GetKeyUp(KeyCode.Joystick1Button5);
                blink = Input.GetKeyDown(KeyCode.Joystick1Button4);
                // Melee = Input.GetButtonDown("Melee");
                LanceReturn = Input.GetKeyDown(KeyCode.Joystick1Button2);
            }
        }

        //Look inputs
        
        //souris et clavier
        if (!manetteInputs)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                //Debug.DrawLine(ray.origin, point,Color.red);
                LookAt(point);
            }
        }
        //manettes
        else
        {
            Vector3 moveDirection = new Vector3(OrientationHorizontal, 0, OrientationVerticale);
            transform.LookAt(transform.position + moveDirection, Vector3.up);
        }
        
        //Tir event
        if (FireWeapon)
        {
            if (OnFire != null)
            {
                OnFire();
            }
        }

        // switch entre anettes controls et souris
        if (Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            manetteInputs = true;
            InputsPS4 = true;
            InputsXbox = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            manetteInputs = false;
            InputsPS4 = false;
            InputsXbox = false;
        }
        
        if (Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            manetteInputs = true;
            InputsPS4 = false;
            InputsXbox = true;
        }
    }
    
    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectionPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectionPoint);
    }
}
