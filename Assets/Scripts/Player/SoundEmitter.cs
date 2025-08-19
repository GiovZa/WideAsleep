using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A marker component to identify GameObjects as sound emitters for the HearingSense system.
/// </summary>
[RequireComponent(typeof(Renderer), typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private Renderer objectRenderer;
    private AudioSource audioSource;

    private Material[] originalMaterials;
    private List<Material> highlightMaterials = new List<Material>();

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        // Store original materials
        if (objectRenderer != null)
        {
            originalMaterials = objectRenderer.materials;
        }
    }

    public void SetVisible(bool isVisible)
    {
        if (objectRenderer != null)
        {
            objectRenderer.enabled = isVisible;
        }
    }

    public void ToggleHighlight(bool isHighlighted)
    {
        if (objectRenderer == null) return;

        if (isHighlighted)
        {
            var newMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                // Create a new material instance from the original
                newMaterials[i] = new Material(originalMaterials[i]);
                newMaterials[i].EnableKeyword("_EMISSION");
                // Set a noticeable emission color. The intensity can be tweaked.
                newMaterials[i].SetColor("_EmissionColor", Color.green * 2.0f);
                highlightMaterials.Add(newMaterials[i]);
            }
            objectRenderer.materials = newMaterials;
        }
        else
        {
            // Restore original materials
            objectRenderer.materials = originalMaterials;

            // Clean up instantiated materials
            foreach (var material in highlightMaterials)
            {
                Destroy(material);
            }
            highlightMaterials.Clear();
        }
    }

    public void PlaySound(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    public void StopSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
