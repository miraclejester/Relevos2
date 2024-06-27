using UnityEngine;

public class HealthDisplay : MonoBehaviour {

    [SerializeField] private float _iconWidth = 128f;

    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {
        EventsManager.Instance.OnPlayerHealthChanged += UpdateHealth;
    }

    private void OnDestroy() {
        EventsManager.Instance.OnPlayerHealthChanged -= UpdateHealth;
    }

    public void UpdateHealth(int amount) {
        _rectTransform.sizeDelta = new Vector2(_iconWidth * amount, _rectTransform.sizeDelta.y);
    }
}
