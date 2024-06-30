using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerObject : MonoBehaviour {
    [SerializeField] private PlayerObjectData _objectData;
    [HideInInspector] public UnityEvent<GameObject> OnMerged;

    private bool _held = false;
    public UnityAction OnPlayerHoldEvent;
    public UnityAction OnPlayerReleaseEvent;

    public string GetName() {
        return _objectData.ObjectName;
    }

    public PlayerObjectData GetData() {
        return _objectData;
    }


    private void OnCollisionEnter(Collision collision) {
        PlayerObject otherObject = collision.gameObject.GetComponent<PlayerObject>();
        if (otherObject && _held) {
            Merge(otherObject);
        }
    }

    private void Merge(PlayerObject other) {
        PlayerObjectMergeData mergeData = _objectData.GetMergeData(other.GetData());
        if (mergeData.mergeable) {
            GameObject newObj = Instantiate(mergeData.result.ObjectPrefab);
            newObj.transform.position = transform.position;
            OnMerged.Invoke(newObj);
            other.OnMerged.Invoke(newObj);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public void OnPlayerHold() {
        _held = true;
        OnPlayerHoldEvent?.Invoke();
    }

    public void OnPlayerRelease() {
        _held = false;
        OnPlayerReleaseEvent?.Invoke();
    }
}
