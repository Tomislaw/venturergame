using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorOscilator
{
    public List<Color> colors = new List<Color>();

    public Color GetColor(float factor)
    {
        if (colors.Count == 0) return new Color();

        var value = factor / (1f / colors.Count);
        int idStart = Mathf.FloorToInt(value);
        if (idStart == colors.Count)
            return colors[colors.Count - 1];

        int idEnd = idStart == colors.Count - 1 ? 0 : idStart + 1;
        var between = value - idStart;

        return Color.Lerp(colors[idStart], colors[idEnd], between);
    }
}

[System.Serializable]
public class IntensityOscilator
{
    public List<float> intensity = new List<float>();

    public float GetIntensity(float factor)
    {
        if (intensity.Count == 0) return 0;

        var value = factor / (1f / intensity.Count);
        int idStart = Mathf.FloorToInt(value);
        if (idStart == intensity.Count)
            return intensity[intensity.Count - 1];

        int idEnd = idStart == intensity.Count - 1 ? 0 : idStart + 1;
        var between = value - idStart;

        return Mathf.Lerp(intensity[idStart], intensity[idEnd], between);
    }
}

public class Background : MonoBehaviour
{
    public ColorOscilator colorsTop;
    public ColorOscilator colorsBottom;

    public IntensityOscilator intensityTop;
    public IntensityOscilator intensityBottom;

    private Color currentTop;
    private Color currentBottom;
    private float currentIntensityTop;
    private float currentIntensityBottom;

    [Range(0, 1)]
    public float value = 0;

    private float previousValue = -1;
    private new Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (previousValue != value)
        {
            UpdateColor();
            previousValue = value;
        }

        ResizeSpriteToScreen();
    }

    private void UpdateColor()
    {
        currentTop = colorsTop.GetColor(value);
        currentBottom = colorsBottom.GetColor(value);
        currentIntensityTop = intensityTop.GetIntensity(value);
        currentIntensityBottom = intensityBottom.GetIntensity(value);

        renderer.material.SetColor("_TopColor", currentTop * currentIntensityTop);
        renderer.material.SetColor("_BottomColor", currentBottom * currentIntensityBottom);
    }

    private void ResizeSpriteToScreen()
    {
        if (renderer == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        float width = 1;
        float height = 1;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f + 20;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width + 20;

        transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
    }
}