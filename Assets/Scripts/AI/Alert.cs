using UnityEngine;
using Pathfinding;

public class Alert : MonoBehaviour
{
    private RichAI agent;
    private Vector3 lastKnownPosition;
    private bool isSearching = false;

    public float searchDuration = 2f;
    private float searchTimer;

    public float arriveThreshold = 1.0f; // Can be adjusted per AI


    void Awake()
    {
        agent = GetComponent<RichAI>();
    }

    public void GoToLastKnownPosition(Vector3 position)
    {
        if (agent == null)
            agent = GetComponent<RichAI>();

        agent.canMove = true;
        agent.destination = position;
    }

    void Update()
    {
        if (!isSearching)
            return;

        if (!agent.pathPending && agent.remainingDistance <= arriveThreshold)
        {
            searchTimer -= Time.deltaTime;

            // Optional: look around
            transform.Rotate(Vector3.up, 30 * Time.deltaTime);

            if (searchTimer <= 0f)
            {
                isSearching = false;
            }
        }
    }
}
