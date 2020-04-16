using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goproj : MonoBehaviour
{
    private float Speed = 12;
    public bool parable;
    private ParryAbility _parryAbility;
    private GameObject _player;

    private void Start()
    {
        _parryAbility = FindObjectOfType<ParryAbility>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MurInvisible"))
        {
            parable = true;
        }

        if (other.CompareTag("Player"))
        {
            _parryAbility.incomings.Remove(this.gameObject);
            StartCoroutine(changeColor());
        }
    }
    
    IEnumerator changeColor()
    {
        _parryAbility.skinMat = _parryAbility.parryMissed;
        yield return new WaitForSeconds(0.7f);
        _parryAbility.skinMat = _parryAbility.playerColor;

    }
}
