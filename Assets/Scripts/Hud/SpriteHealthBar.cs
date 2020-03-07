using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteMask))]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteHealthBarMask : MonoBehaviour
{
    private Texture2D texture;
    private SpriteMask mask;

    public int pixelsPerUnit = 32;

    private void Start()
    {
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(1, 1, 1, 1));
        texture.Apply();

        mask = GetComponent<SpriteMask>();
        mask.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(), pixelsPerUnit);
    }
}

[RequireComponent(typeof(SortingGroup))]
public class SpriteHealthBar : MonoBehaviour
{
    public enum Type
    {
        ENEMY, ENEMY2, FRIENDLY, ALLY, NEUTRAL
    }

    private GameObject maskObj;
    private GameObject foregroundObj;
    private GameObject backgroundObj;

    public Sprite enemy;
    public Sprite enemy2;
    public Sprite neutral;
    public Sprite friendly;
    public Sprite ally;
    public Sprite background;

    public Material material;

    [SerializeField]
    private Type _type;

    public Type HealthbarType
    {
        get => _type;
        set
        {
            _type = value;
            if (!spriteRenderer)
                return;

            if (_type == Type.ALLY)
                spriteRenderer.sprite = ally;
            else if (_type == Type.NEUTRAL)
                spriteRenderer.sprite = neutral;
            else if (_type == Type.FRIENDLY)
                spriteRenderer.sprite = friendly;
            else if (_type == Type.ENEMY)
                spriteRenderer.sprite = enemy;
            else if (_type == Type.ENEMY2)
                spriteRenderer.sprite = enemy2;
        }
    }

    private SpriteRenderer spriteRenderer;

    [Range(0, 1)]
    public float healthRange = 1;

    public int pixelsPerUnit = 32;

    private void Start()
    {
        if (!foregroundObj)
        {
            foregroundObj = new GameObject();
            foregroundObj.transform.parent = transform;
            foregroundObj.name = this.name + "_foreground";
            spriteRenderer = foregroundObj.AddComponent<SpriteRenderer>();
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            spriteRenderer.material = material;
            spriteRenderer.sortingOrder = 1;
            HealthbarType = _type;
            foregroundObj.transform.localPosition = new Vector3();
        }
        if (!backgroundObj)
        {
            backgroundObj = new GameObject();
            backgroundObj.transform.parent = transform;
            backgroundObj.name = this.name + "_background";
            var bsr = backgroundObj.AddComponent<SpriteRenderer>();
            bsr.sprite = background;
            bsr.material = material;
            backgroundObj.transform.localPosition = new Vector3(0, 0, -0.1f);
        }
        if (!maskObj)
        {
            maskObj = new GameObject();
            maskObj.AddComponent<SpriteHealthBarMask>().pixelsPerUnit = pixelsPerUnit;
            maskObj.transform.parent = transform;
            maskObj.name = this.name + "_mask";
        }
        spriteRenderer = foregroundObj.GetComponent<SpriteRenderer>();
        maskObj.transform.localPosition = new Vector2(-spriteRenderer.bounds.size.x / 2f, -spriteRenderer.bounds.size.y / 2f);
    }

    private void OnValidate()
    {
        HealthbarType = _type;
    }

    // Update is called once per frame
    private void Update()
    {
        maskObj.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y * healthRange) * pixelsPerUnit;
    }
}