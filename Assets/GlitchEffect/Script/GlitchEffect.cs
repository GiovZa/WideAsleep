using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CamGlitchKit
{
    [ExecuteAlways]
    public class GlitchEffect : MonoBehaviour
    {
        public enum GlitchType
        {
            Classic, Digital, Analog, Noise, DataMosh,
            Electric, Pixelate, VHS, ScreenTear,
            LiquidDisplace, Thermal, Hologram, GlitchOverlay
        }

        public bool isEnabled = true;
        public GlitchType glitchType = GlitchType.Classic;
        public Material glitchMaterial;

        [Range(0, 1)] public float intensity = 0.5f;
        [Range(0, 50)] public float distortion = 10f;

        private void Update()
        {
            ApplyToRenderFeature();
        }

        private void ApplyToRenderFeature()
        {
            var rendererAssets = Resources.FindObjectsOfTypeAll<UniversalRendererData>();
            foreach (var rendererData in rendererAssets)
            {
                foreach (var feature in rendererData.rendererFeatures)
                {
                    if (feature is GlitchRenderFeature renderFeature && renderFeature.settings != null)
                    {
                        renderFeature.settings.isEnabled = isEnabled;
                        renderFeature.settings.glitchMaterial = glitchMaterial;
                        renderFeature.settings.glitchType = (GlitchRenderFeature.GlitchSettings.GlitchType)glitchType;
                        renderFeature.settings.intensity = intensity;
                        renderFeature.settings.distortion = distortion;

#if UNITY_EDITOR
                        EditorUtility.SetDirty(rendererData);
#endif
                    }
                }
            }
        }
    }
}
