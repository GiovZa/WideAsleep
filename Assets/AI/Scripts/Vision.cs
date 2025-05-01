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
}
