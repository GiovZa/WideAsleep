using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class Hearing : MonoBehaviour
{
    public float hearingRadius = 5f;
    public LayerMask playerMask;

    public bool HeardPlayer(Vector3 noiseSource)
    {
        float distance = Vector3.Distance(transform.position, noiseSource);
        return distance <= hearingRadius;
    }

    void OnEnable()
    {
        SoundEvents.OnSoundPlayed += OnSoundHeard;
        Debug.Log("[Hearing] Subscribed to sound events.");
    }

    void OnDisable()
    {
        SoundEvents.OnSoundPlayed -= OnSoundHeard;
    }

    private void OnSoundHeard(Vector3 soundPos, float radius)
    {
        float distance = Vector3.Distance(transform.position, soundPos);
        if (distance <= hearingRadius && distance <= radius)
        {
            Debug.Log($"[Hearing] Heard sound at {soundPos}, distance = {distance}");

            NurseAI ai = GetComponent<NurseAI>();
            if (ai != null && ai.currentState != ai.chaseState)
            {
                Vector3 searchDir = (soundPos - transform.position).normalized;

                ai.alertState.OverrideSearchWithSound(soundPos);
                ai.search.TriggerSoundDebug(soundPos, radius);  // visualize it
                ai.TransitionToState(ai.alertState);
            }
        }
        else
        {
            // Add detailed log for why the sound was ignored
            Debug.LogWarning($"[Hearing] Sound IGNORED at {soundPos}. " +
                             $"Distance: {distance:F2}, " +
                             $"AI Hearing Radius: {hearingRadius}, " +
                             $"Sound's Radius: {radius:F2}. " +
                             $"(Must be within BOTH radii to be heard)");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
#endif
}