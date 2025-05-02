using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class KillState : IState
{
    private NurseAI nurseAI;
    private NavMeshAgent agent;
    private Animator animator;
    private Player player;

    private float killDelay = 1.5f;
    private float timer = 0f;
    private bool hasKilled = false;

    public KillState(NurseAI ai)
    {
        nurseAI = ai;
        agent = ai.GetComponent<NavMeshAgent>();
        animator = ai.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void Enter()
    {
        Debug.Log("[KillState] Entering Kill State");

        agent.isStopped = true;

        if (animator != null)
        {
            animator.SetTrigger("Kill");
        }

        timer = 0f;
        hasKilled = false;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= killDelay && !hasKilled)
        {
            hasKilled = true;

            if (player != null)
            {
                Debug.Log("[KillState] Executing kill...");
                player.Die();
            }

            // Optional: Scene reload or endgame
        }
    }

    public void Exit()
    {
        Debug.Log("[KillState] Exiting Kill State (this shouldn't happen normally)");
    }
}
