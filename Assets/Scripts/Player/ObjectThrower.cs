using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject objectToThrowPrefab;
    public Transform throwPoint;

    [Header("Throwing")]
    public float throwForce = 20f;
    public float throwAngle = 30f;
    private CustomInput m_Input;
    private int throwableCount = 0;

    private void Awake()
    {
        m_Input = new CustomInput();
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
        if (m_Input.Player.Throw.triggered && throwableCount > 0)
        {
            ThrowObject();
        }
    }

    public void AddThrowable()
    {
        throwableCount++;
        Debug.Log($"Picked up throwable. Total: {throwableCount}");
    }

    private void ThrowObject()
    {
        if (objectToThrowPrefab == null || throwPoint == null)
        {
            Debug.LogError("ObjectToThrowPrefab or ThrowPoint is not assigned in the Inspector.");
            return;
        }

        throwableCount--;

        GameObject thrownObject = Instantiate(objectToThrowPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Calculate the throw direction with an upward angle.
            Vector3 throwDirection = Quaternion.AngleAxis(-throwAngle, throwPoint.right) * throwPoint.forward;
            
            // Throw the object in the new direction.
            rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning("The thrown object prefab is missing a Rigidbody component.");
        }
        Debug.Log($"Threw object. Remaining: {throwableCount}");
    }
}
