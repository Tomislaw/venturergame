using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Chunk : MonoBehaviour
{
    private List<ChunkItem> chunks = new List<ChunkItem>();

    public float additionalOffset;

    public float Width
    {
        get
        {
            float width = additionalOffset;
            foreach (var ch in chunks)
            {
                width += ch.Width;
                width += additionalOffset;
            }

            return width;
        }
    }

    private void Awake()
    {
        Invalidate();
    }

    public void Invalidate()
    {
        chunks.Clear();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var go = transform.GetChild(i).gameObject;
            Debug.Log(name + " - " + go.name);
            var item = go.GetComponent<ChunkItem>();
            if (item == null)
                item = go.AddComponent<ChunkItem>();
            chunks.Add(item);
        }

        float start = additionalOffset;
        foreach (var chunk in chunks)
        {
            if (chunk == null)
                continue;
            chunk.transform.localPosition = new Vector2(start, 0);
            start += chunk.Width;
            start += additionalOffset;
            Debug.Log(name + " - " + chunk.name);
        };
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (Application.isEditor)
            Invalidate();
    }

    private void OnValidate()
    {
        if (Application.isEditor)
            Invalidate();
    }

    private void OnTransformChildrenChanged()
    {
        if (Application.isEditor)
            Invalidate();
    }

#endif
}