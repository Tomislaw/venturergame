using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkDecoratorDictionary : MonoBehaviour
{
    public Dictionary<string, GameObject> entries;

    private void Start()
    {
        var list = Resources.LoadAll<GameObject>("Prefabs/ChunkDecorators");
        foreach (var item in list)
        {
            entries.Add(item.name, item);
        }
    }
}