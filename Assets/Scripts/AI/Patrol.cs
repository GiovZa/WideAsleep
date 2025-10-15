using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Patrol : MonoBehaviour
{
    public Transform waypointParent;
    [SerializeField] private float slowDownTime = 0.2f;
    [SerializeField] private float rotationSpeed = 250f;
    public float arriveThreshold = 1.0f;
    public float waitTimeAtWaypoint = 3.0f; // Time to wait at each waypoint in seconds
    public float yRotationDegrees = 0f; // The desired Y rotation in degrees (set in the inspector)
    public bool applyRotation = true; // Whether or not to apply the Y rotation (toggleable)

    private List<Transform> waypoints = new();
    private RichAI agent;
    private Transform currentTarget;
    private int index = 0;

    public bool isPatrolling = true;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<RichAI>();

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
        agent = GetComponent<RichAI>();

        agent.slowdownTime = slowDownTime;
        agent.slowWhenNotFacingTarget = true;
        agent.rotationSpeed = rotationSpeed;

        // When restarting patrol after an interruption, always tell the agent to resume its path
        // to its last known target waypoint.
        if (currentTarget != null)
        {
            agent.destination = currentTarget.position;
        }
        else
        {
            // Fallback for the very first time the AI starts, ensures it has a target.
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
            agent.destination = currentTarget.position;
            return;
        }

        // Modulo approach to wrap the index safely within the waypoints range
        index = (index + 1) % waypoints.Count;

        // Set the new target
        currentTarget = waypoints[index];

        agent.destination = currentTarget.position;
    }

    // Coroutine to wait for a set period before moving to the next waypoint
    IEnumerator WaitAtWaypoint(float waitTime)
    {
        isPatrolling = false; // Stop Update() from re-triggering this coroutine
        agent.canMove = false; // Pause the agent's movement
        animator.SetBool("isWaiting", true);

        yield return new WaitForSeconds(waitTime); // Wait at the current waypoint

        if (waypoints.Count > 1)
        {
            PickNewTarget(); // Once wait time is over, pick a new target
            agent.canMove = true; // Resume agent movement
            isPatrolling = true; // Allow Update() to trigger this again on arrival
            animator.SetBool("isWaiting", false);
        }
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
