using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float _damage = 3f;

    [SerializeField]
    private float _range = 5f;

    [SerializeField]
    private float _explosionTime = 3f;

    [SerializeField]
    private LayerMask _layer;

    public void StartTimer()
    {
        StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(_explosionTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _range, _layer);

        foreach (Collider other in colliders)
        {
            if(other.TryGetComponent(out IDamageable health))
            {
                health.OnHit(_damage);
            }
        }

        Destroy(gameObject);
    }
}
