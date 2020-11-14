using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SwitchableTexture : MonoBehaviour
{
    private Material material;

    private void Awake()
    {
        var r = GetComponent<Renderer>();
        material = r.material = new Material(r.sharedMaterial);
    }

    public Texture2D texture
    {
        set
        {
            material.SetTexture("_SecondTex", value);
        }
    }

    public Texture2D mask
    {
        set
        {
            material.SetTexture("_SecondMaskTex", value);
        }
    }

    public Texture2D normalmap
    {
        set
        {
            material.SetTexture("_SecondNormalMap", value);
        }
    }

    public bool UseDefaultSprite
    {
        set
        {
            material.SetInt("_ForceSpriteTexture", value ? 1 : 0);
        }
    }
}