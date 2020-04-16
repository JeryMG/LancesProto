using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_EnenmieCac : MonoBehaviour
{
    public Animator anim;
    private void Start() {
        MarcheRapide();
    }
    public void Repos()
    {
        ResetLayer();
        anim.SetLayerWeight(0,1);
        anim.Play("Pause", 0,0f); 
    }
    public void Attaque()
    {
        ResetLayer();
        anim.SetLayerWeight(1,1);
        anim.Play("Attaque", 1,0f);            
    }
    public void OndeRecus()
    {
        ResetLayer();
        anim.SetLayerWeight(2,1);
        anim.Play("EchoOnde",2,0f);       
    }
    public void MarcheLente()
    {
        ResetLayer();
        anim.SetLayerWeight(3,1);
        anim.Play("MarcheLente", 3,0f);
    }
    public void MarcheRapide()
    {
        ResetLayer();
        anim.SetLayerWeight(4,1);
        anim.Play("MarcheRapide", 4,0f);
    }
    public void DegasRecus()
    {
        ResetLayer();
        anim.SetLayerWeight(5,1);
        anim.Play("DgtRecus",5,0f);
    }

    private void ResetLayer()
    {
        for (int i = 1; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i,0);
            }
    }
}
