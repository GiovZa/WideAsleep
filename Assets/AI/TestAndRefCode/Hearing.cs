using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    public float hearingRadius = 5f;
    public LayerMask playerMask;

    public bool HeardPlayer(Vector3 noiseSource)
    {
        float distance = Vector3.Distance(transform.position, noiseSource);
        return distance <= hearingRadius;
    }
}