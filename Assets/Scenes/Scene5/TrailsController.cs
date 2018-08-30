using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailsController : MonoBehaviour
{
    ParticleSystem _particleSystem;
    float _speed = 1.0f;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            _speed = 12 - (12 - _speed) * 0.2f;
            // _speed = 20;
        }
        else {
            _speed = (_speed - 0.8f) * 0.7f + 0.8f;
        }

        _particleSystem.playbackSpeed = _speed;
        // _particleSystem.main.simulationSpeed = _speed;
    }
}
