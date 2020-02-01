using ChunkGenerators;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 24;

    [SerializeField]
    public ChunkDecoratorDictionary dictionary;

    public List<ChunkDecoratorData> items;

    public bool Loaded
    {
        get { return !(items != null && items.Count > 0); }
    }

    private void Start()
    {
        items = DecidousChunkGenerator.GenerateForest();
        LoadChunk();
    }

    public void LoadChunk()
    {
        if (Loaded)
            return;

        int counter = 0;
        foreach (var item in items)
        {
            var go = ChunkDecorator.Create(dictionary, item);

            if (go == null)
            {
                Debug.LogWarning("Item not found: " + item.name);
                continue;
            }
            go.name = item.name;
            go.transform.parent = gameObject.transform;
            go.GetComponent<ChunkDecorator>().Initialize();
        }
        items.Clear();
    }

    public void UnloadChunk()
    {
        if (!Loaded)
            return;

        items = new List<ChunkDecoratorData>();

        foreach (var child in GetComponentsInChildren<ChunkDecorator>())
        {
            items.Add(child.data);
        }

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}