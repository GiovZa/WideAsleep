using UnityEngine;

public class AlertState : IState
{
    private NurseAI nurseAI;
    private Alert alert;
    private Vision vision;
    private Transform player;

    private float alertDuration = 5f; // Total time to remain in alert
    private float alertTimer;

    private float stareTime = 1.0f;
    private float stareTimer;
    private bool isStaring;

    private float searchDuration = 2f;        // time to search in place
    private float searchTimer;
    private bool reachedLastPosition = false;
    private Vector3 lastKnownPosition;


    private bool lostSight = false;

    public AlertState(NurseAI ai)
    {
        nurseAI = ai;
        alert = ai.GetComponent<Alert>();
        vision = ai.GetComponent<Vision>();
        player = GameObject.FindWithTag("Player").transform;
    }

    public void Enter()
    {
        Debug.Log("[AlertState] Entering Alert State");

        alert.enabled = true;

        stareTimer = stareTime;
        isStaring = false;
        lostSight = false;
        reachedLastPosition = false;

        lastKnownPosition = player.position;
        alert.GoToLastKnownPosition(lastKnownPosition);

        Debug.Log("[AlertState] Moving to player's last known position");
    }

    public void Update()
    {
        float distanceToPlayer = Vector3.Distance(nurseAI.transform.position, player.position);
        if (distanceToPlayer <= nurseAI.killRange)
        {
            Debug.Log("[AlertState] Player in kill range. Transitioning to KillState.");
            nurseAI.TransitionToState(nurseAI.killState);
            return;
        }

        if (vision.CanSeePlayer(player))
        {
            // Continue stare logic
            if (!isStaring)
            {
                isStaring = true;
                stareTimer = stareTime;
                Debug.Log("[AlertState] Player spotted. Beginning stare down...");
            }

            Vector3 dir = (player.position - nurseAI.transform.position).normalized;
            dir.y = 0;
            nurseAI.transform.forward = Vector3.Lerp(nurseAI.transform.forward, dir, Time.deltaTime * 5f);

            stareTimer -= Time.deltaTime;
            if (stareTimer <= 0f)
            {
                Debug.Log("[AlertState] Stare complete. Transitioning to ChaseState.");
                nurseAI.TransitionToState(nurseAI.chaseState);
                return;
            }
        }
        else
        {
            isStaring = false;

            if (!reachedLastPosition)
            {
                float distanceToLastPos = Vector3.Distance(nurseAI.transform.position, lastKnownPosition);
                if (distanceToLastPos <= alert.arriveThreshold) // exposed in Alert script
                {
                    Debug.Log("[AlertState] Reached last known position. Starting search.");
                    reachedLastPosition = true;
                    searchTimer = searchDuration;
                }
            }
            else
            {
                // Simulate a quick glance around (could rotate side to side)
                searchTimer -= Time.deltaTime;

                // Optional look-around
                float t = Mathf.PingPong(Time.time * 2f, 1f) - 0.5f;
                nurseAI.transform.Rotate(Vector3.up, t * 30f * Time.deltaTime);

                if (searchTimer <= 0f)
                {
                    Debug.Log("[AlertState] Search timed out. Transitioning to PatrolState.");
                    nurseAI.TransitionToState(nurseAI.patrolState);
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
