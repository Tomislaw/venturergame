using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplashing : MonoBehaviour
{
    public SpriteRenderer splashingPrefab;
    public ContactFilter2D filter;
    public float splashFadeDistance = 0.4f;
    private BoxCollider2D waterCollider;

    private Dictionary<GameObject, SpriteRenderer> splashes = new Dictionary<GameObject, SpriteRenderer>();

    private void Start()
    {
        waterCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        foreach (var splash in splashes)
        {
            var posX = splash.Key.transform.position.x;
            splash.Value.transform.position = new Vector2(posX, transform.position.y);
            SetSpriteAlphaColor(splash.Value);
        }
    }

    private void SetSpriteAlphaColor(SpriteRenderer obj)
    {
        if (waterCollider == null)
            return;

        var width = waterCollider.bounds.extents.x;
        var distance = Mathf.Abs(transform.position.x - obj.transform.position.x);

        var color = obj.color;
        Mathf.Lerp(width, splashFadeDistance, distance);
        if (distance > width || width == 0)
            obj.color = new Color(color.r, color.g, color.b, 0);
        else if (width - splashFadeDistance > distance)
            obj.color = new Color(color.r, color.g, color.b, 1);
        else
            obj.color = new Color(color.r, color.g, color.b, (width - distance) / splashFadeDistance);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var go = other.gameObject;
        if (go == null)
            return;

        SpriteRenderer splash = null;
        splashes.TryGetValue(go, out splash);

        if (splash)
        {
            splashes.Remove(go);
            Destroy(splash.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var go = other.gameObject;
        if (go == null)
            return;

        var splash = Instantiate(splashingPrefab);
        splash.transform.parent = transform;
        splashes.Add(go, splash);
    }
}