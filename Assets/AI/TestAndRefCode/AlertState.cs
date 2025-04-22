using UnityEngine;

public class AlertState : IState
{
    private NurseAI nurseAI;
    private Alert alert;
    private Vision vision;
    private Search search;
    private Transform player;

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

    public AlertState(NurseAI ai)
    {
        nurseAI = ai;
        alert = ai.GetComponent<Alert>();
        vision = ai.GetComponent<Vision>();
        search = ai.GetComponent<Search>();

        player = GameObject.FindWithTag("Player").transform;
    }

    public void Enter()
    {
        Debug.Log("[AlertState] Entering Alert State");

        alert.enabled = true;
        isStaring = false;
        reachedLastPosition = false;
        isScanning = false;
        hasScanned = false;

        currentScanIndex = 0;
        scanPauseTimer = 0f;

        if (!overrideFromSound)
        {
            lastKnownPosition = player.position;
            previousPlayerPosition = player.position;
            lastSeenDirection = (player.position - nurseAI.transform.position).normalized;
        }
        else
        {
            overrideFromSound = false; // reset after use
        }

        search.OnPlayerSpotted = () =>
        {
            Debug.Log("[AlertState] Nurse spotted player during search. Transitioning to ChaseState.");
            nurseAI.TransitionToState(nurseAI.chaseState);
        };

        alert.GoToLastKnownPosition(lastKnownPosition);
        Debug.Log("[AlertState] Moving to player's last known position");
    }

    public void OverrideSearchWithSound(Vector3 soundPos)
    {
        lastKnownPosition = soundPos;
        lastSeenDirection = (soundPos - nurseAI.transform.position).normalized;
        previousPlayerPosition = soundPos;
        reachedLastPosition = false;
        isScanning = false;
        hasScanned = false;
        overrideFromSound = true;
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

        float distanceToPlayer = Vector3.Distance(nurseAI.transform.position, player.position);
        if (distanceToPlayer <= nurseAI.killRange)
        {
            nurseAI.TransitionToState(nurseAI.killState);
            return;
        }

        if (vision.CanSeePlayer(player))
        {
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
            }

            return;
        }

        if (!reachedLastPosition)
        {
            float distanceToLastPos = Vector3.Distance(nurseAI.transform.position, lastKnownPosition);
            if (distanceToLastPos <= alert.arriveThreshold)
            {
                Debug.Log("[AlertState] Reached last known position. Starting smooth scan...");
                reachedLastPosition = true;
                isScanning = true;

                Quaternion baseRot = nurseAI.transform.rotation;
                scanRotations = new Quaternion[]
                {
                    baseRot * Quaternion.AngleAxis(90f, Vector3.up),
                    baseRot * Quaternion.AngleAxis(-90f, Vector3.up),
                    baseRot
                };
            }
        }
        else if (isScanning)
        {
            Quaternion targetRot = scanRotations[currentScanIndex];
            nurseAI.transform.rotation = Quaternion.RotateTowards(
                nurseAI.transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );

            float angle = Quaternion.Angle(nurseAI.transform.rotation, targetRot);
            if (angle < 1f)
            {
                scanPauseTimer += Time.deltaTime;
                if (scanPauseTimer >= scanPauseDuration)
                {
                    scanPauseTimer = 0f;
                    currentScanIndex++;

                    if (currentScanIndex >= scanRotations.Length)
                    {
                        Debug.Log("[AlertState] Finished scanning. Beginning search.");
                        isScanning = false;
                        hasScanned = true;

                        Vector3 playerMovementDir = (player.position - previousPlayerPosition).normalized;
                        previousPlayerPosition = player.position;

                        // Fallback if player isn't moving
                        if (playerMovementDir == Vector3.zero)
                            playerMovementDir = (player.position - nurseAI.transform.position).normalized;

                        search.StartSearch(nurseAI.transform.position, playerMovementDir);

                        // search.StartSearch(nurseAI.transform.position, lastSeenDirection);
                    }
                    else
                    {
                        Vector3 fwd = nurseAI.transform.forward;
                        Debug.Log($"[AlertState] Finished turn {currentScanIndex}, forward = ({fwd.x:F2}, {fwd.y:F2}, {fwd.z:F2})");
                    }
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
