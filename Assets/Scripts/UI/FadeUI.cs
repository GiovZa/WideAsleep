using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float waitTime = 0;

    void Start()
    {
        canvasGroup = FindAnyObjectByType<CanvasGroup>();
    }

    /* IEnumerator WaitAndFade()
    {
        canvasGroup.DOFade(1,1); //alpha value, fade duration
        yield return new WaitForSeconds(waitTime);
        canvasGroup.DOFade(0,1);
    } */

    public void Fader()
    {
       canvasGroup.DOFade(1,0.5f);
    }
}
