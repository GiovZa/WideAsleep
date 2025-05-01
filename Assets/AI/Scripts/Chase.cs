using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void StartChasing(Transform target)
    {
        if (target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    public void UpdateChase(Transform target)
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    public void StopChasing()
    {
        agent.isStopped = true;
    }
}
