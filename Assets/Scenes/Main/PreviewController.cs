using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    [SerializeField] GameObject front;
    [SerializeField] GameObject rear;
    [SerializeField] RenderTexture frontTexture;
    [SerializeField] RenderTexture rearTexture;

    private float rx = 0.0f;
    private float ry = 0.0f;

    public void SetOpacity(float opacity)
    {
        front.GetComponent<Renderer>().material.SetFloat("_Opacity", opacity);
        rear.GetComponent<Renderer>().material.SetFloat("_Opacity", opacity);
    }

    public void SetMix(float mix)
    {
        front.GetComponent<Renderer>().material.SetFloat("_Mix", Mathf.Min(2 - mix * 2, 1));
        rear.GetComponent<Renderer>().material.SetFloat("_Mix", Mathf.Min(mix * 2, 1));
    }

    void Start()
    {
        front.GetComponent<Renderer>().material.mainTexture = frontTexture;
        rear.GetComponent<Renderer>().material.mainTexture = rearTexture;
    }

    void Update()
    {
        var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (shift) {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                ry += 0.03f;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                ry -= 0.03f;
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                rx -= 0.03f;
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                rx += 0.03f;
            }

            rx = Mathf.Clamp(rx, -1, 1);
            ry = Mathf.Clamp(ry, -1, 1);
            front.transform.position = transform.position + new Vector3(-ry * 15, -rx * 2, 0);
            rear.transform.position = transform.position + new Vector3(-ry * 5, rx * 4 - 0.1f, 2);
            front.transform.rotation = Quaternion.Euler(0, ry * 60, 0);
            rear.transform.rotation = Quaternion.Euler(0, ry * 60, 0);
        }
    }
}
