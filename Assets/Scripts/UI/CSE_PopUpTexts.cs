using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CSE_PopUpTexts : CutSceneElementBase
{
    [SerializeField] private TMP_Text popUpText;
    [TextArea] [SerializeField] private string dialouge;
    [SerializeField] Animator animator;

    public override void Execute()
    {
        popUpText.text = dialouge;
        animator.Play("FadeIn");
    }

    public override void FadeOut()
    {
        animator.Play("FadeOut");
    }
}
