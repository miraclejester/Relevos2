using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private float _speed;

    [Header("Powers")]
    [SerializeField] private GameObject _rangeIndicator;
    [SerializeField] private float _rangeRadius = 5f;
    [SerializeField] private Transform _referencePlaneTransform;
    [SerializeField] private float _pickedObjectYOffset = 0.8f;
    [SerializeField] private float maxDraggableWeight = 20;

    private Vector3 _input;
    private float _currentSpeed;
    private Rigidbody _rigidBody;
    private Rigidbody _picked_rb;
    private PlayerActivableObject _picked_obj;
    private int _originalPickedObjLayer;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _rangeIndicator.transform.localScale = new Vector3(_rangeRadius, 0.2f, _rangeRadius);
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
    }

    public void OnMove(InputAction.CallbackContext context) {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _input.Set(moveInput.x, 0f, moveInput.y);
    }

    public void OnPoint(InputAction.CallbackContext context) {
        Vector2 mousePos = context.ReadValue<Vector2>();
        if (_picked_rb) {
            Plane virtualPlane = new Plane(_referencePlaneTransform.up, _referencePlaneTransform.position);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (virtualPlane.Raycast(ray, out float enter)) {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 targetPosition = hitPoint + Vector3.up * _pickedObjectYOffset;
                Vector3 toTarget = targetPosition - _picked_obj.transform.position;

                var draggableComponent = _picked_obj.GetComponent<Draggable>();
                float pickedWeight = Mathf.Min(draggableComponent.Weight, maxDraggableWeight);
                
                toTarget = (1 - pickedWeight/maxDraggableWeight) * toTarget;

                _picked_rb.MovePosition(_picked_obj.transform.position + toTarget);
            }
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
        }
    }

    public void OnObjectReleased() {
        if(_picked_rb) {
            _picked_rb.gameObject.layer = _originalPickedObjLayer;
        }
        _picked_rb = null;
        _picked_obj = null;
    }

}
