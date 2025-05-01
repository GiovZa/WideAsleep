using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public Transform waypointParent;
    public float arriveThreshold = 1.0f;
    public float waitTimeAtWaypoint = 3.0f; // Time to wait at each waypoint in seconds

    private List<Transform> waypoints = new();
    private NavMeshAgent agent;
    private Transform currentTarget;

    public bool isPatrolling = true;

    void Start()
    {
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
        if (!agent.pathPending && agent.remainingDistance <= arriveThreshold && isPatrolling)
        {
            StartCoroutine(WaitAtWaypoint(waitTimeAtWaypoint)); // Wait at the current waypoint
        }

        if (!isPatrolling)
        {
            agent.ResetPath();
        }
    }

    void PickNewTarget()
    {
        if (waypoints.Count == 0) return;

        Transform oldTarget = currentTarget;
        do
        {
            currentTarget = waypoints[Random.Range(0, waypoints.Count)];
        } while (currentTarget == oldTarget && waypoints.Count > 1);

        agent.SetDestination(currentTarget.position);
    }

    // Coroutine to wait for a set period before moving to the next waypoint
    IEnumerator WaitAtWaypoint(float waitTime)
    {
        isPatrolling = false; // Stop patrolling while waiting
        Debug.Log("[Patrol] Reached waypoint. Waiting for " + waitTime + " seconds.");

        yield return new WaitForSeconds(waitTime); // Wait at the current waypoint

        Debug.Log("[Patrol] Wait time complete. Picking new target.");
        PickNewTarget(); // Once wait time is over, pick a new target
        isPatrolling = true; // Resume patrolling
    }
}
