using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreamAI : MonoBehaviour
{
    public IState currentState;

    [Header("Scream Settings")]
    public float minIdleTime = 5f;
    public float maxIdleTime = 10f;
    public float screamDuration = 3f;
    public float cooldownDuration = 5f;

    // States
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public ScreamingState screamingState;
    [HideInInspector] public CooldownState cooldownState;

    void Start()
    {
        idleState = new IdleState(this);
        screamingState = new ScreamingState(this);
        cooldownState = new CooldownState(this);

        TransitionToState(idleState);
    }

    void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        Debug.Log($"[ScreamAI] Transitioned to {newState.GetType().Name}");
    }
}

// --- STATES ---

public class IdleState : IState
{
    private ScreamAI owner;
    private float idleTimer;
    private float idleDuration;

    public IdleState(ScreamAI owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        idleDuration = Random.Range(owner.minIdleTime, owner.maxIdleTime);
        idleTimer = 0f;
        Debug.Log("[IdleState] Waiting to scream...");
    }

    public void Update()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            owner.TransitionToState(owner.screamingState);
        }
    }

    public void Exit() { }
}

public class ScreamingState : IState
{
    private ScreamAI owner;
    private float screamTimer;

    public ScreamingState(ScreamAI owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        screamTimer = 0f;
        BossScreamManager.TriggerScream();
        // Here you could also trigger animations, sounds, etc. on the boss model
    }

    public void Update()
    {
        screamTimer += Time.deltaTime;
        if (screamTimer >= owner.screamDuration)
        {
            owner.TransitionToState(owner.cooldownState);
        }
    }

    public void Exit()
    {
        BossScreamManager.EndScream();
    }
}

public class CooldownState : IState
{
    private ScreamAI owner;
    private float cooldownTimer;

    public CooldownState(ScreamAI owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        cooldownTimer = 0f;
        Debug.Log("[CooldownState] Recovering from scream...");
    }

    public void Update()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= owner.cooldownDuration)
        {
            owner.TransitionToState(owner.idleState);
        }
    }

    public void Exit() { }
}
