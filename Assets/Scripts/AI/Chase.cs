using UnityEngine;
using Pathfinding;

public class Chase : MonoBehaviour
{
    private RichAI agent;

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
