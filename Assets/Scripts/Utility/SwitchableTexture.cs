using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwitchableTexture : MonoBehaviour
{
    public Material Material = null;
    private Material material = null;
    private Renderer r;

    private void Awake()
    {
        r = GetComponent<Renderer>();
        if (Material != null && material != r.material)
        {
            material = new Material(Material);
            texture = _texture;
            mask = _mask;
            normalmap = _normalmap;
            UseDefaultSprite = _UseDefaultSprite;
            r.material = material;
        }
    }

    public void OnValidate()
    {
        r = GetComponent<Renderer>();
        if (r.sharedMaterial != Material)
            r.sharedMaterial = Material;
    }

    [SerializeField]
    private Texture2D _texture = null;

    public Texture2D texture
    {
        set
        {
            if (Application.isPlaying)
                material.SetTexture("_SecondTex", value);
            _texture = value;
        }
        get => _texture;
    }

    [SerializeField]
    private Texture2D _mask = null;

    public Texture2D mask
    {
        set
        {
            if (Application.isPlaying)
                material.SetTexture("_SecondMaskTex", value);
            _mask = value;
        }
        get => _mask;
    }

    [SerializeField]
    private Texture2D _normalmap = null;

    public Texture2D normalmap
    {
        set
        {
            if (Application.isPlaying)
                material.SetTexture("_SecondNormalMap", value);
            _normalmap = value;
        }
        get => _normalmap;
    }

    [SerializeField]
    private bool _UseDefaultSprite;

    public bool UseDefaultSprite
    {
        set
        {
            if (Application.isPlaying)
                material.SetInt("_ForceSpriteTexture", value ? 1 : 0);
            _UseDefaultSprite = value;
        }
        get => _UseDefaultSprite;
    }
}