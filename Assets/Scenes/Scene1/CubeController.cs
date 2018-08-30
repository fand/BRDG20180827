using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {
	void Update () {
		transform.rotation = Quaternion.Euler(Time.time * 10, Time.time * 20, 0);
	}
}
