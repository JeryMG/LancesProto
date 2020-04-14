using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public bool InputsPS4 = true;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        //player inputs
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
        if (!manetteInputs)
        {
            AimWeapon = Input.GetMouseButton(0);
            FireWeapon = Input.GetMouseButtonUp(0);
            Melee = Input.GetKeyDown(KeyCode.Space);
            blink = Input.GetMouseButtonDown(1);
            LanceReturn = Input.GetKeyDown(KeyCode.R);
            Parry = Input.GetKeyDown(KeyCode.F);
        }
        else
        {
            if (InputsPS4)
            {
                OrientationHorizontal = Input.GetAxis("ViseeHorizontale");
                OrientationVerticale = Input.GetAxis("ViseeVerticale");
                AimWeapon = Input.GetButton("TireLance");
                FireWeapon = Input.GetButtonUp("TireLance");
                blink = Input.GetButtonDown("Blink");
                Melee = Input.GetButtonDown("Melee");
                LanceReturn = Input.GetButtonDown("LanceReturn");
                Parry = Input.GetButtonDown("Parry");
            }
            else
            {
                OrientationHorizontal = Input.GetAxis("XboxHorizontale");
                OrientationVerticale = Input.GetAxis("XboxVerticale");
                AimWeapon = Input.GetButton("TireLance");
                FireWeapon = Input.GetButtonUp("TireLance");
                blink = Input.GetButtonDown("Blink");
                Melee = Input.GetButtonDown("Melee");
                LanceReturn = Input.GetButtonDown("LanceReturn");
            }
        }

        //Look inputs
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
        else
        {
            Vector3 moveDirection = new Vector3(OrientationHorizontal, 0, OrientationVerticale);
            transform.LookAt(transform.position + moveDirection, Vector3.up);
        }
        
        //Tir
        if (FireWeapon)
        {
            OnFire();
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            manetteInputs = !manetteInputs;
        }
    }
    
    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectionPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectionPoint);
    }
}
