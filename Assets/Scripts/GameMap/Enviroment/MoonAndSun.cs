﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonAndSun : MonoBehaviour
{
    public Vector2 offset;

    [Range(0, 1)]
    public float value = 0;

    private void Start()
    {
    }

    // Update is called once per frame

    private void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, 360f * value);
        foreach (Transform child in transform)
        {
            child.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y) + offset;
    }
}