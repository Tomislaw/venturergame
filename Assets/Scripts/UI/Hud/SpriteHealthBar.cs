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
    private GameObject maskObj2;
    private GameObject foregroundObj;
    private GameObject betweenObj;
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

    private float previousHealthRange = 0;

    public int pixelsPerUnit = 32;

    private IEnumerator animationCoroutine;

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
            var sg = foregroundObj.AddComponent<SortingGroup>();
            sg.sortingOrder = 2;
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
            maskObj.transform.parent = foregroundObj.transform;
            maskObj.name = this.name + "_mask";
            maskObj.transform.localPosition = new Vector2(-spriteRenderer.bounds.size.x / 2f, -spriteRenderer.bounds.size.y / 2f);
        }
        if (!betweenObj)
        {
            betweenObj = new GameObject();
            betweenObj.transform.parent = transform;
            betweenObj.name = this.name + "_between";
            var bsr = betweenObj.AddComponent<SpriteRenderer>();
            bsr.sprite = background;
            bsr.material = new Material(material);
            bsr.material.shader = Shader.Find("GUI/Text Shader");
            bsr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            bsr.color = new Color(1, 1, 1, 0.6f);
            betweenObj.transform.localPosition = new Vector3(0, 0, -0.2f);
            var sg = betweenObj.AddComponent<SortingGroup>();
            sg.sortingOrder = 1;
        }
        if (!maskObj2)
        {
            maskObj2 = new GameObject();
            maskObj2.AddComponent<SpriteHealthBarMask>().pixelsPerUnit = pixelsPerUnit;
            maskObj2.transform.parent = betweenObj.transform;
            maskObj2.name = this.name + "_mask";
            maskObj2.transform.localPosition = new Vector2(-spriteRenderer.bounds.size.x / 2f, -spriteRenderer.bounds.size.y / 2f);
            maskObj2.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y * healthRange / 2) * pixelsPerUnit;
        }
        spriteRenderer = foregroundObj.GetComponent<SpriteRenderer>();

        previousHealthRange = healthRange;
        maskObj.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y * healthRange) * pixelsPerUnit;
        maskObj2.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y * healthRange) * pixelsPerUnit;
    }

    private void OnValidate()
    {
        HealthbarType = _type;
    }

    // Update is called once per frame
    private void Update()
    {
        if (previousHealthRange != healthRange)
        {
            maskObj.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y * healthRange) * pixelsPerUnit;
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            StartCoroutine(AnimateValueChange(previousHealthRange, healthRange, 0.3f));
            previousHealthRange = healthRange;
        }
    }

    private IEnumerator AnimateValueChange(float start, float end, float time)
    {
        float timeLeft = time;
        while (timeLeft > 0)
        {
            var value = LeanTween.easeInOutCubic(spriteRenderer.bounds.size.y * start, spriteRenderer.bounds.size.y * end, 1 - timeLeft / time);
            maskObj2.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, value) * pixelsPerUnit;
            timeLeft -= Time.deltaTime;
            yield return 0;
        }
        maskObj2.transform.localScale = new Vector2(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y * end) * pixelsPerUnit;
        yield return 0;
    }
}