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

    private bool reachedLastPosition = false;
    private Vector3 lastKnownPosition;
    private Vector3 lastSeenDirection;
    private Vector3 previousPlayerPosition;

    private Quaternion[] scanRotations;
    private int currentScanIndex = 0;
    private float scanPauseDuration = 0.4f;
    private float scanPauseTimer = 0f;
    private bool isScanning = false;
    private bool hasScanned = false;

    private float rotationSpeed = 120f;

    // Reference to the NavMeshAgent for pathfinding
    private NavMeshAgent navMeshAgent;

    public AlertState(NurseAI ai)
    {
        Debug.Log("[AlertState] Constructor");
        nurseAI = ai;
        alert = ai.GetComponent<Alert>();
        vision = ai.GetComponent<Vision>();
        search = ai.GetComponent<Search>();
        navMeshAgent = ai.GetComponent<NavMeshAgent>();
        animator = ai.GetComponent<Animator>();

        player = GameObject.FindWithTag("Player").transform;
        Debug.Log($"[AlertState] Found player: {player.name}");
    }

    public void Enter()
    {
        Debug.Log("[AlertState] Enter()");
        alert.enabled = true;
        isStaring = false;
        reachedLastPosition = false;
        isScanning = false;
        hasScanned = false;

        currentScanIndex = 0;
        scanPauseTimer = 0f;

        navMeshAgent.isStopped = false;
        Debug.Log("[AlertState] NavMeshAgent resumed");

        if (!overrideFromSound)
        {
            lastKnownPosition = player.position;
            previousPlayerPosition = player.position;
            lastSeenDirection = (player.position - nurseAI.transform.position).normalized;
            Debug.Log($"[AlertState] Normal enter: chasing lastKnown={lastKnownPosition}");
        }
        else
        {
            overrideFromSound = false;
            Debug.Log("[AlertState] overrideFromSound reset");
        }

        search.OnPlayerSpotted = () =>
        {
            Debug.Log("[AlertState] Nurse spotted player during search → Transition to ChaseState");
            nurseAI.TransitionToState(nurseAI.chaseState);
        };

        alert.GoToLastKnownPosition(lastKnownPosition);
        Debug.Log("[AlertState] GoToLastKnownPosition called");
    }

    public void OverrideSearchWithSound(Vector3 soundPos)
    {
        Debug.Log($"[AlertState] OverrideSearchWithSound({soundPos})");
        lastKnownPosition = soundPos;
        lastSeenDirection = (soundPos - nurseAI.transform.position).normalized;
        previousPlayerPosition = soundPos;
        reachedLastPosition = false;
        isScanning = false;
        hasScanned = false;
        overrideFromSound = true;
        Debug.Log("[AlertState] overrideFromSound set → will use sound position");
    }

    public void Update()
    {
        Debug.Log("[AlertState] Update()");
        if (search != null && search.IsSearching())
        {
            Debug.Log("[AlertState] Currently searching → skip Update");
            return;
        }
        else if (hasScanned && !search.IsSearching())
        {
            Debug.Log("[AlertState] hasScanned && search done → Transition to PatrolState");
            nurseAI.TransitionToState(nurseAI.patrolState);
            return;
        }

        float distanceToPlayer = Vector3.Distance(nurseAI.transform.position, player.position);
        Debug.Log($"[AlertState] distanceToPlayer = {distanceToPlayer:F2}");
        if (distanceToPlayer <= nurseAI.killRange)
        {
            Debug.Log("[AlertState] Within killRange → Transition to KillState");
            nurseAI.TransitionToState(nurseAI.killState);
            return;
        }

        if (vision.CanSeePlayer(player))
        {
            Debug.Log("[AlertState] vision.CanSeePlayer == true");
            if (!isStaring)
            {
                isStaring = true;
                stareTimer = stareTime;
                Debug.Log("[AlertState] isStaring set → starting stareTimer");
            }

            animator.SetBool("isChasing", true);

            Vector3 dir = (player.position - nurseAI.transform.position).normalized;
            dir.y = 0;
            nurseAI.transform.forward = Vector3.Lerp(nurseAI.transform.forward, dir, Time.deltaTime * 5f);
            Debug.Log($"[AlertState] Lerp forward toward {dir}");

            stareTimer -= Time.deltaTime;
            Debug.Log($"[AlertState] stareTimer = {stareTimer:F2}");
            if (stareTimer <= 0f)
            {
                Debug.Log("[AlertState] Stare complete → Transition to ChaseState");
                nurseAI.TransitionToState(nurseAI.chaseState);
            }

            return;
        }
        else
        {
            Debug.Log("[AlertState] vision.CanSeePlayer == false");
            animator.SetBool("isChasing", false);
        }

        if (!reachedLastPosition)
        {
            float distanceToLastPos = Vector3.Distance(nurseAI.transform.position, lastKnownPosition);
            Debug.Log($"[AlertState] distanceToLastPos = {distanceToLastPos:F2}");
            if (distanceToLastPos <= alert.arriveThreshold)
            {
                Debug.Log("[AlertState] Reached last known position. Starting smooth scan...");
                reachedLastPosition = true;
                isScanning = true;
            }
            else
            {
                if (!navMeshAgent.hasPath || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log("[AlertState] Path invalid → Transition to PatrolState");
                    nurseAI.TransitionToState(nurseAI.patrolState);
                }
                else
                {
                    Debug.Log("[AlertState] Still moving toward lastKnownPosition");
                }
            }
        }
        else if (isScanning)
        {
            Debug.Log("[AlertState] isScanning: True");
            string clipName = "";

            var clips = animator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                // assume the first clip
                clipName = clips[0].clip.name;
                Debug.Log("Current animation clip: " + clipName);
            }
            else
            {
                Debug.Log("No clips playing on layer 0");
            }

            AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);

            // Make sure we’re in the Alert clip, it's done playing, and no transition is happening
            if (clipName == "Nurse Look Around"
                && st.normalizedTime >= 0.5f
                // && !animator.IsInTransition(0)
                )
            {
                Debug.Log("[AlertState] isScanning and hasScanned set to: False and True");
                // flag so we only run this once
                isScanning = false;
                hasScanned = true;

                // calculate the direction you want to search in:
                Vector3 playerMovementDir = (player.position - previousPlayerPosition).normalized;
                // kick off your search
                Debug.Log("[AlertState] Calling search.StartSearch");
                search.StartSearch(nurseAI.transform.position, playerMovementDir);
                return;
            }

            // if you still want the trigger, you can leave it here:
            animator.SetTrigger("Alert");
        }
    }

    public void Exit()
    {
        Debug.Log("[AlertState] Exit()");
        alert.enabled = false;
        Debug.Log("[AlertState] Alert component disabled");
    }
}
