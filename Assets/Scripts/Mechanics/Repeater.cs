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

    private bool _connected = true;

    Collider[] _colliders = new Collider[1];

    private void Update()
    {
        CheckConnection();
        Activate();
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
            _linkedObj.Activate();
        }
    }
}
