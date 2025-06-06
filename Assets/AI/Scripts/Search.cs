using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Search : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vision vision;
    private Transform player;

    private Vector3 lastSeenDirection;
    private List<Vector3> searchPoints = new();
    private int searchIndex = 0;
    private float searchPauseTimer = 0f;
    private bool isSearching = false;
    private bool isLookingAround = false;

    public float maxSearchPointDistance = 6f;
    public float minSearchPointDistance = 3f;
    public int maxSearchAttempts = 10;
    public float searchDurationAtPoint = 2f;
    public float angleSpread = 30f;
    public int numPointsToTry = 2;

    private Quaternion[] scanRotations;
    private int currentScanIndex = 0;
    private float scanPauseDuration = 0.4f;
    private float scanPauseTimer = 0f;
    private float rotationSpeed = 120f;

    public System.Action OnPlayerSpotted;

    private Vector3? soundDebugPosition = null;
    private float soundDebugRadius = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<Vision>();
        player = GameObject.FindWithTag("Player").transform;
    }

    public void StartSearch(Vector3 fromPosition, Vector3 perceivedDirection)
    {
        lastSeenDirection = perceivedDirection.normalized;
        searchPoints.Clear();
        searchIndex = 0;
        searchPauseTimer = 0f;
        isSearching = false;
        isLookingAround = false;

        GenerateSearchPoints(fromPosition);

        if (searchPoints.Count > 0)
        {
            isSearching = true;
            agent.SetDestination(searchPoints[searchIndex]);
        }
        else
        {
            Debug.Log("[Search] No valid search points generated.");
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

        if (vision != null && vision.CanSeePlayer(player))
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

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            BeginLookAround();
        }
    }

    void GenerateSearchPoints(Vector3 origin)
    {
        for (int i = 0; i < maxSearchAttempts && searchPoints.Count < numPointsToTry; i++)
        {
            float angle = Random.Range(-angleSpread, angleSpread);
            Vector3 rotatedDir = Quaternion.Euler(0, angle, 0) * lastSeenDirection;
            float distance = Random.Range(minSearchPointDistance, maxSearchPointDistance);
            Vector3 candidate = origin + rotatedDir.normalized * distance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                NavMeshPath path = new();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    float totalDist = 0f;
                    for (int j = 1; j < path.corners.Length; j++)
                        totalDist += Vector3.Distance(path.corners[j - 1], path.corners[j]);

                    if (totalDist <= maxSearchPointDistance && totalDist >= minSearchPointDistance)
                    {
                        searchPoints.Add(hit.position);
                    }
                }
            }
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
                    isLookingAround = false;
                    searchIndex++;

                    if (searchIndex >= searchPoints.Count)
                    {
                        Debug.Log("[Search] Finished all search points.");
                        isSearching = false;
                    }
                    else
                    {
                        agent.SetDestination(searchPoints[searchIndex]);
                    }
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
