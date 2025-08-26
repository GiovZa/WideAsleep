using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection;

namespace CamGlitchKit
{
    [InitializeOnLoad]
    public static class ForceIntermediateTextureAlways
    {
        const string targetRendererAssetName = "URP-HighFidelity-Renderer";

        static ForceIntermediateTextureAlways()
        {
            EditorApplication.delayCall += SetIntermediateTextureToAlways;
        }

        static void SetIntermediateTextureToAlways()
        {
            string[] guids = AssetDatabase.FindAssets("t:UniversalRendererData");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains(targetRendererAssetName)) continue;

                var rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
                if (rendererData == null) continue;

                var field = typeof(UniversalRendererData).GetField("m_IntermediateTextureMode", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    var current = field.GetValue(rendererData);
                    int alwaysValue = 1;

                    if ((int)current != alwaysValue)
                    {
                        field.SetValue(rendererData, alwaysValue);
                        EditorUtility.SetDirty(rendererData);
                        AssetDatabase.SaveAssets();
                        Debug.Log($"Intermediate Texture mode set to 'Always' in {path}");
                    }
                }
                else
                {
                    Debug.LogWarning("Could not find 'm_IntermediateTextureMode' field via reflection.");
                }
            }
        }
    }

}

