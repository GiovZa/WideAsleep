using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggablePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject rotationHandle;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Outline outline;
    
    private static DraggablePiece selectedPiece;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        rotationHandle = GetComponentInChildren<RotationHandle>().gameObject;
        
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }

        if (rotationHandle != null)
        {
            rotationHandle.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectedPiece == this)
        {
            // If we are already selected, deselect.
            Deselect();
        }
        else
        {
            // Deselect the previously selected piece.
            if (selectedPiece != null)
            {
                selectedPiece.Deselect();
            }
            // Select this piece.
            Select();
        }
    }

    private void Select()
    {
        selectedPiece = this;
        if (rotationHandle != null)
        {
            rotationHandle.SetActive(true);
        }
    }

    private void Deselect()
    {
        selectedPiece = null;
        if (rotationHandle != null)
        {
            rotationHandle.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; 
        transform.SetAsLastSibling(); // Bring the piece to the front while dragging.
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the piece with the mouse/finger.
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Enable the outline on hover.
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Disable the outline when the pointer exits.
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
