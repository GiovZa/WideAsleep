using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using playerChar;

public class KillState : IState
{
    private NurseAI nurseAI;
    private NavMeshAgent agent;
    private Animator animator;
    private Player player;
    private PlayerCharacterController playerController;

    private float killDelay = 1f;
    private float timer = 0f;
    private bool hasKilled = false;

    // This can be adjusted from the NurseAI script if you make it public
    private Vector3 headLookOffset = new Vector3(0f, 5f, 0f);

    public KillState(NurseAI ai)
    {
        nurseAI = ai;
        agent = ai.GetComponent<NavMeshAgent>();
        animator = ai.GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerController = player.GetComponent<PlayerCharacterController>();
    }

    public void Enter()
    {
        if (player != null && !player.canDie)
        {
            Debug.Log("[KillState] Player is invincible. Transitioning back to ChaseState.");
            nurseAI.TransitionToState(nurseAI.chaseState);
            return;
        }

        Debug.Log("[KillState] Entering Kill State");

        agent.isStopped = true;

        if (playerController != null)
        {
            playerController.Freeze();
            playerController.lookTarget = nurseAI.transform;
            playerController.lookTargetOffset = headLookOffset;
        }

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

        // Reset the look target and offset when exiting, just in case.
        if (playerController != null)
        {
            playerController.lookTarget = null;
            playerController.lookTargetOffset = Vector3.zero;
        }
    }
}
