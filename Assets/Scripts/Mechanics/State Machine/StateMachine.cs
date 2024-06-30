using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected State _currentState = null;
    protected State _initialState = null;

    protected virtual void Start()
    {
        SetState(_initialState);
    }

    protected virtual void Update()
    {
        _currentState?.OnUpdate();
    }

    protected virtual void FixedUpdate()
    {
        _currentState?.OnFixedUpdate();
    }

    public virtual void SetState(State targetState)
    {
        if(_currentState != null)
            _currentState.OnExit();

        _currentState = targetState;
        _currentState.OnEnter();
    }

    private void OnDrawGizmos()
    {
        _currentState?.OnDrawGizmos();
    }
}
