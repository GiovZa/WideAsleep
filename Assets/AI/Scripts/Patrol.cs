using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public Transform waypointParent;
    public float arriveThreshold = 1.0f;
    public float waitTimeAtWaypoint = 3.0f; // Time to wait at each waypoint in seconds
    public float yRotationDegrees = 0f; // The desired Y rotation in degrees (set in the inspector)
    public bool applyRotation = true; // Whether or not to apply the Y rotation (toggleable)

    private List<Transform> waypoints = new();
    private NavMeshAgent agent;
    private Transform currentTarget;
    private int index = 0;

    public bool isPatrolling = true;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (waypointParent == null)
        {
            Debug.LogError("Waypoint parent not assigned.");
            return;
        }

        // Get all child transforms as waypoints
        foreach (Transform child in waypointParent)
        {
            waypoints.Add(child);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints found under " + waypointParent.name);
            return;
        }

        // Sort waypoints by their name (no need for custom functions)
        waypoints.Sort((a, b) => a.name.CompareTo(b.name));

        PickNewTarget();
    }

    public void Restart()
    {
        isPatrolling = true;
        agent = GetComponent<NavMeshAgent>();
        if (!agent.pathPending && agent.remainingDistance <= arriveThreshold)
        {
            PickNewTarget();
        }
    }

    void Update()
    {
        if (waypoints.Count == 1 && isPatrolling)
        {
            // If there's only one waypoint, just stay there (no movement)
            if (!agent.pathPending && agent.remainingDistance <= arriveThreshold)
            {
                StartCoroutine(WaitAtWaypoint(waitTimeAtWaypoint)); // Wait indefinitely at the waypoint
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= arriveThreshold && isPatrolling)
        {
            StartCoroutine(WaitAtWaypoint(waitTimeAtWaypoint)); // Wait at the current waypoint
        }

        if (!isPatrolling)
        {
            agent.ResetPath();
        }

        // Apply the rotation if rotation is enabled
        if (applyRotation && currentTarget != null)
        {
            ApplyRotation();
        }
    }

    void PickNewTarget()
    {
        if (waypoints.Count == 0) return;

        // If there's only one waypoint, stay at it and do not change targets
        if (waypoints.Count == 1)
        {
            currentTarget = waypoints[0];
            agent.SetDestination(currentTarget.position);
            return;
        }

        // Modulo approach to wrap the index safely within the waypoints range
        index = (index + 1) % waypoints.Count;

        // Set the new target
        currentTarget = waypoints[index];

        agent.SetDestination(currentTarget.position);
    }

    // Coroutine to wait for a set period before moving to the next waypoint
    IEnumerator WaitAtWaypoint(float waitTime)
    {
        isPatrolling = false; // Stop patrolling while waiting
        animator.SetBool("isWaiting", true);
        // Debug.Log("[Patrol] Reached waypoint. Waiting for " + waitTime + " seconds.");

        yield return new WaitForSeconds(waitTime); // Wait at the current waypoint

        // Debug.Log("[Patrol] Wait time complete. Picking new target.");
        PickNewTarget(); // Once wait time is over, pick a new target
        isPatrolling = true; // Resume patrolling
        animator.SetBool("isWaiting", false);
    }

    // Method to apply the Y rotation based on the value in yRotationDegrees
    void ApplyRotation()
    {
        // Set the rotation to the desired Y rotation (maintaining the same X and Z values)
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = yRotationDegrees; // Set the Y rotation to the specified value

        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
