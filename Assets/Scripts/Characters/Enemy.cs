using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PlayerProjectile")) {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
