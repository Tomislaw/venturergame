using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterBodyRotate : MonoBehaviour
{
    [SerializeField]
    private float heightInPixels;

    [SerializeField]
    private float pixelsPerUnit = 32;

    private float height = 0;

    public void SetHeight(float heightInPixels)
    {
        this.heightInPixels = heightInPixels;
        Invalidate();
    }

    public void SetPixelsPerUnit(float pixelsPerUnit)
    {
        this.pixelsPerUnit = pixelsPerUnit;
        Invalidate();
    }

    private void Update()
    {
        Invalidate();
    }

    private void Awake()
    {
        Invalidate();
    }

    private void OnValidate()
    {
        Invalidate();
    }

    public void Invalidate()
    {
        height = heightInPixels / pixelsPerUnit;
        transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
        foreach (Transform child in transform)
        {
            child.localPosition = new Vector3(child.localPosition.x, -height, child.localPosition.z);
        }
    }
}