using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDecorator : MonoBehaviour
{
    public string Name = "";
    public Dictionary<string, object> Properties = new Dictionary<string, object>();

    public static GameObject Create(ChunkDecoratorDictionary dictionary, ChunkDecorator data)
    {
        var obj = dictionary.entries.GetValue(data.Name, null);
        var assetData = obj.GetComponent<ChunkDecorator>();
        if (assetData == null)
            assetData = obj.AddComponent<ChunkDecorator>();

        foreach (var p in data.Properties)
            assetData.Properties.Add(p.Key, p.Value);

        return obj;
    }

    private void Start()
    {
        transform.position = Properties.GetValueCasting("position", new Vector2());
    }
}