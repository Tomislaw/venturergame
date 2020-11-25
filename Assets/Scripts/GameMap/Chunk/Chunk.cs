using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public List<ChunkItem> chunks = new List<ChunkItem>();

    public float Width
    {
        get
        {
            float width = 0;
            foreach (var ch in chunks)
                width += ch.Width;
            return width;
        }
    }

    public void Load()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        float start = 0;
        foreach (var chunk in chunks)
        {
            var loadedChunk = Instantiate(chunk, transform);
            loadedChunk.transform.localPosition = new Vector2(start, 0);
            loadedChunk.name = chunk.name;
            //loadedChunk.LoadChunk();
            start += loadedChunk.Width;
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Chunk)), CanEditMultipleObjects]
internal class ChunkEditor : Editor
{
    private Chunk gameObject;

    public void OnEnable()
    {
        gameObject = (serializedObject.targetObject as Chunk);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Reload chunk"))
        {
            gameObject.Load();
        }
    }
}

#endif