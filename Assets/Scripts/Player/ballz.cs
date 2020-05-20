using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballz : MonoBehaviour
{
    private Hunter player;
    private PlayerInputs inputs;
    public List<GameObject> balls;
    private Animator ballzAnim;
    public int lancesNumber;

    private void Start()
    {
        player = GetComponentInParent<Hunter>();
        ballzAnim = GetComponent<Animator>();
        inputs = FindObjectOfType<PlayerInputs>();
        
        for (int i = 0; i < 3; i++)
        {
            balls[i].SetActive(true);
        }
    }

    private void Update()
    {
        lancesNumber = player.lancesRestantes;
        if (player.lancesRestantes == 3)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].SetActive(true);
            }
        }
        
        /*if (lancesNumber == 3)
        {
            ballzAnim.SetBool("lance1", true);
        }
        if (inputs.AimWeapon && lancesNumber == 2)
        {
            ballzAnim.SetBool("lance2", true);
        }
        if (inputs.AimWeapon && lancesNumber == 1)
        {
            ballzAnim.SetBool("lance3", true);
        }*/

        /*if (inputs.FireWeapon)
        {
            ballzAnim.SetBool("lance1", false);
            if (player.lancesRestantes == 2)
            {
                balls[0].SetActive(false);
            }
            ballzAnim.SetBool("lance2", false);
            if (player.lancesRestantes == 1)
            {
                balls[1].SetActive(false);
            }
            ballzAnim.SetBool("lance3", false);
            if (player.lancesRestantes == 0)
            {
                balls[2].SetActive(false);
            }
            
        }*/
    }
}
