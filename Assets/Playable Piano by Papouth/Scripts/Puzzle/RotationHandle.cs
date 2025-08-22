using UnityEngine;
using UnityEngine.EventSystems;

public class RotationHandle : MonoBehaviour, IDragHandler
{
    private RectTransform pieceToRotate;

    private void Awake()
    {
        // The handle expects to be a child of the piece it rotates.
        pieceToRotate = transform.parent.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pieceToRotate == null) return;

        // Use the canvas-scaled mouse delta for consistent rotation.
        Vector2 mousePos = eventData.position;
        Vector2 piecePos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, pieceToRotate.position);
        
        Vector2 direction = mousePos - piecePos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the parent piece.
        pieceToRotate.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void LateUpdate()
    {
        // This will counteract the parent's rotation to keep the handle upright.
        if (pieceToRotate != null)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
