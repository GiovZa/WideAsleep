using UnityEngine;

public class ChaseState : IState
{
    private NurseAI nurseAI;
    private Chase chase;
    private Vision vision;
    private Transform player;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    public ChaseState(NurseAI ai)
    {
        nurseAI = ai;
        chase = ai.GetComponent<Chase>();
        vision = ai.GetComponent<Vision>();
        player = GameObject.FindWithTag("Player").transform;
        animator = ai.GetComponent<Animator>();
        agent = ai.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void Enter()
    {
        Debug.Log("[ChaseState] Entering Chase State");

        chase.enabled = true;
        chase.StartChasing(player);

        animator.SetBool("isChasing", true);
    }

    public void Update()
    {
        if (Vector3.Distance(nurseAI.transform.position, player.position) <= nurseAI.killRange)
        {
            Debug.Log("[ChaseState] Player in kill range. Stopping and transitioning to KillState.");
            agent.isStopped = true;
            nurseAI.TransitionToState(nurseAI.killState);
            return;
        }

        if (!vision.CanSeePlayer(player))
        {
            Debug.Log("[ChaseState] Lost sight of player. Transitioning to AlertState.");
            nurseAI.TransitionToState(nurseAI.alertState);
            return;
        }

        chase.UpdateChase(player);
    }

    public void Exit()
    {
        Debug.Log("[ChaseState] Exiting Chase State");

        chase.StopChasing();
        chase.enabled = false;

        animator.SetBool("isChasing", false);
    }
}
