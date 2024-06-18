using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToLive;

    private Rigidbody rb;

    private Vector3 _movementDirection;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        StartCoroutine(startTimer());
    }

    public void SetDirection(Vector3 dir) {
        _movementDirection = dir;
    }

    private void FixedUpdate() {
        Vector3 newPos = transform.position + (_movementDirection.normalized * _speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    private IEnumerator startTimer() {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(gameObject);
    }
}
