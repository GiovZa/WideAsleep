using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace CamGlitchKit
{
    public class GlitchRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class GlitchSettings
        {
            public bool isEnabled = true;
            public Material glitchMaterial;

            public enum GlitchType
            {
                Classic, Digital, Analog, Noise, DataMosh,
                Electric, Pixelate, VHS, ScreenTear,
                LiquidDisplace, Thermal, Hologram, GlitchOverlay
            }

            public GlitchType glitchType = GlitchType.Classic;

            [Range(0, 1)] public float intensity = 0.5f;
            [Range(0, 50)] public float distortion = 10f;

            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public GlitchSettings settings = new GlitchSettings();
        GlitchPass pass;

        public override void Create()
        {
            pass = new GlitchPass(settings)
            {
                renderPassEvent = settings.renderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        class GlitchPass : ScriptableRenderPass
        {
            GlitchSettings settings;
            RTHandle source;
            RTHandle tempTex;

            public GlitchPass(GlitchSettings settings)
            {
                this.settings = settings;
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                tempTex = RTHandles.Alloc(
                    cameraTextureDescriptor.width,
                    cameraTextureDescriptor.height,
                    colorFormat: GraphicsFormat.R8G8B8A8_UNorm,
                    name: "_TempGlitchTex"
                );
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!settings.isEnabled || settings.glitchMaterial == null)
                    return;

                CommandBuffer cmd = CommandBufferPool.Get("Glitch Effect");

                var mat = settings.glitchMaterial;
                mat.SetInt("_EffectType", (int)settings.glitchType);
                mat.SetFloat("_Intensity", settings.intensity);
                mat.SetFloat("_Distortion", settings.distortion);

                cmd.Blit(source, tempTex, mat, 0);
                cmd.Blit(tempTex, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                if (tempTex != null)
                {
                    tempTex.Release();
                    tempTex = null;
                }
            }
        }
    }
}
