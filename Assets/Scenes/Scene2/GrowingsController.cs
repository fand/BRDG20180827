using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingsController : MonoBehaviour
{
    [SerializeField] GameObject pipePrefab;
    [SerializeField] Camera _sceneCamera;
    [SerializeField] GameObject _particles;
    List<Vector3> _points = new List<Vector3>();
    Vector3 _center = Vector3.zero;
    Vector3 _nextCenter = Vector3.up;

    int NUM = 8;

    void Start()
    {
        for (int i = 0; i < NUM; i++)
        {
            _points.Add(Vector3.zero);
        }
        Grow();
    }

    void Update ()
    {
        // Get interpolated position as camera target;
        var target = (_nextCenter - _center) * 0.3f + _center;

        var dir = Quaternion.Euler(
            Time.time * 20,
            Time.time * 50,
            Time.time * 40
        ) * Vector3.up * (3 + Mathf.Sin(Time.time) * 0.3f);

        _sceneCamera.transform.position = target + dir;
        _particles.transform.position = _nextCenter;
        _sceneCamera.transform.LookAt(target);

        _center = target;

        if (Time.frameCount % 30 == 0)
        {
            Grow();
        }
    }

    void Grow()
    {
        var newPoints = new List<Vector3>();
        var center = Vector3.zero;

        for (int i = 0; i < NUM; i++)
        {
            var p1 = _points[i];
            var p2 = _points[(i + 1) / _points.Count];
            var p12 = p1 + (p2 - p1) / 2;

            var rot = Quaternion.Euler(Random.Range(-50, 50), Random.Range(-90, 90), Random.Range(-50, 50));
            var nextPoint = p12 + (rot * Vector3.up) * 3;

            FromTo(p1, nextPoint);
            FromTo(p2, nextPoint);
            FromTo(p1, p2);

            newPoints.Add(nextPoint);
            center += nextPoint;
        }

        _points = newPoints;
        center /= NUM;

        _center = _nextCenter;
        _nextCenter = center;
    }

    void FromTo(Vector3 p1, Vector3 p2)
    {
        var p12 = p2 - p1;
        var rot = Quaternion.FromToRotation(Vector3.up, p12);
        var i = Instantiate(pipePrefab, transform.position + p1, rot, transform);
        i.GetComponent<PipeController>().SetLength(p12.magnitude);
        Destroy(i, 10);
    }
}
