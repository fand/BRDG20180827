using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    float _length = 0.0f;
    float _speed;
    float _pipeWidth = 1;

    void Start()
    {
        _speed = Random.Range(0.05f, 0.95f);
    }

    void Update()
    {
        var l = transform.localScale.y;

        if (l < _length)
        {
            var nextl = _length - (_length - l) * _speed;
            transform.localScale = new Vector3(_pipeWidth, nextl, 1);
        }
    }

    public void SetLength(float length)
    {
        _length = length;
    }

    public void SetWidth(float width)
    {
        _pipeWidth = width;
    }
}
