using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject objectToThrowPrefab;
    public Transform throwPoint;
    public Camera playerCamera; // Reference to the player's camera

    [Header("Throwing")]
    public float throwForce = 20f;
    private CustomInput m_Input;

    private void Awake()
    {
        m_Input = new CustomInput();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        m_Input.Player.Enable();
    }

    private void OnDisable()
    {
        m_Input.Player.Disable();
    }

    private void Update()
    {
        if (m_Input.Player.Throw.triggered && Inventory.Instance.ThrowableCount > 0)
        {
            ThrowObject();
        }
    }

    private void ThrowObject()
    {
        if (objectToThrowPrefab == null || throwPoint == null || playerCamera == null)
        {
            Debug.LogError("ObjectToThrowPrefab, ThrowPoint, or PlayerCamera is not assigned in the Inspector.");
            return;
        }

        // Attempt to use a throwable from the inventory.
        if (Inventory.Instance.UseThrowable())
        {
            GameObject thrownObject = Instantiate(objectToThrowPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // The throw direction is now simply the camera's forward vector.
                Vector3 throwDirection = playerCamera.transform.forward;
                
                // Throw the object in the new direction.
                rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogWarning("The thrown object prefab is missing a Rigidbody component.");
            }
        }
    }
}
