using UnityEngine;

public class EnemyShootingBehaviour : MonoBehaviour {
    [SerializeField] private ProjectileSpawner _projectileSpawner;

    private GameObject _target;

    private void Start() {
        _target = GameObject.FindGameObjectWithTag("Player");
        EventsManager.Instance.OnPlayerDied += OnPlayerDied;
    }

    private void Update() {
        if (_target == null) return;
        _projectileSpawner.transform.LookAt(_target.transform);
        _projectileSpawner.fire(0);
    }

    private void OnDestroy() {
        EventsManager.Instance.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied() {
        _target = null;
    }
}
