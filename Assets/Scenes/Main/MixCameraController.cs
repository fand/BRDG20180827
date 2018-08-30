using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixCameraController : MonoBehaviour
{
    [SerializeField] Shader _shader;
    Material _material;

    [SerializeField] RenderTexture _scene1;
    [SerializeField] RenderTexture _scene2;
    [SerializeField] RenderTexture _scene3;
    [SerializeField] RenderTexture _scene4;
    [SerializeField] RenderTexture _scene5;
    [SerializeField] RenderTexture _scene6;

    void Awake()
    {
        _material = new Material(_shader);
    }

    void Start()
    {
        _material.SetTexture("_Chan_1", _scene1);
        _material.SetTexture("_Chan_2", _scene2);
        _material.SetTexture("_Chan_3", _scene3);
        _material.SetTexture("_Chan_4", _scene4);
        _material.SetTexture("_Chan_5", _scene5);
        _material.SetTexture("_Chan_6", _scene6);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, _material);
    }

    public void SetVideoInputTexture(Texture texture)
    {
        _material.SetTexture("_Chan_0", texture);
    }

    public void SetChannelGain(int chan, float value)
    {
        _material.SetFloat("_Gain_" + chan.ToString(), value);
    }

    public void SetMasterGain(float value)
    {
        _material.SetFloat("_MasterGain", value);
    }
}
