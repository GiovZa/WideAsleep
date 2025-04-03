using UnityEngine;

public class PatrolState : IState
{
    private NurseAI nurseAI;
    private Patrol patrol;
    private Vision vision;
    private Hearing hearing;
    private Transform player;

    public PatrolState(NurseAI ai)
    {
        nurseAI = ai;
        player = GameObject.FindWithTag("Player").transform;
        vision = ai.GetComponent<Vision>();
        hearing = ai.GetComponent<Hearing>();
        patrol = ai.GetComponent<Patrol>();
    }

    public void Enter()
    {
        Debug.Log("[PatrolState] Entering Patrol State");

        patrol.enabled = true;
        patrol.Restart();
    }

    public void Update()
    {
        float distanceToPlayer = Vector3.Distance(nurseAI.transform.position, player.position);
        if (distanceToPlayer <= nurseAI.killRange)
        {
            Debug.Log("[PatrolState] Player in kill range. Transitioning to KillState.");
            nurseAI.TransitionToState(nurseAI.killState);
            return;
        }

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
    }
}
