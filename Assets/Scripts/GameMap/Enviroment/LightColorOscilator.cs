using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class LightColorOscilator : MonoBehaviour
{
    public ColorOscilator colorOscilator;
    public UnityEngine.Experimental.Rendering.Universal.Light2D light;

    [Range(0, 1)]
    public float value = 0;

    private float previousValue = -1;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (previousValue != value)
        {
            light.color = colorOscilator.GetColor(value);
            light.intensity = light.color.a;
            previousValue = value * 1.2f;
        }
    }
}