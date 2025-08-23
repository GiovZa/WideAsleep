using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject objectToThrowPrefab;
    public Transform throwPoint;

    [Header("Throwing")]
    public float throwForce = 20f;
    public float throwAngle = 30f;

    [Header("Input")]
    public KeyCode throwKey = KeyCode.G;

    private int throwableCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(throwKey) && throwableCount > 0)
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
