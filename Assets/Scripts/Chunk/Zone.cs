using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Zone zoneToLeft;
    public Zone zoneToRight;

    public float chunkSize = 4;
    public List<GameObject> chunks = new List<GameObject>();

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}