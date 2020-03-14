using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    public SpriteRenderer targetSpriteRenderer;
    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateOutline(true);
    }

    private void OnDisable()
    {
        UpdateOutline(false);
    }

    private void Update()
    {
        UpdateOutline(true);
    }

    private void UpdateOutline(bool outline)
    {
        if (targetSpriteRenderer.sprite != spriteRenderer.sprite)
            spriteRenderer.sprite = targetSpriteRenderer.sprite;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}