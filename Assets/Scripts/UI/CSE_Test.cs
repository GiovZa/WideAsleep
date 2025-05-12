using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSE_Test : CutSceneElementBase
{
    public override void Execute()
    {
        WaitAndAdvance();
        Debug.Log("Executing" + name);
    }
}
