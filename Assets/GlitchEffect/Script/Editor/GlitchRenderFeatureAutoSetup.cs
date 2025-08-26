#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CamGlitchKit
{
    [InitializeOnLoad]
    public static class GlitchRenderFeatureAutoSetup
    {
        const string targetRendererAssetName = "PC_Renderer";
        const string featureName = "GlitchRenderFeature";

        static GlitchRenderFeatureAutoSetup()
        {
            EditorApplication.delayCall += SetupFeature;
        }

        static void SetupFeature()
        {
            string[] guids = AssetDatabase.FindAssets("t:UniversalRendererData");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains(targetRendererAssetName)) continue;

                var rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
                if (rendererData == null) continue;

                bool exists = System.Array.Exists(rendererData.rendererFeatures.ToArray(),
                    f => f != null && f.name.Contains(featureName));

                if (exists)
                {
                    Debug.Log($"{featureName} already exists in {path}");
                    continue;
                }

                var newFeature = ScriptableObject.CreateInstance<GlitchRenderFeature>();
                newFeature.name = featureName;

                AssetDatabase.AddObjectToAsset(newFeature, rendererData);
                rendererData.rendererFeatures.Add(newFeature);

                EditorUtility.SetDirty(rendererData);
                rendererData.SetDirty();

                Debug.Log($"{featureName} added to {path}");
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif
