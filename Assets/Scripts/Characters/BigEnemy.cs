using UnityEngine;
using TMPro;

public class BigEnemy : StateMachine, IDamageable
{
    [Header("Data")]
    [SerializeField]
    private float _damage = 2;

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _rotateSpeed = 20f;
    [SerializeField]
    private float _range = 3f;
    [SerializeField]
    private float _attackDuration = 1.2f;
    [SerializeField]
    private float _attackAngle = 30f;
    [SerializeField]
    private LayerMask _attackLayer;

    [Header("Debug")]
    [SerializeField]
    private TMP_Text _state;

    private Chase _chase;
    private MeleeAttack _attack;

    private GameObject _target;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");

        _attack = new MeleeAttack(this, _damage, _attackDuration, _range, _attackLayer, _attackAngle);
        _chase = new Chase(this, _target.transform, _speed, _rotateSpeed, _range, _rigidbody);
        
        _chase.SetNextState(_attack);
        _attack.SetNextState(_chase);

        _initialState = _chase;
        base.Start();
    }

    public override void SetState(State targetState)
    {
        base.SetState(targetState);
        _state.text = _currentState.Name;
    }

    public void OnHit(float value)
    {
        throw new System.NotImplementedException();
    }
}
