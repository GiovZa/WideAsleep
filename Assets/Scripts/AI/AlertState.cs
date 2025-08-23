using UnityEngine;
using UnityEngine.AI;

public class AlertState : IState
{
    private NurseAI nurseAI;
    private Alert alert;
    private Vision vision;
    private Search search;
    private Transform player;
    private Animator animator;

    private bool overrideFromSound = false;

    private float stareTime = 1.0f;
    private float stareTimer;
    private bool isStaring;

    private bool wasTriggeredBySound = false;
    private bool reachedLastPosition = false;
    private Vector3 lastKnownPosition;
    private Vector3 lastSeenDirection;
    private Vector3 previousPlayerPosition;

    private bool hasScanned = false;

    // Reference to the NavMeshAgent for pathfinding
    private NavMeshAgent navMeshAgent;

    public AlertState(NurseAI ai)
    {
        nurseAI = ai;
        alert = ai.GetComponent<Alert>();
        vision = ai.GetComponent<Vision>();
        search = ai.GetComponent<Search>();
        navMeshAgent = ai.GetComponent<NavMeshAgent>();  // Add the NavMeshAgent reference
        animator = ai.GetComponent<Animator>();

        player = GameObject.FindWithTag("Player").transform;
    }

    public void Enter()
    {
        Debug.Log("[AlertState] Entering Alert State");

        alert.enabled = true;
        isStaring = false;
        reachedLastPosition = false;
        hasScanned = false;

        navMeshAgent.isStopped = false;  // Ensure the agent is not stopped

        if (!overrideFromSound)
        {
            wasTriggeredBySound = false;
            lastKnownPosition = player.position;
            previousPlayerPosition = player.position;
            lastSeenDirection = (player.position - nurseAI.transform.position).normalized;
        }
        else
        {
            wasTriggeredBySound = true;
            overrideFromSound = false; // reset after use
        }

        search.OnPlayerSpotted = () =>
        {
            Debug.Log("[AlertState] Nurse spotted player during search. Transitioning to ChaseState.");
            nurseAI.TransitionToState(nurseAI.chaseState);
        };

        alert.GoToLastKnownPosition(lastKnownPosition);
        Debug.Log($"[AlertState] Moving to last known position: {lastKnownPosition}");

        if (animator != null)
        {
            animator.SetTrigger("Alert");
        }
    }

    public void OverrideSearchWithSound(Vector3 soundPos)
    {
        lastKnownPosition = soundPos;
        lastSeenDirection = (soundPos - nurseAI.transform.position).normalized;
        reachedLastPosition = false;
        hasScanned = false;
        overrideFromSound = true;

        // Immediately tell the AI to go to the new position
        // This prevents the Update loop from overriding it in the same frame.
        if (alert != null)
        {
            alert.GoToLastKnownPosition(soundPos);
        }
    }

    public void Update()
    {
        if (search != null && search.IsSearching())
        {
            return; // Let search component handle movement
        }
        else if (hasScanned && !search.IsSearching())
        {
            Debug.Log("[AlertState] Search complete. Returning to Patrol.");
            nurseAI.TransitionToState(nurseAI.patrolState);
            return;
        }

        if (vision.CanSeePlayer(player))
        {
            // Continuously update last known position while player is visible
            lastKnownPosition = player.position;
            alert.GoToLastKnownPosition(lastKnownPosition);

            if (!isStaring)
            {
                // This is the first frame of visual contact in this state. Notify the player.
                AINotifier.NotifyEnemySpottedPlayer(nurseAI.gameObject);

                isStaring = true;
                stareTimer = stareTime;
                Debug.Log("[AlertState] Player spotted. Beginning stare down...");
                navMeshAgent.isStopped = true; // Stop moving
            }

            Vector3 dir = (player.position - nurseAI.transform.position).normalized;
            dir.y = 0;
            nurseAI.transform.forward = Vector3.Lerp(nurseAI.transform.forward, dir, Time.deltaTime * 5f);

            stareTimer -= Time.deltaTime;
            if (stareTimer <= 0f)
            {
                Debug.Log("[AlertState] Stare complete. Transitioning to ChaseState.");
                nurseAI.TransitionToState(nurseAI.chaseState);
            }

        if (Vector3.Distance(nurseAI.transform.position, player.position) <= nurseAI.killRange)
        {
            Debug.Log("[AlertState] Player in kill range. Stopping and transitioning to KillState.");
            navMeshAgent.isStopped = true;
            nurseAI.TransitionToState(nurseAI.killState);
        }

            return;
        }

        // If we were staring but lost sight, reset and resume course.
        if (isStaring)
        {
            Debug.Log("[AlertState] Lost sight of player during stare. Resuming path.");
            isStaring = false;
            navMeshAgent.isStopped = false;
        }

        if (!reachedLastPosition)
        {
            float distanceToLastPos = Vector3.Distance(nurseAI.transform.position, lastKnownPosition);
            if (distanceToLastPos <= alert.arriveThreshold)
            {
                Debug.Log("[AlertState] Reached last known position. Beginning search.");
                reachedLastPosition = true;
                hasScanned = true;

                // Start the search based on what triggered the alert.
                if (wasTriggeredBySound)
                {
                    // If triggered by sound, search in the direction the sound came from.
                    search.StartSearch(nurseAI.transform.position, lastSeenDirection);
                }
                else
                {
                    // If triggered by sight, search in the direction the player was moving.
                    Vector3 playerMovementDir = (player.position - previousPlayerPosition).normalized;
                    previousPlayerPosition = player.position;

                    // Fallback if player isn't moving
                    if (playerMovementDir == Vector3.zero)
                        playerMovementDir = (player.position - nurseAI.transform.position).normalized;

                    search.StartSearch(nurseAI.transform.position, playerMovementDir);
                }
            }
            else
            {
                // If not reached, ensure pathfinding to the last known position
                if (!navMeshAgent.hasPath || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    // Path is blocked or not valid, decide what to do next
                    Debug.Log("[AlertState] Path to last known position is blocked, transitioning to search.");
                    nurseAI.TransitionToState(nurseAI.patrolState);  // Or you can start a search state here
                }
            }
        }
    }

    public void Exit()
    {
        Debug.Log("[AlertState] Exiting Alert State");
        alert.enabled = false;
    }
}
