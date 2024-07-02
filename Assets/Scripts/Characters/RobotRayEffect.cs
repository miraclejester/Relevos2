using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobotRayEffect : MonoBehaviour
{
    public GameObject baseRay;
    private List<Transform> targets = new();
    private List<Transform> targetRays = new();
    private GameObject latestPicked;
    private PlayerObject po;
    private UnityAction<GameObject> mergeAction;
    private int latestPickedIdx;

    private void Start()
    {
        baseRay.SetActive(false);
        mergeAction = (no) =>
        {
            OnObjectReleased();
            AddRay(no);
        };
    }


    public void AddRay(GameObject target)
    {
        targets.Add(target.transform);
        GameObject ray = Instantiate(baseRay, baseRay.transform.parent);
        targetRays.Add(ray.transform);
        ray.SetActive(true);
    }


    public void Update()
    {
        UpdateRays();
    }

    public void UpdateRays()
    {
        for (int i = 0; i < targetRays.Count; ++i)
        {
            Transform t = targets[i];
            Transform r = targetRays[i];

            Vector3 dir = (t.position - r.position);
            float dist = dir.magnitude;

            r.localScale = new(dist, r.localScale.y, r.localScale.z);
            r.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.up);

            Debug.DrawLine(r.position, r.position + dir, Color.red);
        }
    }

    public void OnObjectPicked(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        PlayerActivableObject ao = obj.GetComponent<PlayerActivableObject>();
        if (rb && ao)
        {
            latestPickedIdx = targets.Count;
            latestPicked = rb.gameObject;
            po = latestPicked.GetComponent<PlayerObject>();
            AddRay(rb.gameObject);

            po.OnMerged.AddListener(mergeAction);
        }
    }

    public void OnObjectReleased()
    {
        if (latestPicked is null)
            return;

        po.OnMerged.RemoveListener(mergeAction);
        Destroy(targetRays[latestPickedIdx].gameObject);
        targets.RemoveAt(latestPickedIdx);
        targetRays.RemoveAt(latestPickedIdx);
    }
}
