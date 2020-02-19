using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class LightColorOscilator : MonoBehaviour
{
    public ColorOscilator colorOscilator;
    public Light2D light;

    private float timer = 0;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 100)
            timer = 0;

        light.color = colorOscilator.GetColor(timer / 100f);
        light.intensity = light.color.a;
    }
}