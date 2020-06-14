using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private int counter = 1;

    private float timer = 0;
    private int adder = 1;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.1)
        {
            counter += adder;
        }
        if (counter % 200 == 0)
            adder = -1;
        if (Screen.resolutions.Length > 0)
        {
            var res = Screen.resolutions[0];
            Screen.SetResolution(res.width, res.height, true);
        }
        else if (Display.displays.Length > 0)
        {
            var res = Display.displays[0];
            Screen.SetResolution(res.renderingWidth, res.renderingHeight, true);
        }
        else
            Screen.SetResolution(Screen.width, Screen.height, false);
    }
}