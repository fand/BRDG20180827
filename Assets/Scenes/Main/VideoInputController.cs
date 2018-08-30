using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VideoInputController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] Dropdown selector;
    [SerializeField] GameObject preview;

    int _width = 1920;
    int _height = 1080;
    int _fps = 60;
    Dictionary<string, WebCamTexture> _textures = new Dictionary<string, WebCamTexture>();

    void Start()
    {
        selector.ClearOptions();
        UpdateVideoInput();
    }

    public void UpdateVideoInput()
    {
        // Get selected option if exists
        var index = selector.value;
        var selectedOption = selector.options.Count() > 0 ? selector.options[index] : null;

        WebCamDevice[] devices = WebCamTexture.devices;
        var newOptions = devices.Select(x => x.name).ToList();

        selector.ClearOptions();
        selector.AddOptions(newOptions);

        // Restore selection
        if (selectedOption != null) {
            var newIndex = newOptions.IndexOf(selectedOption.text);
            selector.value = newIndex;
        }
        else if (newOptions.Count() > 0)
        {
            selector.value = 0;
            OnChangeVideoInput();
        }
    }

    public void OnChangeVideoInput()
    {
        var oldTexture = preview.GetComponent<Image>().material.mainTexture as WebCamTexture;
        if (oldTexture != null && oldTexture.isPlaying)
        {
            oldTexture.Stop();
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        var newDevice = devices[selector.value];

        WebCamTexture newTexture;
        if (!_textures.TryGetValue(newDevice.name, out newTexture)) {
            newTexture = new WebCamTexture(newDevice.name, _width, _height, _fps);
            _textures[newDevice.name] = newTexture;
        }

        gameController.SetVideoInputTexture(newTexture);
        preview.GetComponent<Image>().material.SetTexture("_VideoInput", newTexture);
        newTexture.Play();
    }
}
