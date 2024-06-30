using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


[RequireComponent(typeof(BoxCollider), typeof(PlayerObject), typeof(PlayerActivableObject))]
public class Wall : MonoBehaviour, IDamageable
{

    [SerializeField] private float _wallLength = 4;
    [SerializeField] private float _timeToExpand = 0.5f;

    [SerializeField] private GameObject leftPole;
    [SerializeField] private GameObject rightPole;
    [SerializeField] private GameObject wall;

    private bool _expanding = false;
    private float _currentHealth = 10.0f;
    private bool _protectionActive = false;
    private BoxCollider _boxCollider;
    private PlayerObject _playerObject;

    private Queue<bool> _wallStateChangeQueue;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var leftPoleLocalPos = leftPole.transform.localPosition;
        leftPoleLocalPos.x = -0.5f;
        leftPole.transform.localPosition = leftPoleLocalPos;

        var rightPoleLocalPos = rightPole.transform.localPosition;
        rightPoleLocalPos.x = 0.5f;
        rightPole.transform.localPosition = rightPoleLocalPos;
        _boxCollider = GetComponent<BoxCollider>();
        _playerObject = GetComponent<PlayerObject>();
        _playerObject.OnPlayerHoldEvent += OnHold;
        _playerObject.OnPlayerReleaseEvent += OnRelease;
        _wallStateChangeQueue = new Queue<bool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_wallStateChangeQueue.Count == 0 || _expanding)
            return;

        bool nextState = _wallStateChangeQueue.Dequeue();
        UpdateWallState(nextState);
    }

    void UpdateWallState(bool expand)
    {
        _protectionActive = expand;
        StartCoroutine(ChangeWallState(expand));
    }

    public void OnActivate()
    {
        
    }

    public void OnHold()
    {
        _wallStateChangeQueue.Enqueue(true);
    }

    public void OnRelease()
    {
        _wallStateChangeQueue.Enqueue(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + 0.5f *_wallLength * Vector3.right, 0.2f);
        Gizmos.DrawWireSphere(transform.position - 0.5f *_wallLength * Vector3.right, 0.2f);
    }

    private IEnumerator<WaitForSeconds> ExpandWall() => ChangeWallState(true);
    private IEnumerator<WaitForSeconds> CloseWall() => ChangeWallState(false);

    private IEnumerator<WaitForSeconds> ChangeWallState(bool expanded)
    {
        const float step = 1.0f / 60.0f;
        float elapsedTime = 0.0f;

        _expanding = true;
        while (elapsedTime < _timeToExpand + step)
        {
            float t = elapsedTime / _timeToExpand;
            if (!expanded)
                t = 1.0f - t;

            var leftPolePosition = leftPole.transform.localPosition;
            leftPolePosition.x = Mathf.Lerp(-0.5f, -_wallLength * 0.5f, t);
            leftPole.transform.localPosition = leftPolePosition;

            var rightPolePosition = rightPole.transform.localPosition;
            rightPolePosition.x = Mathf.Lerp(0.5f, _wallLength * 0.5f, t);
            rightPole.transform.localPosition = rightPolePosition;

            var wallScale = wall.transform.localScale;
            wallScale.x = Mathf.Lerp(1.0f, _wallLength, t);
            wall.transform.localScale = wallScale;

            var boxSize = _boxCollider.size;
            boxSize.x = Mathf.Lerp(2.0f, _wallLength, t);
            _boxCollider.size = boxSize;

            yield return new WaitForSeconds(step);
            elapsedTime += step;
        }
        _expanding = false;
    }

    void IDamageable.OnHit(float value)
    {
        if (_protectionActive)
            _currentHealth = Mathf.Max(0, _currentHealth - value);

        Debug.Log($"Current wall heatlh: {_currentHealth}. Damage taken: {value}");
        if (_currentHealth == 0.0f)
            Destroy(gameObject);
    }
}
