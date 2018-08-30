using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BismuthController : MonoBehaviour
{
    [SerializeField] Transform _barPrefab;
    List<GameObject> _loops = new List<GameObject>();

    [SerializeField] int _steps = 20;

    [SerializeField] float _radius = 1.0f;
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _offset = 0.0f;
    float _birth = 0.0f;
    float _death = 1.0f;
    float _lifeSpeed = 1.0f;
    int _rotation;

    void Start()
    {
        _steps = Random.Range(3, 12);
        _rotation = Random.Range(0, 1) * 2 - 1;

        for (int i = 1; i < _steps; i++)
        {
            float k = 1.5f;
            float fi = (float)i / 10f * k;
            float sx = fi * 10f;

            float height = 0.5f;
            float offset = 0.1f * height;

            var go = new GameObject("Loop");
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(Mathf.PerlinNoise(1, fi + Time.time), Mathf.PerlinNoise(2, fi + Time.time), fi * height);
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);

            var o0 = Instantiate(_barPrefab, go.transform);
            o0.localPosition = new Vector3(offset, fi, 0);
            o0.localRotation = Quaternion.Euler(0, 0, 0);
            o0.localScale = new Vector3(sx, .5f, 1);

            var o1 = Instantiate(_barPrefab, go.transform);
            o1.localPosition = new Vector3(fi, -offset, 0);
            o1.localRotation = Quaternion.Euler(0, 0, 90);
            o1.localScale = new Vector3(sx, .5f, 1);

            var o2 = Instantiate(_barPrefab, go.transform);
            o2.localPosition = new Vector3(-fi, offset, 0);
            o2.localRotation = Quaternion.Euler(0, 0, 90);
            o2.localScale = new Vector3(sx, .5f, 1);

            var o3 = Instantiate(_barPrefab, go.transform);
            o3.localPosition = new Vector3(-offset, -fi, 0);
            o3.localRotation = Quaternion.Euler(0, 0, 0);
            o3.localScale = new Vector3(sx, .5f, 1);

            _loops.Add(go);
        }

        _birth = Time.time;
        _lifeSpeed = Random.Range(0.2f, 0.8f);

        transform.localScale = new Vector3(0, 0, 0);
    }

    void Update()
    {
        transform.Rotate(-0.07f * _speed, 0.1f * _speed, 0.05f * _speed);
        transform.position = transform.rotation * Vector3.forward * _radius;
        _radius += 0.043f * _speed;

        var t = (Time.time - _birth) * _lifeSpeed;
        var scale = 1 - Mathf.Pow(t - 1, 2);
        transform.localScale = new Vector3(1, 1, 1) * scale;

        var i = 1;
        _loops.ForEach((loop) => {
            float tt = (1 - scale) * (100 - _steps * 5) * (i + 1);
            loop.transform.localRotation = Quaternion.Euler(0, 0, tt * _rotation);
            i++;
        });

        // Die
        if (t > 2) {
            transform.localScale = new Vector3(1, 1, 1) * 0;
            Destroy(gameObject);
        }
    }
}
