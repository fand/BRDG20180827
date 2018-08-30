using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2GameController : MonoBehaviour
{
    [SerializeField] Camera _sceneCamera;
    [SerializeField] GameObject _particles;
    [SerializeField] GameObject _building;
    [SerializeField] Material _material;

    // For FX
    float _time = 0;
    float _clockSpeed = 1;
    bool _isParty = false;
    float _partyCounter = 0;
    Color _originalColor;
    float _rot = 0;
    float _pipeWidth = 1;

    // Managing Pipes
    [SerializeField] GameObject pipePrefab;
    List<Vector3> _points = new List<Vector3>();
    Vector3 _center = Vector3.zero;
    Vector3 _nextCenter = Vector3.up;
    int NUM = 8; // Points per Grow()
    float _nextGrow = 0;
    int _birthRate = 1;
    List<GameObject> _objects = new List<GameObject>();
    int _index = 0;
    [SerializeField, Range(20, 100)] int _arraySize = 50;

    void Start()
    {
        _originalColor = _material.GetColor("_EmissionColor");

        for (int i = 0; i < NUM; i++)
        {
            _points.Add(Vector3.zero);
        }
        Grow();
    }

    bool Party(float speed, float x)
    {
        _partyCounter++;
        return _isParty && Mathf.PerlinNoise(_partyCounter, Time.time * speed) < x;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isParty = !_isParty;
        }

        // Accelerate
        if (Input.GetKey(KeyCode.Q) || Party(0.3f, 0.2f))
        {
            _clockSpeed = 4;
        }
        else
        {
            _clockSpeed = 1;
        }

        _time += Time.deltaTime * _clockSpeed;

        // Wiggle
        if (Input.GetKey(KeyCode.W) || Party(1, 0.2f))
        {
            transform.position = new Vector3(
                0,
                Mathf.Sin(_time * 524) * .25f,
                Mathf.Sin(_time * 924) * .1f
            );
        }

        // Evolve
        if (Input.GetKey(KeyCode.E) || Party(0.3f, .4f))
        {
            _birthRate = 5;
        }
        else
        {
            _birthRate = 1;
        }

        // Rotate Scene
        if (Input.GetKey(KeyCode.R) || Party(1.5f, 0.2f))
        {
            _rot += 30;
        }

        // Transform
        if (Input.GetKey(KeyCode.T) || Party(0.0000003f, 0.4f))
        {
            _pipeWidth = 1 + Mathf.Sin(_time * 492) * 2.8f;
        }

        // Yo
        Color color;
        if (Input.GetKey(KeyCode.Y) || Party(0.0000003f, 0.4f))
        {
            float hue = (Mathf.PerlinNoise(1, _time * 0.04f) * 5) % 1;
            color = UnityEngine.Color.HSVToRGB(hue, 2, 0.5f, true);
        }
        else
        {
            if (Mathf.PerlinNoise(1, _time * 0.08f) > .5f)
            {
                color = new Color(0, 2, 3, 1);
            }
            else {
                color = new Color(5, 0.2f, 0.2f, 1);
            }

        }
        _material.SetColor("_EmissionColor", color);

        UpdateCamera();
    }

    void UpdateCamera()
    {
        // Get interpolated position as camera target;
        var target = (_nextCenter - _center) * 0.3f + _center;

        var dir = (Quaternion.Euler(new Vector3(
            _time * 20,
            _time * 50,
            _time * 40
        ) + Vector3.up * _rot)) * Vector3.up * (3 + Mathf.Sin(_time) * 0.3f);

        _sceneCamera.transform.position = target + dir;
        _particles.transform.position = _nextCenter;
        _sceneCamera.transform.LookAt(target);

        _center = target;

        if (_time > _nextGrow)
        {
            Grow();
            _nextGrow = _time + Random.Range(0.4f, 0.8f);
        }
    }

    void Grow()
    {
        var newPoints = new List<Vector3>();
        var center = Vector3.zero;

        for (int i = 0; i < NUM * _birthRate; i++)
        {
            var p1 = _points[i % _points.Count];
            var p2 = _points[(i + 1) % _points.Count];
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
        center /= NUM * _birthRate;

        _center = _nextCenter;
        _nextCenter = center;
    }

    void FromTo(Vector3 p1, Vector3 p2)
    {
        var p12 = p2 - p1;
        var rot = Quaternion.FromToRotation(Vector3.up, p12);

        UpdateArraySize(); // must be run before using _objects

        var o = Instantiate(pipePrefab, transform.position + p1, rot, _building.transform);
        if (_objects[_index])
        {
            Destroy(_objects[_index].gameObject);
        }
        _objects[_index] = o;
        _index = (_index + 1) % _arraySize;

        var ctrl = o.GetComponent<PipeController>();
        ctrl.SetLength(p12.magnitude);
        ctrl.SetWidth(_pipeWidth);
        Destroy(o, 3);
    }

    void UpdateArraySize()
    {
        if (_objects.Count > _arraySize)
        {
            for (int i = _arraySize; i < _objects.Count; i++)
            {
                if (_objects[i])
                {
                    Destroy(_objects[i].gameObject);
                }
            }
            _objects = _objects.GetRange(0, _arraySize);
        }
        else if (_objects.Count < _arraySize) {
            _objects.AddRange(new List<GameObject>(new GameObject[_arraySize - _objects.Count]));
        }

        _index = _index % _arraySize;
    }
}
