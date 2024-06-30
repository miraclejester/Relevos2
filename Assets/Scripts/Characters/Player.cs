using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable {
    [Header("Health")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _invincibilityDuration;

    [Header("Movement")]
    [SerializeField] private float _speed;

    [Header("Powers")]
    [SerializeField] private GameObject _rangeIndicator;
    [SerializeField] private float _rangeRadius = 5f;
    [SerializeField] private Transform _referencePlaneTransform;
    [SerializeField] private float _pickedObjectYOffset = 0.8f;
    [SerializeField] private float maxDraggableWeight = 20;

    [SerializeField]
    private float _scrollSpeed = 30f;
    private float _scrollInput;

    private Vector3 _input;
    private float _currentSpeed;
    private Rigidbody _rigidBody;
    private Rigidbody _picked_rb;
    private PlayerActivableObject _picked_obj;
    private int _originalPickedObjLayer;
    
    private int _currentHealth;
    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            _currentHealth = value;
            EventsManager.Instance?.PlayerHealthChanged(_currentHealth);
        }
    }

    private bool _invincible = false;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _rangeIndicator.transform.localScale = new Vector3(_rangeRadius, 0.2f, _rangeRadius);
    }

    private void Start() {
        CurrentHealth = _maxHealth;

        if(WavesManager.Instance)
            WavesManager.Instance.OnGameFinished += OnGameFinished;
    }

    private void Update() {
        _input.Normalize();

        _currentSpeed = _speed * _input.magnitude;

        if (_picked_obj) {
            if (Vector3.Distance(_picked_rb.transform.position, transform.position) <= _rangeRadius) {
                _picked_obj.Activate(Vector3.Distance(_picked_rb.position, transform.position));
            }
        }
    }

    private void FixedUpdate() {
        Vector3 movementDirection = _input.normalized;
        Vector3 newPosition = _rigidBody.position + _currentSpeed * Time.fixedDeltaTime * movementDirection;
        _rigidBody.MovePosition(newPosition);

        if (_picked_rb is null)
            return;

        if (_picked_rb.gameObject is null)
        {
            _picked_rb = null;
            return;
        }
        float frameRot = _scrollInput * _scrollSpeed * Time.fixedDeltaTime;
        Vector3 axis = Mathf.Sign(_scrollInput) * Vector3.up;
        _picked_rb.MoveRotation(Quaternion.RotateTowards(_picked_rb.rotation, _picked_rb.rotation * Quaternion.AngleAxis(frameRot, axis), frameRot));
    }

    private void OnDestroy() {
        if(WavesManager.Instance)
            WavesManager.Instance.OnGameFinished -= OnGameFinished;
    }

    private void OnCollisionEnter(Collision collision) {
        HandleHostileCollision(collision);
    }

    private void OnCollisionStay(Collision collision) {
        HandleHostileCollision(collision);
    }

    private void HandleHostileCollision(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            ReceiveDamage(collision.gameObject.GetComponent<Enemy>().Power);
        }
    }

    public void ReceiveDamage(int amount) {
        if (_invincible || _currentHealth <= 0) return;
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, _maxHealth);
        if (CurrentHealth <= 0) {
            Die();
        } else {
            _invincible = true;
            StartCoroutine(StartInvincibilityTimer());
        }
    }

    private IEnumerator StartInvincibilityTimer() {
        yield return new WaitForSeconds(_invincibilityDuration);
        _invincible = false;
    }

    private void Die() {
        EventsManager.Instance.PlayerDied();
        Destroy(gameObject);
    }

    public void OnMove(InputAction.CallbackContext context) {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _input.Set(moveInput.x, 0f, moveInput.y);
    }

    public void OnPoint(InputAction.CallbackContext context) {
        Vector2 mousePos = context.ReadValue<Vector2>();

        if (_picked_rb == null)
            return;

        Plane virtualPlane = new Plane(_referencePlaneTransform.up, _referencePlaneTransform.position);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (virtualPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 targetPosition = hitPoint + Vector3.up * _pickedObjectYOffset;
            Vector3 toTarget = targetPosition - _picked_obj.transform.position;

            var draggableComponent = _picked_obj.GetComponent<Draggable>();
            float pickedWeight = Mathf.Min(draggableComponent.Weight, maxDraggableWeight);

            toTarget = (1 - pickedWeight / maxDraggableWeight) * toTarget;

            _picked_rb.MovePosition(_picked_obj.transform.position + toTarget);
        }
    }

    public void OnObjectPicked(GameObject obj) {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        PlayerActivableObject ao = obj.GetComponent<PlayerActivableObject>();
        if (rb && ao) {
            _originalPickedObjLayer = obj.layer;
            obj.layer = LayerMask.NameToLayer("PickedObject");
            _picked_rb = rb;
            _picked_obj = ao;
            PlayerObject po = obj.GetComponent<PlayerObject>();
            po.OnPlayerHold();
            po.OnMerged.AddListener(OnHeldObjectMerged);
        }
    }


    public void OnObjectReleased() {
        if(_picked_rb) {
            _picked_rb.gameObject.layer = _originalPickedObjLayer;
            PlayerObject po = _picked_rb.GetComponent<PlayerObject>();
            po.OnPlayerRelease();
            po.OnMerged.RemoveListener(OnHeldObjectMerged);
        }
        _picked_rb = null;
        _picked_obj = null;
        _scrollInput = 0;
    }


    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (_picked_rb is null)
            return;

        if (context.started)
            _scrollInput = context.ReadValue<float>();
        else if (context.canceled)
            _scrollInput = 0f;
    }


    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (_picked_rb is null)
            return;

        if (context.started)
            _scrollInput = -context.ReadValue<float>();
        else if (context.canceled)
            _scrollInput = 0f;
    }

    public void OnHeldObjectMerged(GameObject newObj) {
        OnObjectReleased();
        OnObjectPicked(newObj);
    }

    public void OnHit(float value) {
        ReceiveDamage((int)value);
    }

    private void OnGameFinished(bool victory) {
        _invincible = victory;
    }
}
