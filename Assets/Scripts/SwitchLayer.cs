using UnityEngine;
using System.Collections.Generic;

public class SwitchLayer : MonoBehaviour
{
    [Tooltip("The base color to apply to the X-ray effect for this object.")]
    public Color xRayColor = Color.red;
    [Tooltip("The HDR intensity of the X-ray color.")]
    public float xRayIntensity = 1.0f;
    public LayerMask XRayLayer;

    // The reference name of the color property in the X-ray shader.
    private const string XRAY_COLOR_PROPERTY = "_Color";

    private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();
    private Renderer[] childRenderers;
    private MaterialPropertyBlock propertyBlock;
    private int xRayLayerIndex = -1;
    private static int colorPropertyID;

    private void Awake()
    {
        xRayLayerIndex = GetLayerIndexFromMask(XRayLayer);
        propertyBlock = new MaterialPropertyBlock();
        childRenderers = GetComponentsInChildren<Renderer>(true);
        
        if (colorPropertyID == 0)
        {
            colorPropertyID = Shader.PropertyToID(XRAY_COLOR_PROPERTY);
        }

        // Store the original layers for this object and all its children
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            originalLayers.Add(t, t.gameObject.layer);
        }
    }

    public void SetXRayActive(bool active)
    {
        if (xRayLayerIndex == -1)
        {
            Debug.LogWarning("XRayLayer is not set or is invalid in the inspector for " + name);
            return;
        }

        // Apply color and intensity or clear it
        if (active)
        {
            // We multiply the color by the intensity to create an HDR color
            Color finalColor = xRayColor * xRayIntensity;
            propertyBlock.SetColor(colorPropertyID, finalColor);

            foreach (var renderer in childRenderers)
            {
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
        else
        {
            // Clear the property block to revert to the original material state
            foreach (var renderer in childRenderers)
            {
                renderer.SetPropertyBlock(null);
            }
        }

        // Switch this object and all children to the XRayLayer or back to their original layers
        foreach (KeyValuePair<Transform, int> entry in originalLayers)
        {
            if (entry.Key != null)
            {
                entry.Key.gameObject.layer = active ? xRayLayerIndex : entry.Value;
            }
        }
    }

    private static int GetLayerIndexFromMask(LayerMask layerMask)
    {
        int layerMaskValue = layerMask.value;
        if (layerMaskValue == 0) return -1;

        for (int i = 0; i < 32; i++)
        {
            if ((layerMaskValue & (1 << i)) != 0)
            {
                return i;
            }
        }

        return -1;
    }
}
