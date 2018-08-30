using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene4GameController : MonoBehaviour
{
    [SerializeField] Transform _lightPrefab;
    [SerializeField] Material _lightMaterial;
    [SerializeField] GameObject _lights;
    [SerializeField] GameObject _sceneCamera;

    List<Transform> _objects = new List<Transform>();
    int _index = 0;
    [SerializeField, Range(10, 200)] int _arraySize = 50;

    float _time = 0;
    float _clockSpeed = 1;
    float _red = 0;

    float _nextBirth = 0;

    float _partyCounter = 0;
    bool _isParty = false;

    bool Party(float speed, float x)
    {
        _partyCounter += 1;
        return _isParty && Mathf.PerlinNoise(0, Time.time * speed + _partyCounter) < x;
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.P))
        {
            _isParty = !_isParty;
        }

        // Accelerate
        if (Input.GetKey(KeyCode.Q) || Party(0.17f, 0.4f))
        {
            _clockSpeed = 5 - (5 - _clockSpeed) * 0.1f;
        }
        else
        {
            _clockSpeed = (_clockSpeed - 1) * 0.4f + 1;
        }

        _time += Time.deltaTime * _clockSpeed;

        // Wiggle
        if (Input.GetKey(KeyCode.W) || Party(1, 0.1f))
        {
            _sceneCamera.transform.position = _sceneCamera.transform.position +
                _sceneCamera.transform.right * Mathf.Sin(_time * 2923) * .9f +
                _sceneCamera.transform.up * Mathf.Sin(_time * 3233) * .4f;
        }

        _lights.transform.rotation = Quaternion.Euler(
            _time * 31,
            _time * 60,
            _time * 43
        );

        // Evolve
        if (Input.GetKey(KeyCode.E) || Party(0.2f, 0.3f))
        {
            _arraySize = 200;
            _nextBirth = 0;
        }
        else {
            _arraySize = Mathf.Max(_arraySize - 1, 50);
        }

        // Rotate
        if (Input.GetKey(KeyCode.R) || Party(1.1f, 0.3f))
        {
            _sceneCamera.transform.Rotate(2, 3, 14 * Mathf.PerlinNoise(84, _time) - 7);
        }

        // Transform
        if (Input.GetKey(KeyCode.T) || Party(0.7f, 0.4f))
        {
            _lightMaterial.SetFloat("_LightWidth", Mathf.Sin(_time) * 10 + 10);
        }

        // Yo
        if (Input.GetKey(KeyCode.Y) || Party(0.07f, 0.2f))
        {
            _red = Mathf.Sin(_time * 239) * .5f + .5f;
        }
        else
        {
            _red *= 0.8f;
        }
        _lightMaterial.SetFloat("_LightRed", _red);

        if (_time > _nextBirth)
        {
            float s = 15; // size
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

            var o = Instantiate(_lightPrefab, position, rotation, _lights.transform);
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
            _objects.AddRange(new List<Transform>(new Transform[_arraySize - _objects.Count]));
        }

        _index = _index % _arraySize;
    }
}
