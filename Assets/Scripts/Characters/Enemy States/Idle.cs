using UnityEngine;

public class Idle : State
{
    private float _patience = 0;
    private float _patienceTimer = 0;

    public Idle(StateMachine owner, float patience) : base(owner)
    {
        _name = "Idle";
        _patience = patience;
    }

    public override void OnEnter()
    {
        _patienceTimer = 0;
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnFixedUpdate() { }

    public override void OnUpdate()
    {
        _patienceTimer += Time.deltaTime;

        if(_patienceTimer > _patience)
            _owner.SetState(_nextState);
    }

    public override void OnDrawGizmos() { }
}
