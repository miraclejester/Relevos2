using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour {
    public static EventsManager Instance { get; private set; }

    public UnityAction OnPlayerDied;
    public UnityAction<int> OnPlayerHealthChanged;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public void PlayerDied() {
        OnPlayerDied.Invoke();
    }

    public void PlayerHealthChanged(int amount) {
        OnPlayerHealthChanged.Invoke(amount);
    }
}
