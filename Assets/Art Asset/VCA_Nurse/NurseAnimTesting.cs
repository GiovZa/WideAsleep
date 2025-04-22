using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseAnimTesting : MonoBehaviour
{
    Animator animator;
    [SerializeField] Patrol patrol;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (patrol.isPatrolling == true)
            animator.SetBool("IsMoving", true);
        else
            animator.SetBool("IsMoving", false);
        
        //animator.SetBool("IsMoving", true);
    }
}
