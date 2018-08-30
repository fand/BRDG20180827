using UnityEngine;
using System.Collections;

public class FPSMeter : MonoBehaviour
{
    string _label = "";
    float _count;

    IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                _count = 1 / Time.deltaTime;
                _label = "FPS :" + Mathf.Round(_count);
            }
            else
            {
                _label = "Pause";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect (5, 40, 100, 25), _label);
    }
}
