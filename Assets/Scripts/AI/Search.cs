using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Search : MonoBehaviour
{
    private RichAI agent;
    private Vision vision;
    private Transform player;

    private Vector3 lastSeenDirection;
    private List<Vector3> searchPoints = new();
    private int searchIndex = 0;
    private bool isSearching = false;
    private bool isLookingAround = false;
    public float maxSearchPointDistance = 6f;
    public float minSearchPointDistance = 3f;
    [Tooltip("Maximum attempts to generate random search points (some may fail validation)")]
    public int maxGenerationAttempts = 10;
    public float angleSpread = 30f;
    [Tooltip("Target number of valid search points to find")]
    public int targetSearchPoints = 2;
    
    [Header("Give Up Settings")]
    [Tooltip("If true, skip look-around when path is blocked and move to next point")]
    public bool skipLookAroundWhenBlocked = true;
    [Tooltip("Max time to wait for reaching a search point before giving up on it")]
    public float searchPointTimeout = 5f;
    [Tooltip("Distance threshold to consider a search point 'reached'")]
    public float arriveThreshold = 1.5f;

    private Quaternion[] scanRotations;
    private int currentScanIndex = 0;
    private float scanPauseDuration = 0.4f;
    private float scanPauseTimer = 0f;
    private float rotationSpeed = 120f;
    
    private float timeSpentOnCurrentPoint = 0f;

    public System.Action OnPlayerSpotted;

    private Vector3? soundDebugPosition = null;
    private float soundDebugRadius = 0f;

    void Awake()
    {
        agent = GetComponent<RichAI>();
        vision = GetComponent<Vision>();
    }

    void OnEnable()
    {
        SceneSwapManager.OnPlayerSpawned += HandlePlayerSpawned;
        SceneSwapManager.OnPlayerWillBeDestroyed += HandlePlayerDestroyed;

        // If the player already exists when this AI is enabled, get the reference immediately.
        if (SceneSwapManager.PlayerInstance != null)
        {
            player = SceneSwapManager.PlayerInstance.transform;
        }
    }

    void OnDisable()
    {
        SceneSwapManager.OnPlayerSpawned -= HandlePlayerSpawned;
        SceneSwapManager.OnPlayerWillBeDestroyed -= HandlePlayerDestroyed;
    }

    private void HandlePlayerSpawned(GameObject playerObject)
    {
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void HandlePlayerDestroyed()
    {
        player = null;
    }

    public void StartSearch(Vector3 fromPosition, Vector3 perceivedDirection)
    {
        lastSeenDirection = perceivedDirection.normalized;
        searchPoints.Clear();
        searchIndex = 0;
        isSearching = false;
        isLookingAround = false;
        timeSpentOnCurrentPoint = 0f;

        GenerateSearchPoints(fromPosition);

        if (searchPoints.Count > 0)
        {
            isSearching = true;
            agent.canMove = true;
            agent.destination = searchPoints[searchIndex];
            Debug.Log($"[Search] Starting search with {searchPoints.Count} points.");
        }
        else
        {
            Debug.LogWarning("[Search] No valid search points generated. Beginning look-around at current position.");
            // If we can't find search points, at least do a look-around from current position
            isSearching = true;
            isLookingAround = true;
            BeginLookAround();
        }
    }

    public void TriggerSoundDebug(Vector3 soundPosition, float radius)
    {
        soundDebugPosition = soundPosition;
        soundDebugRadius = radius;
    }

    void Update()
    {
        if (!isSearching) return;

        if (vision != null && player != null && vision.CanSeePlayer(player))
        {
            Debug.Log("[Search] Player spotted during search!");
            isSearching = false;
            OnPlayerSpotted?.Invoke();
            return;
        }

        if (isLookingAround)
        {
            HandleLookAround();
            return;
        }

        // Track time spent trying to reach this search point
        timeSpentOnCurrentPoint += Time.deltaTime;

        // Check if we've reached the end of the path
        if (agent.reachedEndOfPath)
        {
            // Check if we actually reached the destination or if the path is blocked
            Vector3 currentDestination = searchPoints[searchIndex];
            float distanceToDestination = Vector3.Distance(transform.position, currentDestination);
            bool actuallyReached = distanceToDestination <= arriveThreshold;

            if (actuallyReached)
            {
                // We successfully reached the search point, do a look-around
                Debug.Log($"[Search] Reached search point {searchIndex + 1}/{searchPoints.Count}");
                BeginLookAround();
            }
            else if (skipLookAroundWhenBlocked)
            {
                // Path is blocked, skip look-around and move to next point
                Debug.Log($"[Search] Path to search point {searchIndex + 1} is blocked (distance: {distanceToDestination:F2}). Skipping to next point.");
                MoveToNextSearchPoint();
            }
            else
            {
                // Path is blocked but we still do a look-around from where we are
                Debug.Log($"[Search] Path blocked but doing look-around from current position.");
                BeginLookAround();
            }
        }
        // Timeout check: if we've been trying to reach this point for too long, give up
        else if (timeSpentOnCurrentPoint >= searchPointTimeout)
        {
            Debug.LogWarning($"[Search] Timeout reaching search point {searchIndex + 1}. Moving to next point.");
            MoveToNextSearchPoint();
        }
    }

    void GenerateSearchPoints(Vector3 origin)
    {
        if (AstarPath.active == null)
        {
            Debug.LogError("[Search] AstarPath.active is null. Cannot generate search points.");
            return;
        }

        for (int i = 0; i < maxGenerationAttempts && searchPoints.Count < targetSearchPoints; i++)
        {
            float angle = Random.Range(-angleSpread, angleSpread);
            Vector3 rotatedDir = Quaternion.Euler(0, angle, 0) * lastSeenDirection;
            float distance = Random.Range(minSearchPointDistance, maxSearchPointDistance);
            Vector3 candidate = origin + rotatedDir.normalized * distance;

            var nninfo = AstarPath.active.GetNearest(candidate, NNConstraint.Walkable);
            Vector3 closestPoint = nninfo.position;

            if (nninfo.node != null && Vector3.Distance(candidate, closestPoint) < 2f)
            {
                try
                {
                    var path = ABPath.Construct(origin, closestPoint);
                    AstarPath.StartPath(path);  // Must start the path before blocking
                    AstarPath.BlockUntilCalculated(path);
                    
                    if (!path.error)
                    {
                        float totalDist = path.GetTotalLength();
                        if (totalDist <= maxSearchPointDistance && totalDist >= minSearchPointDistance)
                        {
                            searchPoints.Add(closestPoint);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[Search] Failed to calculate path for search point: {e.Message}");
                }
            }
        }
    }

    void MoveToNextSearchPoint()
    {
        searchIndex++;
        timeSpentOnCurrentPoint = 0f;

        if (searchIndex >= searchPoints.Count)
        {
            Debug.Log("[Search] Finished all search points.");
            isSearching = false;
        }
        else
        {
            agent.destination = searchPoints[searchIndex];
            Debug.Log($"[Search] Moving to search point {searchIndex + 1}/{searchPoints.Count}");
        }
    }

    void BeginLookAround()
    {
        isLookingAround = true;
        scanPauseTimer = 0f;
        currentScanIndex = 0;

        Quaternion baseRot = transform.rotation;
        scanRotations = new Quaternion[]
        {
            baseRot * Quaternion.AngleAxis(90f, Vector3.up),
            baseRot * Quaternion.AngleAxis(-90f, Vector3.up),
            baseRot
        };
    }

    void HandleLookAround()
    {
        Quaternion targetRot = scanRotations[currentScanIndex];
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        float angle = Quaternion.Angle(transform.rotation, targetRot);
        if (angle < 1f)
        {
            scanPauseTimer += Time.deltaTime;
            if (scanPauseTimer >= scanPauseDuration)
            {
                scanPauseTimer = 0f;
                currentScanIndex++;

                if (currentScanIndex >= scanRotations.Length)
                {
                    // Finished looking around at this point
                    isLookingAround = false;
                    MoveToNextSearchPoint();
                }
            }
        }
    }

    public bool IsSearching() => isSearching;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (searchPoints.Count == 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            return;
        }

        for (int i = 0; i < searchPoints.Count; i++)
        {
            Gizmos.color = (i == searchIndex) ? Color.green : Color.yellow;
            Gizmos.DrawSphere(searchPoints[i], 0.3f);
            Gizmos.DrawLine(transform.position, searchPoints[i]);
        }

        if (isSearching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, lastSeenDirection.normalized * maxSearchPointDistance);
        }

        if (soundDebugPosition.HasValue)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
            Gizmos.DrawWireSphere(soundDebugPosition.Value, soundDebugRadius);
        }
    }
#endif
}
