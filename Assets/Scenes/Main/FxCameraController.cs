using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxCameraController : MonoBehaviour
{
    [SerializeField] RenderTexture _input;
    [SerializeField] Shader _shader;
    Material _material;

    List<float> _values = new List<float>(new float[16]);  // TODO: make length dynamic
    List<float> _hits = new List<float>(new float[16]);
    float _volumeRatio = 1.0f;
    float[] _knobs = new float[8];

    void Start()
    {
        _material = new Material(_shader);
        MidiJack.MidiMaster.knobDelegate += OnControlChange;
    }

    void OnControlChange(MidiJack.MidiChannel channel, int knobNumber, float knobValue)
    {
        if (16 <= knobNumber && knobNumber <= 22)
        {
            _knobs[knobNumber - 16] = knobValue;
            _material.SetFloatArray("_Knob", _knobs);
        }

        // Use last knob for volume ratio
        if (knobNumber == 23)
        {
            _volumeRatio = knobValue * 3;
        }
    }

    void Update()
    {
        var volume = Lasp.AudioInput.CalculateRMS(Lasp.FilterType.Bypass);
        _material.SetFloat("_Volume", volume * _volumeRatio);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        var texture = _input != null ? _input : src;
        Graphics.Blit(texture, dst, _material);
    }

    public void SetFxLevel(int fxNumber, float value)
    {
        if (fxNumber >= _values.Count)
        {
            Debug.LogError("Invalid index: " + fxNumber);
            return;
        }

        _values[fxNumber] = value;
        _hits[fxNumber] = Time.time;

        _material.SetFloatArray("_Fx", _values);
        _material.SetFloatArray("_Hit", _hits);
    }
}
