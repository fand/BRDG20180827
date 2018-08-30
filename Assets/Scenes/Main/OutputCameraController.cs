using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputCameraController : MonoBehaviour
{
	[SerializeField] RenderTexture _input;

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(_input, dst);
	}
}
