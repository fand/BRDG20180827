using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject preview;
    [SerializeField] Camera frontMixCamera;
    [SerializeField] Camera rearMixCamera;

    void Start()
    {
        int maxDisplayCount = 3;
        for (int i = 0; i < maxDisplayCount && i < Display.displays.Length; i++) {
            Display.displays[i].Activate();
        }
    }

    void OnNoteOn(MidiJack.MidiChannel channel, int noteNumber, float velocity)
    {
        Debug.Log(">> Note On: " + channel.ToString() + ": " + noteNumber);
    }

    void OnNoteOff(MidiJack.MidiChannel channel, int noteNumber, float velocity)
    {
        Debug.Log(">> Note Off: " + channel.ToString() + ": " + noteNumber);
    }

    void OnControlChange(MidiJack.MidiChannel channel, int knobNumber, float knobValue)
    {
        Debug.Log(">> CC: " + channel.ToString() + ": " + knobNumber + ": " + knobValue);
    }

    public void SetVideoInputTexture(Texture texture)
    {
        frontMixCamera.GetComponent<MixCameraController>().SetVideoInputTexture(texture);
        rearMixCamera.GetComponent<MixCameraController>().SetVideoInputTexture(texture);
    }

    public void SetPreviewOpacity(float opacity)
    {
        preview.GetComponent<PreviewController>().SetOpacity(opacity);
    }

    public void SetPreviewMix(float mix)
    {
        preview.GetComponent<PreviewController>().SetMix(mix);
    }
}
