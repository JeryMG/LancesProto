using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimGong : MonoBehaviour
{
    public Animator anim;



    public void Marche()
    {
        ResetLayer();
        anim.SetLayerWeight(0,1);
        anim.Play("Marche", 0,0f); 
    }
    public void animCac()
    {
        ResetLayer();
        anim.SetLayerWeight(1,1);
        anim.SetLayerWeight(2,1);
        anim.Play("CacCorps", 1,0f);       
        anim.Play("CacArme", 2,0f);       


    }
    public void AnimCac90()
    {
        ResetLayer();
        anim.SetLayerWeight(3,1);
        anim.Play("Cac90", 3,0f);
    }
    public void AnimCac180()
    {
        ResetLayer();
        anim.SetLayerWeight(4,1);
        anim.Play("Cac180", 4,0f);
    }


    public void GongActiver()
    {
        ResetLayer();
        anim.SetLayerWeight(5,1);
        anim.Play("DongCorps", 5,0f);       
    }

    private void ResetLayer()
    {
        for (int i = 1; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i,0);
            }
    }
}
