using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    protected QuickOutline outline;
    public string message;

    public UnityEvent onInteraction;

    // Start is called before the first frame update
    public virtual void Start()
    {
        outline = GetComponent<QuickOutline>();
        if (outline == null)
        {
            outline = GetComponentInChildren<QuickOutline>();
        }
        DisableOutline();
    }

    public virtual void Interact()
    {
        onInteraction.Invoke();
        Debug.Log("[" + gameObject.name + "] " + message);
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
