using ChunkGenerators;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomChunkGenerator : MonoBehaviour
{
    public int style = 0;
    public static int CHUNK_SIZE = 24;
    public ChunkDecorators chunkDecorators;

    private bool needUpdating = false;
    private int _previousStyle = 0;

    private void Awake()
    {
        needUpdating = true;
    }

    private void Update()
    {
        needUpdating = _previousStyle != style;
        if (needUpdating)
            Reload();
    }

    public void Reload()
    {
        UnloadChunk();
        LoadChunk();
        needUpdating = false;
        _previousStyle = style;
    }

    private void LoadChunk()
    {
        List<ChunkDecoratorData> items;
        if (style == 0)
            items = DecidousChunkGenerator.Forest.GenerateForest();
        else if (style == 1)
            items = DecidousChunkGenerator.LightForest.GenerateForest();
        else if (style == 2)
            items = DecidousChunkGenerator.Grassland.GenerateForest();
        else
            items = DecidousChunkGenerator.NoTrees.GenerateForest();

        int counter = 0;
        foreach (var item in items)
        {
            var go = ChunkDecorator.Create(item, chunkDecorators);

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

    private void UnloadChunk()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
#if UNITY_EDITOR
            if (Application.isEditor)
                GameObject.DestroyImmediate(child.gameObject);
#endif
        }
    }
}