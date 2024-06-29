using System;
using Unity.VisualScripting;
using UnityEngine;

public class Repeater : MonoBehaviour
{
    [SerializeField]
    private float _range = 8f;

    [SerializeField]
    private float _linkDistance = 20f;

    [SerializeField]
    private LayerMask _activateLayer;

    [SerializeField]
    private LayerMask _linkLayer;

    [SerializeField]
    private PlayerActivableObject _linkedObj;

    [SerializeField]
    private Color _activeColor = Color.blue;

    [SerializeField]
    private Color _inactiveColor = Color.gray;

    private bool _connected = false;
    private bool _active = false;

    private float _linkTimer = 0;
    private float _lastActivated = 0;

    Vector3[] _linePositions;
    Collider[] _colliders = new Collider[1];

    private LineRenderer _lineRenderer;

    private const float LINK_TIME = 1.5f;
    private const float ACTIVATE_COOLDOWN = 2f;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _linePositions = new Vector3[2];
        _lastActivated = Time.time;

        if(_linkedObj)
            _linkedObj.GetComponent<PlayerObject>().OnMerged.AddListener(OnLinkedObjectMerge);
    }

    private void Update()
    {
        CheckConnection();
        Activate();
        TriggerLink();
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
        if(Time.time - _lastActivated < ACTIVATE_COOLDOWN)
            return;

        int collisions = Physics.OverlapSphereNonAlloc(transform.position, _range, _colliders, _activateLayer);

        if(collisions > 0)
        {
            _active = !_active;
            _lastActivated = Time.time;
        }
    }

    private void TriggerLink()
    {
        if(_active)
        {
            float distanceToLinked = Vector3.Distance(_linkedObj.transform.position, transform.position);
            _linkedObj.Activate(distanceToLinked);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    private void UpdateConnection()
    {
        if(!_connected )
            return;

        _lineRenderer.startColor = _active ? _activeColor : _inactiveColor;
        _lineRenderer.endColor = _active ? _activeColor : _inactiveColor;

        _linePositions[0] = transform.position;
        _linePositions[1] = _linkedObj.transform.position;
        _lineRenderer.SetPositions(_linePositions);
    }

    private void OnLinkedObjectMerge(GameObject newObj) {
        this._linkedObj = newObj.GetComponent<PlayerActivableObject>();
    }

    private void NewLink(PlayerActivableObject obj)
    {
        if(_linkedObj)
            _linkedObj.GetComponent<PlayerObject>().OnMerged.RemoveListener(OnLinkedObjectMerge);

        _linkedObj = obj;
        _linkedObj.GetComponent<PlayerObject>().OnMerged.AddListener(OnLinkedObjectMerge);
    }

    private void OnTriggerStay(Collider other)
    {
        if((_linkLayer.value & 1 << other.gameObject.layer) != 0)
            return;

        _linkTimer += Time.deltaTime;

        if(_linkTimer > LINK_TIME)
        {
            if(other.TryGetComponent(out PlayerActivableObject obj))
            {
                NewLink(obj);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _linkTimer = 0;
    }
}
