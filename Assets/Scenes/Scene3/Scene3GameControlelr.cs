using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3GameControlelr : MonoBehaviour
{
    [SerializeField] Camera _sceneCamera;
    [SerializeField] Material _cubeMaterial;
    [SerializeField] Material _sphereMaterial;
    [SerializeField] GameObject _cubeParticleObject;
    [SerializeField] GameObject _sphereParticleObject;
    ParticleSystem _cubeParticles;
    ParticleSystem _sphereParticles;

    float _time = 0;
    float _clockSpeed = 1.0f;
    float _radius = 10;
    bool _isParty = false;
    float _partyCounter = 0;
    float _rot = 0;

    void Start ()
    {
        _cubeParticles = _cubeParticleObject.GetComponent<ParticleSystem>();
        _sphereParticles = _sphereParticleObject.GetComponent<ParticleSystem>();
    }

    bool Party(float speed, float x)
    {
        _partyCounter +=0.01f;
        return _isParty && Mathf.PerlinNoise(Time.time * speed, _partyCounter * 2499) < x;
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isParty = !_isParty;
        }

        // Accelerate
        if (Input.GetKey(KeyCode.Q) || Party(1.7f, 0.4f))
        {
            _clockSpeed = 4;
        }
        else
        {
            _clockSpeed = 1;
        }
        _time += Time.deltaTime * _clockSpeed;
        _cubeParticles.playbackSpeed = _clockSpeed;
        _sphereParticles.playbackSpeed = _clockSpeed;

        // Wiggle
        if (Input.GetKey(KeyCode.W) || Party(1, 0.2f))
        {
            _sceneCamera.transform.position = _sceneCamera.transform.position +
                (
                    _sceneCamera.transform.right * (Mathf.PerlinNoise(1, _time * 50) * 2 - 1) * 1.2f +
                    _sceneCamera.transform.up * (Mathf.PerlinNoise(2, _time * 80) * 2 - 1) * .7f
                );
            _sceneCamera.transform.position *= _radius / _sceneCamera.transform.position.magnitude;
        }

        // Rotate Scene
        if (Input.GetKey(KeyCode.R) || Party(0.04f, 0.3f))
        {
            _rot += 4;
        }
        var dir = Quaternion.Euler(
            _time * 20 + _rot,
            _time * 40 + _rot,
            _time * 30 + _rot * 2
        ) * Vector3.up;
        _sceneCamera.transform.position = dir * _radius;

        _sceneCamera.transform.position *= _radius / _sceneCamera.transform.position.magnitude;
        _sceneCamera.transform.LookAt(Vector3.zero);

        // Evolve
        var emit = 1;
        if (Input.GetKey(KeyCode.E) || Party(0.3f, .4f))
        {
            emit = 3;
        }

        // Transform
        if (Input.GetKey(KeyCode.T) || Party(0.3f, 0.3f))
        {
            _cubeParticles.emissionRate = 4 * emit;
            _sphereParticles.emissionRate = 20 * emit;
        }
        else
        {
            _cubeParticles.emissionRate = 20 * emit;
            _sphereParticles.emissionRate = 7 * emit;
        }

        if (Input.GetKey(KeyCode.Y) || Mathf.PerlinNoise(Time.time * 2f, 10) < 0.5f)
        {
            _cubeMaterial.SetColor("_Emission", Color.red);
            _sphereMaterial.SetColor("_Emission", Color.white);
        }
        else
        {
            _cubeMaterial.SetColor("_Emission", Color.black);
            _sphereMaterial.SetColor("_Emission", Color.black);
        }

        _cubeMaterial.SetFloat("_Speed", _clockSpeed * 0.01f);
        _sphereMaterial.SetFloat("_Speed", _clockSpeed * 0.01f);
    }
}
