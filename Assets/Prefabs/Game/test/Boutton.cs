using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boutton : MonoBehaviour
{
    public Vector3 respawnPos;
    public GameObject levelActuel;
    public GameObject levelSuivant;
    private Renderer bouttonRenderer;


    private void Start()
    {
        bouttonRenderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = respawnPos;
            levelActuel.SetActive(false);
            levelSuivant.SetActive(true);
            bouttonRenderer.material.shader = Shader.Find("_BaseColor");
            bouttonRenderer.material.SetColor("_BaseColor", Color.red);
        }
    }
}
