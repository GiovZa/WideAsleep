using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class NurseAI : MonoBehaviour
{
    private IState currentState;

    public PatrolState patrolState;
    public AlertState alertState;
    public ChaseState chaseState;
    public KillState killState;


    public float killRange = 1.5f;

    void Start()
    {
        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        chaseState = new ChaseState(this);
        killState = new KillState(this);

        TransitionToState(patrolState);
    }

    void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(IState newState)
    {
        Debug.Log($"[NurseAI] Transitioning from {currentState?.GetType().Name} to {newState.GetType().Name}");

        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

}
