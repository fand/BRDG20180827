using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1CameraController : MonoBehaviour {
    float _radius;
    [SerializeField] GameObject _rayBox;


    void Start()
    {
        _radius = transform.position.magnitude;
    }

    void Update()
    {

    }
}
