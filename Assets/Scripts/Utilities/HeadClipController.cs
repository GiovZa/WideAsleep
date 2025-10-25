using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadClipController : MonoBehaviour 
{
    public SkinnedMeshRenderer[] smrs;   // 玩家自己的SMR
    public Transform headOrCamera;    // 建议用FPCam或CameraPivot
    [Range(0,0.30f)] public float radius = 0.18f;

    static readonly int HeadWS = Shader.PropertyToID("_HeadWS");
    static readonly int HeadRadius = Shader.PropertyToID("_HeadRadius");
    MaterialPropertyBlock mpb;

    void LateUpdate() {
        foreach (var smr in smrs)
        {
            if (!smr || !headOrCamera) return;
            mpb ??= new MaterialPropertyBlock();
            smr.GetPropertyBlock(mpb);
            mpb.SetVector(HeadWS, headOrCamera.position);
            mpb.SetFloat (HeadRadius, radius);
            smr.SetPropertyBlock(mpb);
        }
    }
}

