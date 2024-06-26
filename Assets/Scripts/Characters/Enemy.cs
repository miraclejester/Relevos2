using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] 
    private float _speed;

    [Header("Data")]
    [SerializeField]
    private float _maxHealth;
    [SerializeField]
    private int _power = 1;
    public int Power => _power;


    [SerializeField]
    private Image _healthBar;

    public UnityAction OnDeath;

    private GameObject _target = null;
    private float _currentHealth = 0;
    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        _target = GameObject.FindGameObjectWithTag("Player");

        EventsManager.Instance.OnPlayerDied += OnPlayerDeath;
    }

    private void FixedUpdate() 
    {
        if( _target != null )
            Move();
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnPlayerDied -= OnPlayerDeath;
    }

    private void Move()
    {
        Vector3 movementDirection = (_target.transform.position - transform.position).normalized;
        Vector3 newPosition = _rigidBody.position + _speed * Time.fixedDeltaTime * movementDirection;
        _rigidBody.MovePosition(newPosition);
    }

    public void OnHit(float value)
    {
        _currentHealth -= value;
        _healthBar.fillAmount = _currentHealth / _maxHealth;
        if(_currentHealth <= 0)
            Die();
    }

    public void OnPlayerDeath()
    {
        _target = null;
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
