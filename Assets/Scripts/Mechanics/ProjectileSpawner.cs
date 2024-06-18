using System.Collections;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private float _fireRate;

    private bool _canFire = true;

    public void fire() {
        if (!_canFire) return;
        Projectile instance = Instantiate<Projectile>(_projectilePrefab);
        instance.SetDirection(_projectileSpawnPoint.forward.normalized);
        instance.transform.position = _projectileSpawnPoint.position;
        StartCoroutine(StartFireTimer());
    }

    private IEnumerator StartFireTimer() {
        _canFire = false;
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }
}
