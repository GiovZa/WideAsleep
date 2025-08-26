using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CamGlitchKit
{
    [InitializeOnLoad]
    public static class RenderGraphCompatibilityModeAutoSetter
    {
        static RenderGraphCompatibilityModeAutoSetter()
        {
            EnableCompatibilityMode();
        }

        private static void EnableCompatibilityMode()
        {
            var renderGraphSettings = GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>();

            if (renderGraphSettings != null)
            {
                if (!renderGraphSettings.enableRenderCompatibilityMode)
                {
                    renderGraphSettings.enableRenderCompatibilityMode = true;
                    Debug.Log("Render Graph Compatibility Mode AUTOMATICALLY enabled.");
                }
            }
            else
            {
                Debug.LogWarning("RenderGraphSettings not found. Check that URP is active.");
            }
        }
    }
}


