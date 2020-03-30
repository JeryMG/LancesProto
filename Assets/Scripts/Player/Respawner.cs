using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    private Hunter playerTransform;
    private float clampedY;

    void Start()
    {
        playerTransform = FindObjectOfType<Hunter>();
        clampedY = playerTransform.transform.position.y;
    }
    
    void Update()
    {
        Ray rayonDown = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(playerTransform.transform.position, -transform.up, 15, LayerMask.GetMask("Ground")))
        {
            var position = playerTransform.transform.position;
            clampedY = Mathf.Clamp(transform.position.y, position.y, position.y);
            transform.position = new Vector3(position.x, clampedY, position.z);
        }
    }
}
