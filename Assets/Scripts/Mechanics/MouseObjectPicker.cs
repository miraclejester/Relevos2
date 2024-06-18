using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MouseObjectPicker : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private UnityEvent<GameObject> _OnObjectPicked;
    [SerializeField] private UnityEvent _OnObjectReleased;

    private GameObject _currentObject = null;

    public void OnClick(InputAction.CallbackContext context) {
        float clickValue = context.ReadValue<float>();
        if (clickValue == 1f) {
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 5000, _collisionLayers);
            if (hit) {
                _currentObject = hitInfo.transform.gameObject;
                _OnObjectPicked.Invoke(_currentObject);
            }
        } else {
            _currentObject = null;
            _OnObjectReleased.Invoke();
        }
    }
}
