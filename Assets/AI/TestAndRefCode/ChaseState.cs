using UnityEngine;

public class ChaseState : IState
{
    private NurseAI nurseAI;
    private Chase chase;
    private Vision vision;
    private Transform player;

    public float killDistance = 1.5f;

    public ChaseState(NurseAI ai)
    {
        nurseAI = ai;
        chase = ai.GetComponent<Chase>();
        vision = ai.GetComponent<Vision>();
        player = GameObject.FindWithTag("Player").transform;
    }

    public void Enter()
    {
        Debug.Log("[ChaseState] Entering Chase State");

        chase.enabled = true;
        chase.StartChasing(player);
    }

    public void Update()
    {
        if (Vector3.Distance(nurseAI.transform.position, player.position) <= killDistance)
        {
            Debug.Log("[ChaseState] Player in kill range. Transitioning to KillState.");
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
    }
}
