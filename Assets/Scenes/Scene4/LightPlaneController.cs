using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPlaneController : MonoBehaviour
{
    [SerializeField] Transform _lightBarPrefab;

    int _num = 10;
    float _height = 2;
    List<Transform> _lights = new List<Transform>();

    void Start()
    {
        var h = _height / _num;
        for (int i = 0; i < _num; i++)
        {
            var position = transform.position + new Vector3(0, -_height/2 + h * (float)i, 0);
            var o = Instantiate(_lightBarPrefab,  position, Quaternion.identity, transform);
            // o.localScale = transform.localScale;
            o.rotation = transform.transform.rotation;
            _lights.Add(o);
        }
    }

    void Update()
    {
        var i = 0;
        _lights.ForEach(l => {
            l.position = l.position + l.right * Mathf.Sin(Time.time + i * 20) * .2f;
            i++;
        });
    }
}
