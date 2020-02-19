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

[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
    public ColorOscilator colorsTop;
    public ColorOscilator colorsBottom;

    private Color currentTop;
    private Color currentBottom;

    private SpriteRenderer sr;
    private Texture2D texture;

    private float timer = 0;

    private void Start()
    {
        texture = new Texture2D(1, 2);
        texture.wrapMode = TextureWrapMode.Clamp;
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 2), new Vector2(0.5f, 0.5f));
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        UpdateColor();
        ResizeSpriteToScreen();
    }

    private void UpdateColor()
    {
        timer += Time.deltaTime;

        if (timer > 100)
            timer = 0;

        currentTop = colorsTop.GetColor(timer / 100);
        currentBottom = colorsBottom.GetColor(timer / 100);

        texture.SetPixel(0, 1, currentTop);
        texture.SetPixel(0, 0, currentBottom);
        texture.Apply();
    }

    private void ResizeSpriteToScreen()
    {
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f + 20;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width + 20;

        transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y);
    }
}