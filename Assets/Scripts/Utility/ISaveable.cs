using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISaveable
{
    void Load(SaveableData data);

    SaveableData Save();
}

public struct SaveableData
{
    public Dictionary<string, object> data;

    public T GetProperty<T>(string name, T ifNotFound = default(T))
    {
        return data.GetValueCasting(name, ifNotFound);
    }

    public void SetProperty(string name, object property)
    {
        data[name] = property;
    }
}