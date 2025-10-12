using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A marker component to identify GameObjects as sound emitters for the HearingSense system.
/// Plays a sound via the AudioManager that can be detected by AI.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class SoundEmitterHint : MonoBehaviour
{
    private Renderer objectRenderer;
    [SerializeField] private AudioClip soundClip;
    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }

        if (objectRenderer != null)
        {
            objectRenderer.enabled = false;
        }
    }

    public void SetVisible(bool isVisible)
    {
        if (objectRenderer != null)
        {
            objectRenderer.enabled = isVisible;
        }
    }

    public void PlaySound(float volume)
    {
        if (soundClip == null)
        {
            Debug.LogError("No sound clip assigned to " + gameObject.name);
            return;
        }
        AudioManager.Instance.PlaySoundForPlayerOnly(soundClip, transform.position, volume, true, AudioManager.Instance.SFXMixerGroup);
    }
}
