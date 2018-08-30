using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewSettingsController : MonoBehaviour
{

    [SerializeField] GameController gameController;
    [SerializeField] Slider opacity;
    [SerializeField] Slider mix;

    void Start ()
    {
        gameController.SetPreviewOpacity(opacity.value);
        gameController.SetPreviewMix(mix.value);
    }

    public void UpdatePreviewOpacity()
    {
        gameController.SetPreviewOpacity(opacity.value);
    }

    public void UpdatePreviewMix()
    {
        gameController.SetPreviewMix(mix.value);
    }
}
