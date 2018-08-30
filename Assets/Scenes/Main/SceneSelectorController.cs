using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSelectorController : MonoBehaviour
{
    [SerializeField] GameController _gameController;
    [SerializeField] Camera _frontMixCamera;
    [SerializeField] Camera _rearMixCamera;
    MixCameraController _front;
    MixCameraController _rear;

    bool[] _isLoaded = { false, false, false, false, false, false, false };
    int[] _sceneState = { 0, 0, 0, 0, 0, 0, 0 }; // 0: disabled, 1: front, 2: rear, 3: both
    bool[] _isFrontToggled = { false, false, false, false, false, false, false };
    bool[] _isRearToggled = { false, false, false, false, false, false, false };
    float[] _value = { 1, 1, 1, 1, 1, 1, 1 };

    [SerializeField] Camera _frontFxCamera;
    [SerializeField] Camera _rearFxCamera;
    FxCameraController _frontFx;
    FxCameraController _rearFx;
    List<bool> _isFrontFxToggled = new List<bool>(new bool[16]);
    List<bool> _isRearFxToggled = new List<bool>(new bool[16]);
    [SerializeField] SubSceneController[] _subScenes;

    // Called only when Update button is clicked
    public void LoadScenes()
    {
        for (int i = 1; i < 7; i++)
        {
            if (!_isLoaded[i])
            {
                SceneManager.LoadScene("Scene" + i.ToString(), LoadSceneMode.Additive);
                _isLoaded[i] = true;
            }
        }
    }

    public void RegisterScene(int channel, SubSceneController scene)
    {
        _subScenes[channel] = scene;
    }

    void Start()
    {
        _front = _frontMixCamera.GetComponent<MixCameraController>();
        _rear = _rearMixCamera.GetComponent<MixCameraController>();
        _frontFx = _frontFxCamera.GetComponent<FxCameraController>();
        _rearFx = _rearFxCamera.GetComponent<FxCameraController>();

        MidiJack.MidiMaster.knobDelegate += OnControlChange;
    }

    void OnControlChange(MidiJack.MidiChannel channel, int knobNumber, float knobValue)
    {
        // MasterGain
        if (knobNumber == 7)
        {
            _front.SetMasterGain(knobValue);
            _rear.SetMasterGain(knobValue);
            return;
        }

        int i = knobNumber + 1;
        if (knobNumber == 6) { i = 0; } // Use last fader for video input
        if (_value.Length <= i) { return; }

        _value[i] = knobValue;

        if ((_sceneState[i] & 1) == 1)
        {
            _front.SetChannelGain(i, knobValue);
        }
        if ((_sceneState[i] & 2) == 2)
        {
            _rear.SetChannelGain(i, knobValue);
        }
    }

    void Update()
    {
        var isFront = Input.GetKey(KeyCode.LeftShift);
        var isRear = Input.GetKey(KeyCode.RightShift);
        var isToggle = Input.GetKey(KeyCode.Space);

        if (isToggle)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) { ToggleScene(0, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { ToggleScene(1, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { ToggleScene(2, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { ToggleScene(3, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { ToggleScene(4, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { ToggleScene(5, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha6)) { ToggleScene(6, isFront, isRear); }

            if (Input.GetKeyDown(KeyCode.A)) { ToggleFx(0, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.S)) { ToggleFx(1, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.D)) { ToggleFx(2, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.F)) { ToggleFx(3, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.G)) { ToggleFx(4, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.H)) { ToggleFx(5, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.J)) { ToggleFx(6, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.K)) { ToggleFx(7, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.L)) { ToggleFx(8, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Z)) { ToggleFx(9, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.X)) { ToggleFx(10, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.C)) { ToggleFx(11, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.V)) { ToggleFx(12, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.B)) { ToggleFx(13, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.N)) { ToggleFx(14, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.M)) { ToggleFx(15, isFront, isRear); }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) { EnableScene(0, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { EnableScene(1, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { EnableScene(2, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { EnableScene(3, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { EnableScene(4, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { EnableScene(5, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Alpha6)) { EnableScene(6, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha0)) { DisableScene(0, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha1)) { DisableScene(1, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha2)) { DisableScene(2, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha3)) { DisableScene(3, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha4)) { DisableScene(4, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha5)) { DisableScene(5, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Alpha6)) { DisableScene(6, isFront, isRear); }

            if (Input.GetKeyDown(KeyCode.A)) { EnableFx(0, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.S)) { EnableFx(1, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.D)) { EnableFx(2, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.F)) { EnableFx(3, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.G)) { EnableFx(4, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.H)) { EnableFx(5, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.J)) { EnableFx(6, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.K)) { EnableFx(7, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.L)) { EnableFx(8, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.Z)) { EnableFx(9, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.X)) { EnableFx(10, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.C)) { EnableFx(11, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.V)) { EnableFx(12, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.B)) { EnableFx(13, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.N)) { EnableFx(14, isFront, isRear); }
            if (Input.GetKeyDown(KeyCode.M)) { EnableFx(15, isFront, isRear); }

            if (Input.GetKeyUp(KeyCode.A)) { DisableFx(0, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.S)) { DisableFx(1, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.D)) { DisableFx(2, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.F)) { DisableFx(3, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.G)) { DisableFx(4, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.H)) { DisableFx(5, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.J)) { DisableFx(6, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.K)) { DisableFx(7, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.L)) { DisableFx(8, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.Z)) { DisableFx(9, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.X)) { DisableFx(10, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.C)) { DisableFx(11, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.V)) { DisableFx(12, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.B)) { DisableFx(13, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.N)) { DisableFx(14, isFront, isRear); }
            if (Input.GetKeyUp(KeyCode.M)) { DisableFx(15, isFront, isRear); }
        }
    }

    void ToggleScene(int chan, bool isFront, bool isRear)
    {
        if (isFront)
        {
            if (!_isFrontToggled[chan])
            {
                _front.SetChannelGain(chan, _value[chan]);
                _isFrontToggled[chan] = true;
                _sceneState[chan] = _sceneState[chan] | 1;
            }
            else
            {
                _front.SetChannelGain(chan, 0);
                _isFrontToggled[chan] = false;
                _sceneState[chan] = _sceneState[chan] & 2;
            }
        }
        if (isRear)
        {
            if (!_isRearToggled[chan])
            {
                _rear.SetChannelGain(chan, _value[chan]);
                _isRearToggled[chan] = true;
                _sceneState[chan] = _sceneState[chan] | 2;
            }
            else
            {
                _rear.SetChannelGain(chan, 0);
                _isRearToggled[chan] = false;
                _sceneState[chan] = _sceneState[chan] & 1;
            }
        }
        if (!isFront && !isRear)
        {
            if (_isFrontToggled[chan] && _isRearToggled[chan])
            {
                _front.SetChannelGain(chan, 0);
                _rear.SetChannelGain(chan, 0);
                _isFrontToggled[chan] = false;
                _isRearToggled[chan] = false;
                _sceneState[chan] = 0;
            }
            else
            {
                _front.SetChannelGain(chan, _value[chan]);
                _rear.SetChannelGain(chan, _value[chan]);
                _isFrontToggled[chan] = true;
                _isRearToggled[chan] = true;
                _sceneState[chan] = 3;
            }
        }

        if (_subScenes[chan] != null) {
            _subScenes[chan].SetActive(_sceneState[chan] != 0);
        }
    }

    void EnableScene(int chan, bool isFront, bool isRear)
    {
        if (isFront || (!isFront && !isRear))
        {
            _front.SetChannelGain(chan, _value[chan]);
            _sceneState[chan] = _sceneState[chan] | 1;
        }
        if (isRear || (!isFront && !isRear))
        {
            _rear.SetChannelGain(chan, _value[chan]);
            _sceneState[chan] = _sceneState[chan] | 2;
        }

        if (_subScenes[chan] != null) {
            _subScenes[chan].SetActive(true);
        }
    }

    void DisableScene(int chan, bool isFront, bool isRear)
    {
        if (isFront && !_isFrontToggled[chan])
        {
            _front.SetChannelGain(chan, 0);
            _sceneState[chan] = _sceneState[chan] & 2;
        }
        if (isRear && !_isRearToggled[chan])
        {
            _rear.SetChannelGain(chan, 0);
            _sceneState[chan] = _sceneState[chan] & 1;
        }
        if (!isFront && !isRear)
        {
            if (!_isFrontToggled[chan]) {
                _front.SetChannelGain(chan, 0);
                _sceneState[chan] = _sceneState[chan] & 2;
            }
            if (!_isRearToggled[chan]) {
                _rear.SetChannelGain(chan, 0);
                _sceneState[chan] = _sceneState[chan] & 1;
            }
        }

        if (_subScenes[chan] != null) {
            _subScenes[chan].SetActive(_sceneState[chan] != 0);
        }
    }

    void ToggleFx(int chan, bool isFront, bool isRear)
    {
        if (isFront)
        {
            if (!_isFrontFxToggled[chan])
            {
                _frontFx.SetFxLevel(chan, 1);
                _isFrontFxToggled[chan] = true;
            }
            else
            {
                _frontFx.SetFxLevel(chan, 0);
                _isFrontFxToggled[chan] = false;
            }
        }
        if (isRear)
        {
            if (!_isRearFxToggled[chan])
            {
                _rearFx.SetFxLevel(chan, 1);
                _isRearFxToggled[chan] = true;
            }
            else
            {
                _rearFx.SetFxLevel(chan, 0);
                _isRearFxToggled[chan] = false;
            }
        }
        if (!isFront && !isRear)
        {
            if (_isFrontFxToggled[chan] && _isRearFxToggled[chan])
            {
                _frontFx.SetFxLevel(chan, 0);
                _rearFx.SetFxLevel(chan, 0);
                _isFrontFxToggled[chan] = false;
                _isRearFxToggled[chan] = false;
            }
            else
            {
                _frontFx.SetFxLevel(chan, 1);
                _rearFx.SetFxLevel(chan, 1);
                _isFrontFxToggled[chan] = true;
                _isRearFxToggled[chan] = true;
            }
        }
    }

    void EnableFx(int chan, bool isFront, bool isRear)
    {
        if (isFront || (!isFront && !isRear))
        {
            _frontFx.SetFxLevel(chan, 1);
        }
        if (isRear || (!isFront && !isRear))
        {
            _rearFx.SetFxLevel(chan, 1);
        }
    }

    void DisableFx(int chan, bool isFront, bool isRear)
    {
        if (isFront && !_isFrontFxToggled[chan])
        {
            _frontFx.SetFxLevel(chan, 0);
        }
        if (isRear && !_isRearFxToggled[chan])
        {
            _rearFx.SetFxLevel(chan, 0);
        }
        if (!isFront && !isRear)
        {
            if (!_isFrontFxToggled[chan]) {
                _frontFx.SetFxLevel(chan, 0);
            }
            if (!_isRearFxToggled[chan]) {
                _rearFx.SetFxLevel(chan, 0);
            }
        }
    }
}
