﻿using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ChunkDecoratorData
{
    public string name;
    public Dictionary<string, object> properties;
}

[DisallowMultipleComponent]
public class ChunkDecorator : MonoBehaviour
{
    public ChunkDecoratorData data;

    public static GameObject Create(ChunkDecoratorData decorator, ChunkDecorators decorators = null)
    {
        var generator = decorators ? decorators : ChunkDecorators.Instance;
        var obj = generator.decoratos.GetValue(decorator.name, null);
        if (obj == null)
            return null;

        obj = Instantiate(obj);

        var assetData = obj.GetComponent<ChunkDecorator>();
        if (assetData == null)
            assetData = obj.AddComponent<ChunkDecorator>();

        assetData.data = decorator;

        return obj;
    }

    public void Initialize()
    {
        transform.localPosition = data.properties.GetValueCasting("position", new Vector3());
    }

    public T GetProperty<T>(string name, T ifNotFound = default(T))
    {
        return data.properties.GetValueCasting(name, ifNotFound);
    }
}