using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PixelPosition : MonoBehaviour
{
    public float PixelsPerUnit;
    public Vector3 Position;

    public void LateUpdate()
    {
        transform.localPosition = Position / PixelsPerUnit;
    }
}