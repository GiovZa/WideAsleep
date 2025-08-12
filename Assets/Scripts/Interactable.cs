using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    protected Outline outline;
    public string message;

    public UnityEvent onInteraction;

    // Start is called before the first frame update
    public virtual void Start()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = GetComponentInChildren<Outline>();
        }
        DisableOutline();
    }

    public virtual void Interact()
    {
        onInteraction.Invoke();
        Debug.Log("[Piano] Picked up object!");
    }

    public void DisableOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
    public void EnableOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public virtual CrosshairType GetCrosshairType()
    {
        return CrosshairType.Interact; //default interact type
    }
}
