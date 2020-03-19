using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AnimGong : MonoBehaviour
{
    [SerializeField] private List<Animation> Anim = new List<Animation>();
    [SerializeField] int i = 0;
    public AnimatorController Controllor;
    public Animator f;
    
    private void Start()
    {

    }

}
