using UnityEngine;
using Pathfinding;

public class Chase : MonoBehaviour
{
    private RichAI agent;
    [SerializeField] private float slowDownTime = 0f;
    [SerializeField] private float rotationSpeed = 360f;

    void Awake()
    {
        agent = GetComponent<RichAI>();
    }

    public void StartChasing(Transform target)
    {
        if (target != null)
        {
            agent.canMove = true;
            agent.destination = target.position;

            agent.slowdownTime = slowDownTime;
            agent.slowWhenNotFacingTarget = false;
            agent.rotationSpeed = rotationSpeed;
        }
    }

    public void UpdateChase(Transform target)
    {
        if (target != null)
        {
            agent.destination = target.position;
        }
    }

    public void StopChasing()
    {
        agent.canMove = false;
    }
}
