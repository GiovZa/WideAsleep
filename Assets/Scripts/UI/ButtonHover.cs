using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Color orignalColor;
    public Color hoverColor = Color.white;

    void Start()
    {
        orignalColor = text.color;
    }

    public void OnHover()
    {
        text.color = hoverColor;
    }

    public void OnHoverEnd()
    {
        text.color = orignalColor;
    }
}
