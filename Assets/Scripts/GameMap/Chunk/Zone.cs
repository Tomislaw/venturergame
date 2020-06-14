using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Zone zoneToLeft;
    public Zone zoneToRight;

    public float chunkSize = 10;
    public List<Chunk> chunks = new List<Chunk>();

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void CreateFromRegion(WorldStructures.Serializable.Region region)
    {
    }
}