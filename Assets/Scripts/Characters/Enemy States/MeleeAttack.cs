using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : State
{
    private float _damage;
    private float _attackDuration;
    private float _radius;
    private LayerMask _layer;
    private float _attackAngle;

    private float _timer;
    private bool _attacking;

    private Collider[] _targets = new Collider[5];
    private List<GameObject> _alreadyHit = new List<GameObject>();

    private float INITIAL_COOLDOWN = 0.5f;

    private float CosAttackAngle => Mathf.Cos(_attackAngle * Mathf.Deg2Rad);

    public MeleeAttack(StateMachine owner, float damage, float attackDuration, 
                    float radius, LayerMask layer, float attackAngle) : base(owner)
    {
        _name = "Melee Attack";
        _damage = damage;
        _attackDuration = attackDuration;
        _radius = radius;
        _layer = layer;
        _attackAngle = attackAngle;
    }

    public override void OnEnter()
    {
        _targets = new Collider[5];
        _alreadyHit = new List<GameObject>();
    }

    public override void OnExit() 
    { 
        _timer = 0;
    }

    public override void OnFixedUpdate() { }

    public override void OnUpdate()
    {
        _timer += Time.deltaTime;

        if(_timer < INITIAL_COOLDOWN)
            return;
        
        if(_timer < _attackDuration)
        {
            Attack();
            return;
        }

        _owner.SetState(_nextState);
    }

    private void Attack()
    {
        Physics.OverlapSphereNonAlloc(_owner.transform.position, _radius, _targets, _layer);

        for(int i = 0; i < _targets.Length; i++)
        {
            if(_targets[i] == null)
                continue;

            if(_targets[i].gameObject == _owner.gameObject)
                continue;

            if(_alreadyHit.Contains(_targets[i].gameObject))
                continue;

            Vector3 toTarget = _targets[i].transform.position - _owner.transform.position;
            
            // Not in user cone of attack
            if(Vector3.Dot(toTarget.normalized, _owner.transform.forward) < CosAttackAngle)
                return;


            if(_targets[i].TryGetComponent(out IDamageable hurtbox))
            {
                hurtbox.OnHit(_damage);
            }

            Debug.Log("Hitting " + _targets[i].gameObject.name);

            _alreadyHit.Add(_targets[i].gameObject);
        }
    }

    public override void OnDrawGizmos()
    {
        float steps = 4;
        Vector3 forwardLimit = _owner.transform.position + _owner.transform.forward;
        float srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimit.z - _owner.transform.position.z, forwardLimit.x - _owner.transform.position.x);
        Vector3 initialPos = _owner.transform.position;
        Vector3 posA = initialPos;
        float stepAngles = _attackAngle / steps;
        var angle = srcAngles - _attackAngle / 2;

        for (var i = 0; i <= steps; i++)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 posB = initialPos;
            posB += new Vector3(_radius * Mathf.Cos(rad), 0, _radius * Mathf.Sin(rad));

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Gizmos.DrawLine(posA, initialPos);
    }
}
