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
            StartCoroutine(changeColor(Color.green));
        }

        if (other.CompareTag("Player"))
        {
            _parryAbility.incomings.Remove(this.gameObject);
            StartCoroutine(changeColor(Color.red));
        }
    }
    
    IEnumerator changeColor(Color color)
    {
        _parryAbility.skinMat.SetColor("missed", color);
        yield return new WaitForSeconds(0.5f);
        _parryAbility.skinMat.SetColor("player", _parryAbility.playerColor);

    }
}
