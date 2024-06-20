using UnityEngine;
using UnityEngine.Events;

public class PlayerActivableObject : MonoBehaviour
{
    [SerializeField] private UnityEvent<float> _onActivate;

    public void Activate(float distanceToActivator) {
        _onActivate?.Invoke(distanceToActivator);
    }
}
