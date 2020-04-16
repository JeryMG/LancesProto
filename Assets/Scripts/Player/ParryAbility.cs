using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryAbility : MonoBehaviour
{
    private PlayerInputs playerInputs;
    public Material skinMat;
    public Material playerColor;
    private bool inputPressed;
    private GameObject _player;
    private bool go;
    float countdown;
    public Material parryAvailable;
    public Material parryMissed;
    public Material parrySuccess;

    public List<GameObject> incomings;
    
    void Start()
    {
        playerInputs = FindObjectOfType<PlayerInputs>();
        _player = GameObject.FindGameObjectWithTag("Player");
        skinMat= _player.GetComponent<Renderer>().material;
        playerColor = skinMat;
    }

    private void Update()
    {
        incomings.RemoveAll(item => item == null);
        
        if (!inputPressed && playerInputs.Parry)
        {
            Debug.Log("Update Input");
            inputPressed = true;
        }
        
        if (go)
        {
            countdown += Time.deltaTime;
        }

        if (countdown >= 1)
        {
            _player.GetComponent<Renderer>().material = playerColor;
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
            }
            incomings.Clear();
            colorChange(parrySuccess);
            inputPressed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            print("projectile incoming");
            skinMat = parryAvailable;
            incomings.Add(other.gameObject);
        }
    }

    IEnumerator changeColor(Material newColor)
    {
        skinMat = newColor;
        yield return new WaitForSeconds(1f);
        skinMat = playerColor;
    }

    void colorChange(Material newMat)
    {
        go = true;
        skinMat = newMat;
    }
}
