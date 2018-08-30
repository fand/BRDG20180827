using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene5GameController : MonoBehaviour
{
    [SerializeField] Camera _sceneCamera;
    [SerializeField] GameObject _particles;
    [SerializeField] Material _material;
    float _time = 0;
    float _clockSpeed = 1;
    float _radius = 5;
    bool _isParty = false;
    float _partyCounter = 0;

    ParticleSystem[] _particleSystems;

    void Start()
    {
        _particleSystems = _particles.GetComponentsInChildren<ParticleSystem>();
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
        if (Input.GetKey(KeyCode.Q) || Party(1, 0.3f))
        {
            _clockSpeed = 5 - (5 - _clockSpeed) * 0.1f;
        }
        else
        {
            _clockSpeed = (_clockSpeed - 2) * 0.4f + 2;
        }

        _time += Time.deltaTime * _clockSpeed;

        // Wiggle
        if (Input.GetKey(KeyCode.W) || Party(0.5f, 0.2f))
        {
            _radius += Mathf.Sin(_time * 3230) * 1.0f;
        }

        // Evolve
        if (Input.GetKey(KeyCode.E) || Party(0.3f, .4f))
        {
            foreach (var s in _particleSystems) { s.emissionRate = 200; }
        }
        else
        {
            foreach (var s in _particleSystems) { s.emissionRate = 20; }
        }

        // Rotate Scene
        if (Input.GetKey(KeyCode.R) || Party(1.5f, 0.5f))
        {
            transform.rotation = Quaternion.Euler(
                _time * 20,
                _time * 60,
                _time * 37
            );
            _radius = 5 + (Mathf.Sin(_time) + Mathf.Cos(_time * 1.7f)) * 7;
        }
        else
        {
            // _radius = 5;
        }
        _sceneCamera.transform.position *= _radius / _sceneCamera.transform.position.magnitude;
        _sceneCamera.transform.LookAt(Vector3.zero);

        // Transform
        if (Input.GetKey(KeyCode.T) || Party(0.3f, 0.2f))
        {
            foreach (var s in _particleSystems) { var t = s.trails; t.mode = ParticleSystemTrailMode.Ribbon; }
        }
        else
        {
            foreach (var s in _particleSystems) { var t = s.trails; t.mode = ParticleSystemTrailMode.PerParticle; }
        }

        // Yo
        if (Input.GetKey(KeyCode.Y) || Party(0.3f, 0.2f))
        {
            float e = Mathf.PerlinNoise(1, _time * 294) * 1.3f;
            _material.SetColor("_EmissionColor", new Color(e,e,e,1));
        }
        else
        {
            float e = 0.5f;
            _material.SetColor("_EmissionColor", new Color(e,e,e,1));
        }

        // Constant motion
        _particles.transform.rotation = Quaternion.Euler(
            Time.time * 40,
            Time.time * 30,
            Time.time * 20
        );
    }
}
