using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [Header("Eye Setup")]
    [Tooltip("Assign your Head.001 (or eye) Transform here")]
    public Transform eyeTransform;

    [Header("Vision Parameters")]
    public float viewDistance = 10f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    public LayerMask playerMask;       // (unused here but kept for future)
    public LayerMask obstructionMask;

    /// <summary>
    /// Returns true if there is line‐of‐sight from the eyeTransform (or root) to the player.
    /// </summary>
    public bool CanSeePlayer(Transform player)
    {
        Vector3 origin = eyeTransform ? eyeTransform.position : transform.position;
        Vector3 forward = eyeTransform ? eyeTransform.up : transform.up;
        Vector3 toPlayer = player.position - origin;
        float distance = toPlayer.magnitude;

        if (distance > viewDistance) 
            return false;

        Vector3 dirToPlayer = toPlayer / distance;
        if (Vector3.Angle(forward, dirToPlayer) > viewAngle * 0.5f) 
            return false;

        if (Physics.Raycast(origin, dirToPlayer, out RaycastHit hit, distance, obstructionMask))
            return false;

        return true;
    }

    void Update()
    {
        // Draw play‐mode ray (only visible if “Gizmos” is ticked in Game view)
        Vector3 origin = eyeTransform ? eyeTransform.position : transform.position;
        Vector3 forward = eyeTransform ? eyeTransform.up : transform.forward;
        Debug.DrawRay(origin, forward * viewDistance, Color.cyan);
    }

    void OnDrawGizmos()
    {
        // Draw editor‐mode ray (always visible in Scene view)
        Vector3 origin = eyeTransform ? eyeTransform.position : transform.position;
        Vector3 forward = eyeTransform ? eyeTransform.up : transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, forward * viewDistance);

        // Optional: show the FOV cone edges
        Vector3 rightEdge = Quaternion.Euler(0, viewAngle * 0.5f, 0) * forward;
        Vector3 leftEdge  = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * forward;
        Gizmos.DrawRay(origin, rightEdge * viewDistance);
        Gizmos.DrawRay(origin, leftEdge  * viewDistance);
    }
}
