using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    public bool CanSeePlayer(Transform player)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (!Physics.Raycast(transform.position, dirToPlayer, distance, obstructionMask))
            {
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = transform.up;

        // Draw the vision cone
        UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.1f);
        Vector3 fromDirection = Quaternion.AngleAxis(-viewAngle / 2, up) * forward;
        UnityEditor.Handles.DrawSolidArc(position, up, fromDirection, viewAngle, viewDistance);

        // Draw the boundary lines of the vision cone
        Gizmos.color = Color.red;
        Vector3 leftBoundary = fromDirection * viewDistance;
        Vector3 rightBoundary = Quaternion.AngleAxis(viewAngle, up) * fromDirection * viewDistance;
        Gizmos.DrawRay(position, leftBoundary);
        Gizmos.DrawRay(position, rightBoundary);
    }
#endif
}
