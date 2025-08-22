using UnityEngine;
using UnityEngine.Animations;

public class PatrolState : IState
{
    private NurseAI nurseAI;
    private Patrol patrol;
    private Vision vision;
    private Hearing hearing;
    private Transform player;
    private Animator animator;

    public PatrolState(NurseAI ai)
    {
        nurseAI = ai;
        player = GameObject.FindWithTag("Player").transform;
        vision = ai.GetComponent<Vision>();
        hearing = ai.GetComponent<Hearing>();
        patrol = ai.GetComponent<Patrol>();
        animator = ai.GetComponent<Animator>();
    }

    public void Enter()
    {
        Debug.Log("[PatrolState] Entering Patrol State");

        patrol.enabled = true;
        patrol.Restart();

        animator.SetBool("isPatrolling", true);
    }

    public void Update()
    {
        if (vision.CanSeePlayer(player.transform))
        {
            Debug.Log("[PatrolState] Player spotted. Transitioning to AlertState.");
            nurseAI.TransitionToState(nurseAI.alertState);
        }
    }

    public void Exit()
    {
        Debug.Log("[PatrolState] Exiting Patrol State");

        patrol.enabled = false;

        animator.SetBool("isPatrolling", false);
    }
}
