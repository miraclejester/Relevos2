using System.Collections;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _optimalActivationDistance;
    [SerializeField] private float _maxActivationDistance;

    private float _currentFireRate;

    private bool _canFire = true;

    public void fire(float distanceToActivator)
    {
        if (!canFire(distanceToActivator)) return;

        UpdateCurrentFireRate(distanceToActivator);
        Projectile instance = Instantiate<Projectile>(_projectilePrefab);
        instance.SetDirection(_projectileSpawnPoint.forward.normalized);
        instance.transform.position = _projectileSpawnPoint.position;
        StartCoroutine(StartFireTimer());
    }

    private bool canFire(float distanceToActivator) => _canFire && distanceToActivator <= _maxActivationDistance;


    private void UpdateCurrentFireRate(float distanceToActivator)
    {
        Debug.Assert(_maxActivationDistance >= _optimalActivationDistance, 
            "Max Activation distance should be lower than the optimal activation instance!"
        );
        if (distanceToActivator <= _optimalActivationDistance)
        {
            _currentFireRate = _fireRate;
            return;
        }

        float t = (distanceToActivator - _optimalActivationDistance) / (_maxActivationDistance - _optimalActivationDistance);
        _currentFireRate = Mathf.Lerp(
                _fireRate,
                _fireRate * 10, 
                t 
        );
    }

    private IEnumerator StartFireTimer()
    {
        _canFire = false;
        yield return new WaitForSeconds(_currentFireRate);
        _canFire = true;
    }
}
