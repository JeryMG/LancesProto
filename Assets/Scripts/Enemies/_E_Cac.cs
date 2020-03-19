using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;


public class _E_Cac : MonoBehaviour
{
    [SerializeField] private List<AnimatorController> Anim =new List<AnimatorController>();
    public Animator animPerso;
    [SerializeField] private int i=0;
    private void Start() {
       animPerso.runtimeAnimatorController=Anim[i];
    }
    private void Update() {
       animPerso.runtimeAnimatorController=Anim[i];
       
    }
    
}
