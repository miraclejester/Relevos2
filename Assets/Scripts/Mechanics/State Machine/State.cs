using UnityEngine;

public abstract class State
{
    protected string _name;
    protected StateMachine _owner;
    protected State _nextState;

    public string Name => _name;

    public State(StateMachine owner)
    {
        _owner = owner;
    }

    public void SetNextState(State state)
    {
        _nextState = state;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnExit();

    public abstract void OnDrawGizmos();
}
