using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryAbility : MonoBehaviour
{
    private PlayerInputs playerInputs;
    public Material skinMat;
    public Material parade;
    public Color playerColor;
    private bool inputPressed;
    private GameObject _player;
    private bool go;
    float countdown;

    public List<GameObject> incomings;
    
    void Start()
    {
        playerInputs = FindObjectOfType<PlayerInputs>();
        _player = GameObject.FindGameObjectWithTag("Player");
        skinMat= _player.GetComponent<Renderer>().material;
        playerColor = skinMat.color;
    }

    private void Update()
    {
        incomings.RemoveAll(item => item == null);
        
        if (!inputPressed && playerInputs.Parry)
        {
            skinMat = parade;
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
            }
            incomings.Clear();
            StartCoroutine(changeColor(Color.yellow));
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

    IEnumerator changeColor(Color newColor)
    {
        skinMat.SetColor("_newcol", newColor);
        yield return new WaitForSeconds(1f);
        skinMat.SetColor("player", playerColor);
    }
}
