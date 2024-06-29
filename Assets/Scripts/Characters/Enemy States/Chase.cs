using UnityEngine;

public class Chase : State
{

    private float _speed;
    private float _rotateSpeed;
    private float _range;
    private Transform _target;
    private Rigidbody _rigidbody;

    public Chase(StateMachine owner, Transform target, float speed, float rotateSpeed, float range, Rigidbody rigidbody) : base(owner)
    {
        _name = "Chase";
        _target = target;
        _speed = speed;
        _rotateSpeed = rotateSpeed;
        _range = range;
        _rigidbody = rigidbody;
    }

    public override void OnEnter() { }

    public override void OnExit() { }

    public override void OnFixedUpdate()
    {
        if( _target != null )
        {
            Rotate();
            Move();
        }
    }

    public override void OnUpdate()
    {
        if(Vector3.Distance(_owner.transform.position, _target.position) < _range)
            _owner.SetState(_nextState);
    }

    private void Move()
    {
        Vector3 movementDirection = (_target.transform.position - _owner.transform.position).normalized;
        Vector3 newPosition = _rigidbody.position + _speed * Time.fixedDeltaTime * movementDirection;
        _rigidbody.MovePosition(newPosition);
    }

    private void Rotate()
    {
        Vector3 direction = _target.transform.position - _owner.transform.position;
        direction.y = 0;
        
        Quaternion rotationDirection = Quaternion.LookRotation(direction.normalized);

        if(Quaternion.Angle(rotationDirection, _rigidbody.rotation) <= 5)
            return;

        Quaternion targetRotation = Quaternion.RotateTowards(_rigidbody.rotation, rotationDirection, _rotateSpeed * Time.deltaTime);
        _rigidbody.MoveRotation(targetRotation);
    }

    public override void OnDrawGizmos() { }
}
