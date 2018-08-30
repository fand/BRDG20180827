using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene6GameController : MonoBehaviour
{
    [SerializeField] Transform _crystalPrefab;
    [SerializeField] Material _crystalMaterial;
    [SerializeField] Camera _sceneCamera;
    [SerializeField] float offsetRange = 2.0f;
    [SerializeField] GameObject _crystals;

    List<Transform> _objects = new List<Transform>();
    int _index = 0;
    [SerializeField, Range(10, 200)] int _arraySize = 50;

    float _time = 0;
    float _clockSpeed = 1;
    float _radius = 3.5f;
    float _nextBirth = 0;

    float _partyCounter = 0;
    bool _isParty = false;
    Color _originalEmission;

    void Start()
    {
        _originalEmission = _crystalMaterial.GetColor("_EmissionColor");
    }

    bool Party(float speed, float x)
    {
        _partyCounter += 0.001f;
        return _isParty && (Mathf.PerlinNoise(_partyCounter, (Time.time * speed) % 1) < x);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isParty = !_isParty;
        }

        // Accelerate
        if (Input.GetKey(KeyCode.Q) || Party(1.7f, 0.3f))
        {
            _clockSpeed = 5 - (5 - _clockSpeed) * 0.1f;
        }
        else
        {
            _clockSpeed = (_clockSpeed - 1) * 0.4f + 1;
        }

        _time += Time.deltaTime * _clockSpeed;

        // Wiggle
        if (Input.GetKey(KeyCode.W) || Party(0.3f, 0.03f))
        {
            _sceneCamera.transform.position = _sceneCamera.transform.position +
                _sceneCamera.transform.right * Mathf.Sin(_time * 2923) * .9f +
                _sceneCamera.transform.up * Mathf.Sin(_time * 3233) * .4f;
            _sceneCamera.transform.position *= _radius / _sceneCamera.transform.position.magnitude;
        }

        _crystals.transform.rotation = Quaternion.Euler(
            _time * 31,
            _time * 80,
            _time * 53
        );

        // Evolve
        if (Input.GetKey(KeyCode.E) || Party(0.2f, 0.2f))
        {
            _arraySize = 80;
            _nextBirth = 0;
        }
        else {
            _arraySize = Mathf.Max(_arraySize - 1, 30);
        }

        // Rotate
        if (Input.GetKey(KeyCode.R) || Party(0.1f, 0.3f))
        {
            _radius = 3.5f + Mathf.PerlinNoise(_time, 294) * 2.0f;

            var i = 1;
            _objects.ForEach(o => {
                if (o != null) {
                    o.transform.Rotate(0, 0, 10 * Mathf.Sin(i * 76));
                    i++;
                }
            });
        }
        _sceneCamera.transform.LookAt(Vector3.zero);

        // Transform
        if (Input.GetKey(KeyCode.T) || Party(0.003f, 0.17f))
        {
            float s = (0.8f + ((_time * 3892) % 1) * 4f);
            _crystals.transform.localScale = new Vector3(s,s,1);
        }

        // Yo
        var ec = _originalEmission;
        if (Input.GetKey(KeyCode.Y) || Party(0.7f, 0.1f))
        {
            ec = Color.red * 6 * Mathf.Sin(_time * 940);
        }
        _crystalMaterial.SetColor("_EmissionColor", ec);

        if (_time > _nextBirth)
        {
            float s = 20; // size
            var position = new Vector3(
                Random.Range(-s, s),
                Random.Range(-s, s),
                Random.Range(-s, s)
            );
            var rotation = Quaternion.Euler(
                Random.Range(-90, 90),
                Random.Range(-90, 90),
                Random.Range(-90, 90)
            );

            UpdateArraySize(); // must be run before using _objects

            // Make random offset
            position += _sceneCamera.transform.up * Random.Range(-offsetRange, offsetRange);
            position += _sceneCamera.transform.right * Random.Range(-offsetRange, offsetRange);

            var o = Instantiate(_crystalPrefab, position, rotation, _crystals.transform);
            o.localScale = new Vector3(Random.Range(0.8f, 1.7f), Random.Range(0.8f, 1.8f), 1);

            if (_objects[_index])
            {
                Destroy(_objects[_index].gameObject);
            }
            _objects[_index] = o;
            _index = (_index + 1) % _arraySize;

            _nextBirth = _time + Random.Range(0.03f, .2f);
        }
    }

    void UpdateArraySize()
    {
        if (_objects.Count > _arraySize)
        {
            _objects = _objects.GetRange(0, _arraySize);
        }
        else if (_objects.Count < _arraySize) {
            _objects.AddRange(new List<Transform>(new Transform[_arraySize - _objects.Count]));
        }

        _index = _index % _arraySize;
    }
}
