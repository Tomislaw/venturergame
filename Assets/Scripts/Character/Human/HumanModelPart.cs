using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class HumanModelPart : MonoBehaviour
{
    private Sprite previousSpriteToReplace;

    public int spriteOrderOffset = 0;
    private int previousSpriteOrderOffset = 0;

    private Dictionary<SpriteRenderer, int> innerSpriteRenderers = new Dictionary<SpriteRenderer, int>();
    private SpriteRenderer spriteRenderer;

    public void Add(SpriteRenderer renderer)
    {
        if (renderer == null || innerSpriteRenderers.ContainsKey(renderer))
            return;

        innerSpriteRenderers.Add(renderer, renderer.sortingOrder);
        renderer.sprite = spriteRenderer.sprite;
        renderer.sortingOrder += spriteOrderOffset;
        renderer.transform.SetParent(transform, false);
    }

    public void Add(GameObject obj)
    {
        if (obj == null)
            return;

        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            innerSpriteRenderers.Add(renderer, renderer.sortingOrder);
            renderer.sprite = spriteRenderer.sprite;
            renderer.sortingOrder += spriteOrderOffset;
        }
        obj.transform.SetParent(transform, false);
    }

    public void RemoveAndDestroy(SpriteRenderer renderer)
    {
        if (!innerSpriteRenderers.ContainsKey(renderer))
            return;

        innerSpriteRenderers.Remove(renderer);
        Destroy(renderer.gameObject);
    }

    public void RemoveAndDestroy(GameObject obj)
    {
        if (!obj)
            return;

        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!innerSpriteRenderers.ContainsKey(renderer))
                return;

            innerSpriteRenderers.Remove(renderer);
        }
        Destroy(obj);
    }

    public void RemoveAndDestroy(string name)
    {
        var item = transform.Find(name);
        if (item != null)
            RemoveAndDestroy(item.gameObject);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        foreach (var existing in GetComponentsInChildren<SpriteRenderer>())
            if (!innerSpriteRenderers.ContainsKey(existing))
                innerSpriteRenderers.Add(existing, existing.sortingOrder);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (previousSpriteOrderOffset != spriteOrderOffset)
        {
            foreach (var renderer in innerSpriteRenderers)
                renderer.Key.sortingOrder = renderer.Value + spriteOrderOffset;

            previousSpriteOrderOffset = spriteOrderOffset;
        }

        if (spriteRenderer.sprite != previousSpriteToReplace)
        {
            previousSpriteToReplace = spriteRenderer.sprite;
            foreach (var renderer in innerSpriteRenderers)
                renderer.Key.sprite = spriteRenderer.sprite;
        }
    }
}