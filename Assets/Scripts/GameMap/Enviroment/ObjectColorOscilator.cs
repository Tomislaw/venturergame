using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class ObjectColorOscilator : MonoBehaviour
{
    public ColorOscilator colorOscilator;
    public UnityEngine.Experimental.Rendering.Universal.Light2D light;
    public SpriteRenderer spriteRenderer;

    [Range(0, 1)]
    public float value = 0;

    public float lightIntensityFactor = 1.2f;
    public bool spriteNeverTransparent = true;

    private float previousValue = -1;

    // Update is called once per frame
    private void Update()
    {
        if (previousValue != value)
        {
            if (light)
            {
                light.color = colorOscilator.GetColor(value);
                light.intensity = light.color.a * lightIntensityFactor;
            }
            if (spriteRenderer)
            {
                var color = colorOscilator.GetColor(value);
                if (spriteNeverTransparent)
                    color.a = 1;
                spriteRenderer.color = color;
            }

            previousValue = value;
        }
    }
}