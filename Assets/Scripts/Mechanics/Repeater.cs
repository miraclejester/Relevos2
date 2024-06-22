using System;
using UnityEngine;

public class Repeater : MonoBehaviour
{
    [SerializeField]
    private float _range = 8f;

    [SerializeField]
    private float _linkDistance = 20f;

    [SerializeField]
    private LayerMask _layer;

    [SerializeField]
    private PlayerActivableObject _linkedObj;

    [SerializeField]
    private Color _activeColor = Color.blue;

    [SerializeField]
    private Color _inactiveColor = Color.gray;

    private bool _connected = true;

    private bool _active = true;

    Vector3[] _linePositions;

    Collider[] _colliders = new Collider[1];

    private void Start()
    {
        _linePositions = new Vector3[2];
        _linkedObj.GetComponent<PlayerObject>().OnMerged.AddListener(OnLinkedObjectMerge);
    }

    private void Update()
    {
        CheckConnection();
        Activate();
        UpdateConnection();
    }

    private void CheckConnection()
    {
        if(_linkedObj)
        {
            if(Vector3.Distance(transform.position, _linkedObj.transform.position) > _linkDistance && _connected)
                _connected = false;
            else if(Vector3.Distance(transform.position, _linkedObj.transform.position) <= _linkDistance && !_connected)
                _connected = true;  
        }
    }

    private void Activate()
    {
        int collisions = Physics.OverlapSphereNonAlloc(transform.position, _range, _colliders, _layer);

        if(collisions > 0)
        {
            float distanceToLinked = Vector3.Distance(_linkedObj.transform.position, transform.position);
            _linkedObj.Activate(distanceToLinked);
        }
        _active = collisions > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    private void UpdateConnection()
    {
        var lineRenderer = GetComponent<LineRenderer>();

        if (_connected && _active)
        {
            lineRenderer.startColor = _activeColor;
            lineRenderer.endColor = _activeColor;
        }
        else
        {
            lineRenderer.startColor = _inactiveColor;
            lineRenderer.endColor = _inactiveColor;
        }

        _linePositions[0] = transform.position;
        _linePositions[1] = _linkedObj.transform.position;
        lineRenderer.SetPositions(_linePositions);
    }

    private void OnLinkedObjectMerge(GameObject newObj) {
        this._linkedObj = newObj.GetComponent<PlayerActivableObject>();
    }
}
