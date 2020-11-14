using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public string zoneName;

    public List<Chunk> chunks = new List<Chunk>();

    private List<Chunk> _loadedChunks = new List<Chunk>();

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

    private void Awake()
    {
        Load();
    }

    public void Load()
    {
        float start = 0;
        foreach (var chunk in chunks)
        {
            var loadedChunk = Instantiate(chunk);
            loadedChunk.transform.SetParent(this.transform, false);
            loadedChunk.transform.localPosition = new Vector2(start, 0);
            //loadedChunk.LoadChunk();
            start += loadedChunk.Width;
            _loadedChunks.Add(loadedChunk);
        }
    }
}