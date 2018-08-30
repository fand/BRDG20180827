using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1GameController : MonoBehaviour
{
    [SerializeField] Camera _sceneCamera;
    [SerializeField] GameObject _particles;
    [SerializeField] GameObject _rayBox;
    [SerializeField] Material _material;
    [SerializeField] Material _particleMaterial;

    float _time = 0;
    float _clockSpeed = 1;
    float _radius = 5;
    bool _isParty = false;
    float _partyCounter = 0;
    float _wiggle = 0.2f;

    ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = _particles.GetComponent<ParticleSystem>();
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
            _clockSpeed = 10 - (10 - _clockSpeed) * 0.1f;
        }
        else
        {
            _clockSpeed = (_clockSpeed - 2) * 0.4f + 2;
        }

        _time += Time.deltaTime * _clockSpeed;

        // Wiggle
        if (Input.GetKey(KeyCode.W) || Party(1, 0.2f))
        {
            _rayBox.transform.position = new Vector3(
                0,
                Mathf.Sin(_time * 324) * .3f,
                Mathf.Sin(_time * 924) * .1f
            );
        }

        // Evolve
        if (Input.GetKey(KeyCode.E) || Party(0.3f, .4f))
        {
            _material.SetFloat("_Size1", 1 + (Lasp.AudioInput.CalculateRMS(Lasp.FilterType.Bypass) * 100) % 1);
            _material.SetFloat("_Size2", 1 + (Lasp.AudioInput.CalculateRMS(Lasp.FilterType.HighPass) * 100) % 1);
            _particleSystem.emissionRate = 100;
        }
        else
        {
            _material.SetFloat("_Size1", 1);
            _material.SetFloat("_Size2", 1);
            _particleSystem.emissionRate = 30;
        }

        transform.rotation = Quaternion.Euler(
            _time * 20,
            _time * 60,
            _time * 37
        );

        _radius = 9 + (Mathf.Sin(_time) + Mathf.Cos(_time * 1.7f)) * 1.4f;

        // Rotate Scene
        if (Input.GetKey(KeyCode.R) || Party(1.5f, 0.7f))
        {
            _radius += (Mathf.PerlinNoise(_time *2, 2) + Mathf.Cos(_time * .7f)) * 7.4f;
        }
        else
        {
            _radius = Mathf.Abs(_radius - 9) * .4f + 9;
        }

        _sceneCamera.transform.position *= _radius / _sceneCamera.transform.position.magnitude;
        _sceneCamera.transform.LookAt(Vector3.zero);

        // Transform
        if (Input.GetKey(KeyCode.T) || Party(0.0000003f, 0.4f))
        {
            _material.SetFloat("_Box", 1);
        }
        else
        {
            _material.SetFloat("_Box", 0);
        }

        // Yo
        if (Input.GetKey(KeyCode.Y) || Party(0.3f, 0.2f))
        {
            float e = Mathf.PerlinNoise(1, _time * 294) * 2.0f;
            _material.SetColor("_YoColor", new Color(0,e,e,1));
            _particleMaterial.SetColor("_TintColor", new Color(3,1,1,1));
        }
        else
        {
            float e = 0.005f;
            _material.SetColor("_YoColor", new Color(e,e,e,1));
            _particleMaterial.SetColor("_TintColor", new Color(.7f,.7f,1,1));
        }

        _material.SetFloat("_AccTime", _time);

        // Adjust the size of rayBox
        var length = (_rayBox.transform.position - _sceneCamera.transform.position).magnitude;
        _rayBox.transform.localScale = new Vector3(30, 30, (length * 2) * 0.8f);
        _rayBox.transform.LookAt(transform);

        // Constant motion
    }
}
